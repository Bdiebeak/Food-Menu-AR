using System;
using System.Collections.Generic;
using ARMenu.Scripts.Runtime.Data;
using ARMenu.Scripts.Runtime.Data.ImageLibrary;
using ARMenu.Scripts.Runtime.Data.ImageLibrary.Dishes;
using ARMenu.Scripts.Runtime.Services.AssetProvider;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Object = UnityEngine.Object;

namespace ARMenu.Scripts.Runtime.Services.DishTracker
{
	public class TrackingData
	{
		public Dish cachedDish;
		public GameObject spawnedPrefab;
	}

	public class DishTracker : IDishTracker
	{
		public event Action<Dish> TrackingChanged;
		public event Action TrackingLost;

		private readonly Dictionary<string, TrackingData> _trackingDishes = new();
		private readonly CurrentTrackedData _currentTrackedData = new();
		private readonly IAssetProvider _assetProvider;
		private readonly ARTrackedImageManager _trackedImageManager;

		private DishImageLibrary _imageLibrary;

		public DishTracker(IAssetProvider assetProvider,
						   ARTrackedImageManager trackedImageManager)
		{
			_assetProvider = assetProvider;
			_trackedImageManager = trackedImageManager;
		}

		public void Initialize(DishImageLibrary dishImageLibrary)
		{
			if (_imageLibrary != null)
			{
				Debug.LogError("Initialization was already completed. Reinitialization is not supported.");
				return;
			}

			_imageLibrary = dishImageLibrary;
			_trackedImageManager.referenceLibrary = dishImageLibrary.XRReferenceImageLibrary;
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
				PrepareDishReference(addedImage);
			}
		}

		private async void PrepareDishReference(ARTrackedImage addedImage)
		{
			XRReferenceImage referenceImage = addedImage.referenceImage;
			if (_imageLibrary.TryGetLinkedData(referenceImage.name, out AssetReferenceDish dishReference) == false)
			{
				Debug.LogError("Can't find linked dish asset reference.");
				return;
			}

			// Spawn prefab when its tracked image was added.
			Dish linkedDish = await _assetProvider.LoadAssetAsync<Dish>(dishReference);
			if (_trackingDishes.TryGetValue(addedImage.name, out TrackingData trackingData) == false)
			{
				trackingData = new TrackingData();
				_trackingDishes[addedImage.name] = trackingData;
			}
			trackingData.cachedDish = linkedDish;
			AddDishPrefab(addedImage, linkedDish);
		}

		private void AddDishPrefab(ARTrackedImage addedImage, Dish linkedDish)
		{
			GameObject dish = Object.Instantiate(linkedDish.prefab, addedImage.transform);
			dish.SetActive(false); // Deactivate object, because it visible state is set inside HandleUpdatedImages method.
			_trackingDishes[addedImage.name].spawnedPrefab = dish;
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
				if (_trackingDishes.TryGetValue(updatedImage.name, out TrackingData trackingData) == false)
				{
					continue;
				}

				// Reinitialize current tracked values.
				GameObject dishObject = trackingData.spawnedPrefab;
				_currentTrackedData.Set(updatedImage, dishObject);
				dishObject.SetActive(true);
				TrackingChanged?.Invoke(trackingData.cachedDish);

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
				if (_trackingDishes.TryGetValue(removedImage.name, out TrackingData trackingData) == false)
				{
					continue;
				}

				_trackingDishes.Remove(removedImage.name);
				CleanTrackingDish(trackingData.spawnedPrefab);
			}
		}

		private void CleanTrackingDishes()
		{
			foreach (TrackingData trackingData in _trackingDishes.Values)
			{
				CleanTrackingDish(trackingData.spawnedPrefab);
			}
			_trackingDishes.Clear();
		}

		private void CleanTrackingDish(GameObject dish)
		{
			Object.Destroy(dish);
		}
	}
}