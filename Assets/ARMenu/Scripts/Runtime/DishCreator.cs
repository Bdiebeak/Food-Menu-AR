using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ARMenu.Scripts.Runtime.Data;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.XR.ARFoundation;

namespace ARMenu.Scripts.Runtime
{
    [RequireComponent(typeof(ARTrackedImageManager))]
    public class DishCreator : MonoBehaviour
    {
        [SerializeField]
        private ARTrackedImageManager trackedImageManager;

        private DishImageLibrary _imageLibrary;
        private Dictionary<string, GameObject> _spawnedDishes = new();

        public void Construct(DishImageLibrary menu)
        {
            _imageLibrary = menu;

            // TODO: should it be here?
            trackedImageManager.referenceLibrary = _imageLibrary;
            trackedImageManager.enabled = true;
        }

        private void OnEnable()
        {
            trackedImageManager.trackedImagesChanged += OnTrackedImageChanged;
        }

        private void OnDisable()
        {
            trackedImageManager.trackedImagesChanged -= OnTrackedImageChanged;
        }

        private void OnTrackedImageChanged(ARTrackedImagesChangedEventArgs obj)
        {
            foreach (ARTrackedImage addedImage in obj.added)
            {
                ReferenceImageToDishNode referenceImageToDishNode = _imageLibrary.imageToDishNodes.FirstOrDefault(x => x.arImage.name.Equals(addedImage.referenceImage.texture.name));
                if (referenceImageToDishNode == null)
                {
                    Debug.LogError($"BDIEBEAK: Can't find dish for required image {addedImage.name}.");
                    continue;
                }
                OnTrackedImageAddedAsyncHandler(referenceImageToDishNode.dish.prefabPath, addedImage);
            }

            // TODO: cleanup on tracked image removed and unfocused.
            // foreach (ARTrackedImage addedImage in obj.updated)
            // {
            //     if (_spawnedDishes.TryGetValue(addedImage.name, out GameObject dish) == false)
            //     {
            //         continue;
            //     }
            //     dish.transform.position = addedImage.transform.position;
            // }
            //
            // foreach (ARTrackedImage addedImage in obj.removed)
            // {
            //     if (_spawnedDishes.TryGetValue(addedImage.name, out GameObject dish) == false)
            //     {
            //         continue;
            //     }
            //     Destroy(dish);
            // }
        }

        private async void OnTrackedImageAddedAsyncHandler(string dishPrefabKey, ARTrackedImage trackedImage)
        {
            GameObject prefab = await LoadAssetAsync<GameObject>(dishPrefabKey);
            GameObject dish = Instantiate(prefab, trackedImage.transform);
            _spawnedDishes[trackedImage.name] = dish;
        }

        private void Reset()
        {
            trackedImageManager = GetComponent<ARTrackedImageManager>();
        }

#region DEBUG

        [SerializeField]
        private string libraryKey = "General/BurgersLibrary.asset";

        [ContextMenu("Initialize")]
        private async void Initialize()
        {
            DishImageLibrary menu = await LoadAssetAsync<DishImageLibrary>(libraryKey);
            Construct(menu);
        }

        private async Task<T> LoadAssetAsync<T>(string key)
        {
            AsyncOperationHandle<T> assetLoadHandler = Addressables.LoadAssetAsync<T>(key);
            return await assetLoadHandler.Task;
        }

#endregion
    }
}
