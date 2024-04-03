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
        private IAssetProvider _assetProvider;
        private ARTrackedImageManager _trackedImageManager;

        private DishImageLibrary _imageLibrary;
        private Dictionary<string, GameObject> _spawnedDishes = new();

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
            foreach (ARTrackedImage addedImage in obj.added)
            {
                Debug.Log($"Tracked Image Added {addedImage.name}");
                OnTrackedImageAddedAsyncHandler(addedImage);
            }

            foreach (ARTrackedImage addedImage in obj.updated)
            {
                OnTrackedImageUpdatedHandler(addedImage);
            }

            foreach (ARTrackedImage addedImage in obj.removed)
            {
                Debug.Log($"Tracked Image Removed {addedImage.name}");
                OnTrackedImageRemovedHandler(addedImage);
            }
        }

        private async void OnTrackedImageAddedAsyncHandler(ARTrackedImage trackedImage)
        {
            if (trackedImage.referenceImage.texture == null)
            {
                Debug.LogError($"Can't find texture in reference image {trackedImage.name}.");
                return;
            }

            string imageName = trackedImage.referenceImage.texture.name;
            if (_imageLibrary.TryGetLinkedDish(imageName, out Dish linkedDish) == false)
            {
                Debug.LogError($"Can't find linked dish for {imageName} image.");
                return;
            }
            GameObject dishPrefab = await _assetProvider.LoadAssetAsync<GameObject>(linkedDish.prefabPath);
            GameObject dish = Object.Instantiate(dishPrefab, trackedImage.transform);
            _spawnedDishes[trackedImage.name] = dish;
        }

        private void OnTrackedImageUpdatedHandler(ARTrackedImage trackedImage)
        {
            if (_spawnedDishes.TryGetValue(trackedImage.name, out GameObject dish) == false)
            {
                return;
            }

            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                dish.SetActive(true);
            }
            else if (trackedImage.trackingState == TrackingState.None)
            {
                dish.SetActive(false);
            }
        }

        private void OnTrackedImageRemovedHandler(ARTrackedImage trackedImage)
        {
            if (_spawnedDishes.TryGetValue(trackedImage.name, out GameObject dish) == false)
            {
                return;
            }
            Object.Destroy(dish);
            _spawnedDishes.Remove(trackedImage.name);
        }
    }
}
