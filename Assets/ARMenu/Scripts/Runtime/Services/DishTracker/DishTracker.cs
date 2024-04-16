using System;
using System.Collections.Generic;
using ARMenu.Scripts.Runtime.Data;
using ARMenu.Scripts.Runtime.Data.ImageLibrary;
using ARMenu.Scripts.Runtime.Services.AssetProvider;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Object = UnityEngine.Object;

namespace ARMenu.Scripts.Runtime.Services.DishTracker
{
	public class DishTracker : IDishTracker
	{
		public event Action<Dish> TrackingChanged;
		public event Action TrackingLost;

		private readonly IAssetProvider _assetProvider;
		private readonly ARTrackedImageManager _trackedImageManager;
		private readonly Dictionary<string, GameObject> _trackingDishes = new();
		private readonly CurrentTrackedData _currentTrackedData = new();

		private IImageLibrary<Dish> _imageLibrary;

		public DishTracker(IAssetProvider assetProvider,
						   ARTrackedImageManager trackedImageManager)
		{
			_assetProvider = assetProvider;
			_trackedImageManager = trackedImageManager;
		}

		public void Initialize(IImageLibrary<Dish> dishImageLibrary)
		{
			if (_imageLibrary != null)
			{
				Debug.LogError("Initialization was already completed. Reinitialization is not supported.");
				return;
			}

			_imageLibrary = dishImageLibrary;
			_trackedImageManager.referenceLibrary = dishImageLibrary.GetReferenceImageLibrary();
			_trackedImageManager.trackablesChanged.AddListener(OnTrackedImageChanged);
			_trackedImageManager.enabled = true;
		}

		public void CleanUp()
		{
			_trackedImageManager.trackablesChanged.RemoveListener(OnTrackedImageChanged);
			CleanTrackingDishes();
		}

		private void OnTrackedImageChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
		{
			HandleAddedImages(eventArgs.added);
			HandleUpdatedImages(eventArgs.updated);
			HandleRemovedImages(eventArgs.removed);
		}

		/// <summary>
		/// Added is called only once, when the subsystem find that the trackable was scanned for the first time.
		/// We can't ignore it. We should create prefab for tracked image.
		/// </summary>
		/// <param name="addedImages"> Images which were added this frame. </param>
		private void HandleAddedImages(IReadOnlyCollection<ARTrackedImage> addedImages)
		{
			foreach (ARTrackedImage addedImage in addedImages)
			{
				// Get linked dish prefab.
				if (addedImage.referenceImage.texture == null)
				{
					Debug.LogError($"Can't find texture in reference image {addedImage.name}.");
					continue;
				}

				if (TryGetLinkedDishData(addedImage, out Dish linkedDish) == false)
				{
					continue;
				}

				// Spawn prefab when its tracked image was added.
				AddDishPrefabAsync(addedImage, linkedDish);
			}
		}

		/// <summary>
		/// Update should be called if the subsystem find that the trackable was changed.
		/// This logic isn't determinate, because we don't know the order of updated images.
		/// </summary>
		/// <param name="updatedImages"> Images which were updated this frame. </param>
		private void HandleUpdatedImages(IReadOnlyCollection<ARTrackedImage> updatedImages)
		{
			ARTrackedImage currentTrackedImage = _currentTrackedData.TrackedImage;
			if (currentTrackedImage != null)
			{
				// Don't handle other tracked images if we already have right one.
				if (currentTrackedImage.trackingState == TrackingState.Tracking)
				{
					return;
				}

				// Clean current tracked data if current image doesn't have Tracking state.
				_currentTrackedData.TrackedObject.SetActive(false);
				_currentTrackedData.Clean();

				// If new Tracked image wasn't found.
				TrackingLost?.Invoke();
			}

			foreach (ARTrackedImage updatedImage in updatedImages)
			{
				// Find tracked image with Tracking state.
				if (updatedImage.trackingState != TrackingState.Tracking)
				{
					continue;
				}
				// Find tracked image with prepared Dish game object.
				if (_trackingDishes.TryGetValue(updatedImage.name, out GameObject dish) == false)
				{
					continue;
				}
				// Find tracked image with linked Dish data.
				if (TryGetLinkedDishData(updatedImage, out Dish dishData) == false)
				{
					continue;
				}

				// Reinitialize current tracked values.
				_currentTrackedData.Set(updatedImage, dish);
				dish.SetActive(true);
				TrackingChanged?.Invoke(dishData);

				// Stop function, because we have found required image.
				return;
			}
		}

		/// <summary>
		/// Remove should be called if the subsystem can't find the trackable again.
		/// But it doesn't seem to remove these at all, but if it does, it would delete linked dish object.
		/// </summary>
		/// <param name="removedImages"> Images which were removed this frame. </param>
		private void HandleRemovedImages(IReadOnlyCollection<ARTrackedImage> removedImages)
		{
			foreach (ARTrackedImage removedImage in removedImages)
			{
				if (_trackingDishes.TryGetValue(removedImage.name, out GameObject dish) == false)
				{
					continue;
				}

				_trackingDishes.Remove(removedImage.name);
				CleanTrackingDish(dish);
			}
		}

		private bool TryGetLinkedDishData(ARTrackedImage addedImage, out Dish linkedDish)
		{
			string imageName = addedImage.referenceImage.texture.name;
			if (_imageLibrary.TryGetLinkedDish(imageName, out linkedDish))
			{
				return true;
			}

			Debug.LogError($"Can't find linked dish for {imageName} image.");
			return false;
		}

		// TODO: fix problem when instantiate is called after removed.
		private async void AddDishPrefabAsync(ARTrackedImage addedImage, Dish linkedDish)
		{
			GameObject dishPrefab = await _assetProvider.LoadAssetAsync<GameObject>(linkedDish.prefabPath);
			GameObject dish = Object.Instantiate(dishPrefab, addedImage.transform);
			dish.SetActive(false); // Deactivate object, because it visible state is set inside HandleUpdatedImages method.
			_trackingDishes[addedImage.name] = dish;
		}

		private void CleanTrackingDishes()
		{
			foreach (GameObject dish in _trackingDishes.Values)
			{
				CleanTrackingDish(dish);
			}
			_trackingDishes.Clear();
		}

		private void CleanTrackingDish(GameObject dish)
		{
			Object.Destroy(dish);
		}
	}
}