using System;
using System.Collections.Generic;
using ARMenu.Scripts.Runtime.Data;
using ARMenu.Scripts.Runtime.Services.AssetProvider;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Object = UnityEngine.Object;

namespace ARMenu.Scripts.Runtime.Services.DishTracker
{
	public class DishTrackerAR : IDishTrackerAR
	{
		public event Action<Dish> TrackingDishChanged;

		private readonly IAssetProvider _assetProvider;
		private readonly ARTrackedImageManager _trackedImageManager;

		private DishImageLibrary _imageLibrary;
		private Dictionary<string, GameObject> _trackingDishes = new();

		public DishTrackerAR(IAssetProvider assetProvider,
							 ARTrackedImageManager trackedImageManager)
		{
			_assetProvider = assetProvider;
			_trackedImageManager = trackedImageManager;
		}

		public void SetImageLibrary(DishImageLibrary initialLibrary)
		{
			if (_imageLibrary != null)
			{
				Debug.LogError("Reinitialization of ImageLibrary in runtime is not supported. It was already initialized.");
				return;
			}

			_imageLibrary = initialLibrary;
			_trackedImageManager.referenceLibrary = initialLibrary;
		}

		public void Start()
		{
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
					return;
				}

				string imageName = addedImage.referenceImage.texture.name;
				if (_imageLibrary.TryGetLinkedDish(imageName, out Dish linkedDish) == false)
				{
					Debug.LogError($"Can't find linked dish for {imageName} image.");
					return;
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
			foreach (ARTrackedImage updatedImage in updatedImages)
			{
				if (_trackingDishes.TryGetValue(updatedImage.name, out GameObject dish) == false)
				{
					return;
				}
				dish.SetActive(updatedImage.trackingState == TrackingState.Tracking);
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
					return;
				}

				_trackingDishes.Remove(removedImage.name);
				CleanTrackingDish(dish);
			}
		}

		// TODO: fix problem when instantiate is called after removed.
		private async void AddDishPrefabAsync(ARTrackedImage addedImage, Dish linkedDish)
		{
			GameObject dishPrefab = await _assetProvider.LoadAssetAsync<GameObject>(linkedDish.prefabPath);
			GameObject dish = Object.Instantiate(dishPrefab, addedImage.transform);
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