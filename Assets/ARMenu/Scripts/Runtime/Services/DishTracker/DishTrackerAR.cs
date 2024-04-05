using System.Collections.Generic;
using ARMenu.Scripts.Runtime.Data;
using ARMenu.Scripts.Runtime.Services.AssetProvider;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace ARMenu.Scripts.Runtime.Services.DishTracker
{
    public class DishTrackerAR : IDishTrackerAR
    {
        private readonly IAssetProvider _assetProvider;
        private readonly ARTrackedImageManager _trackedImageManager;

        private DishImageLibrary _imageLibrary;
        private ARTrackedImage _lastTrackingImage;
        private Dictionary<string, GameObject> _addedDishes = new();

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
            _trackedImageManager.trackedImagesChanged += OnTrackedImageChanged;
            _trackedImageManager.enabled = true;
        }

        public void CleanUp()
        {
            _trackedImageManager.trackedImagesChanged -= OnTrackedImageChanged;
            // TODO: remove spawned dishes
        }

        private void OnTrackedImageChanged(ARTrackedImagesChangedEventArgs obj)
        {
            HandleAddedImages(obj.added);
            HandleUpdatedImages(obj.updated);
            HandleRemovedImages(obj.removed);
        }

        /// <summary>
        /// Added is called only once, when the subsystem find that the trackable was scanned for the first time.
        /// We can't ignore it. We should create prefab for tracked image.
        /// </summary>
        /// <param name="addedImages"> Images which were added this frame. </param>
        private void HandleAddedImages(List<ARTrackedImage> addedImages)
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
        private void HandleUpdatedImages(List<ARTrackedImage> updatedImages)
        {
            // If last tacking image didn't change it state, don't do anything and keep show it.
            int lastTrackingImageIndex = updatedImages.IndexOf(_lastTrackingImage);
            if (lastTrackingImageIndex != -1)
            {
                if (updatedImages[lastTrackingImageIndex].trackingState == TrackingState.Tracking)
                {
                    return;
                }
            }

            // If last tracking image did change it state, find first visible one and show it.
            bool wasTrackingFound = false;
            foreach (ARTrackedImage updatedImage in updatedImages)
            {
                if (_addedDishes.TryGetValue(updatedImage.name, out GameObject dish) == false)
                {
                    return;
                }
                if (wasTrackingFound == false &&
                    updatedImage.trackingState == TrackingState.Tracking)
                {
                    wasTrackingFound = true;
                    _lastTrackingImage = updatedImage;
                    dish.SetActive(true);
                }
                else
                {
                    dish.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Remove should be called if the subsystem can't find the trackable again.
        /// But it doesn't seem to remove these at all, but if it does, it would delete linked dish object.
        /// </summary>
        /// <param name="removedImages"> Images which were removed this frame. </param>
        private void HandleRemovedImages(List<ARTrackedImage> removedImages)
        {
            foreach (ARTrackedImage removedImage in removedImages)
            {
                if (_addedDishes.TryGetValue(removedImage.name, out GameObject dish) == false)
                {
                    return;
                }
                _addedDishes.Remove(removedImage.name);
                Object.Destroy(dish);
            }
        }

        private async void AddDishPrefabAsync(ARTrackedImage addedImage, Dish linkedDish)
        {
            GameObject dishPrefab = await _assetProvider.LoadAssetAsync<GameObject>(linkedDish.prefabPath);
            GameObject dish = Object.Instantiate(dishPrefab, addedImage.transform);
            _addedDishes[addedImage.name] = dish;

            Debug.Log($"Dish for {addedImage.name} was spawned.\n" +
                      $"Image size {addedImage.transform.lossyScale}, Prefab size: {dishPrefab.transform.lossyScale}");
        }
    }
}
