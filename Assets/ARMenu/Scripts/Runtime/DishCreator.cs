using System.Collections.Generic;
using System.Linq;
using ARMenu.Scripts.Runtime.Data;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace ARMenu.Scripts.Runtime
{
    public class DishCreator : MonoBehaviour
    {
        [SerializeField]
        private string menuKey = "General/BurgersMenu.asset";
        [SerializeField]
        private string imageLibraryKey = "General/BurgerJointImageLibrary.asset";

        private Menu _menu;
        private XRReferenceImageLibrary _imageLibrary;
        private ARTrackedImageManager _trackedImageManager;

        private Dictionary<string, GameObject> _spawnedDishes = new();

        [ContextMenu("Initialize")]
        private void Initialize()
        {
            InitializeAddressablesAsync();
            InitializeTrackedImageManager();
        }

        private async void InitializeAddressablesAsync()
        {
            AsyncOperationHandle<Menu> menuLoadHandler = Addressables.LoadAssetAsync<Menu>(menuKey);
            await menuLoadHandler.Task;
            _menu = menuLoadHandler.Result;

            AsyncOperationHandle<XRReferenceImageLibrary> imageLibraryHandler = Addressables.LoadAssetAsync<XRReferenceImageLibrary>(imageLibraryKey);
            await imageLibraryHandler.Task;
            _imageLibrary = imageLibraryHandler.Result;
        }

        private void InitializeTrackedImageManager()
        {
            _trackedImageManager = gameObject.AddComponent<ARTrackedImageManager>();
            _trackedImageManager.referenceLibrary = _imageLibrary;
            _trackedImageManager.requestedMaxNumberOfMovingImages = 2;
            _trackedImageManager.enabled = true;
        }

        private void OnEnable()
        {
            _trackedImageManager.trackedImagesChanged += OnTrackedImageChanged;
        }

        private void OnDisable()
        {
            _trackedImageManager.trackedImagesChanged -= OnTrackedImageChanged;
        }

        private void OnTrackedImageChanged(ARTrackedImagesChangedEventArgs obj)
        {
            // TODO: cleanup on tracked image removed and unfocused.
            foreach (ARTrackedImage addedImage in obj.added)
            {
                DishToImageNode dishToImageNode = _menu.dishes.FirstOrDefault(x => x.arImage.name.Equals(addedImage.referenceImage.texture.name));
                if (dishToImageNode == null)
                {
                    Debug.LogError($"BDIEBEAK: Can't find dish for required image {addedImage.name}.");
                    continue;
                }

                Debug.Log((string)dishToImageNode.dish.prefab.RuntimeKey);
                // GameObject dish = Instantiate(dishToImageNode.dish.prefab, addedImage.transform);
                // _spawnedDishes[addedImage.name] = dish;
            }

            foreach (ARTrackedImage addedImage in obj.updated)
            {
                if (_spawnedDishes.TryGetValue(addedImage.name, out GameObject dish) == false)
                {
                    continue;
                }
                dish.transform.position = addedImage.transform.position;
            }

            foreach (ARTrackedImage addedImage in obj.removed)
            {
                if (_spawnedDishes.TryGetValue(addedImage.name, out GameObject dish) == false)
                {
                    continue;
                }
                Destroy(dish);
            }
        }
    }
}
