using System.Collections.Generic;
using System.Linq;
using ARMenu.Scripts.Runtime.Data;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace ARMenu.Scripts.Runtime
{
    public class DishCreator : MonoBehaviour
    {
        [SerializeField]
        private Menu menu;
        [SerializeField]
        private XRReferenceImageLibrary imageLibrary;

        private ARTrackedImageManager _trackedImageManager;

        private Dictionary<string, GameObject> _spawnedDishes = new();

        private void Awake()
        {
            InitializeTrackedImageManager();
        }

        private void InitializeTrackedImageManager()
        {
            _trackedImageManager = gameObject.AddComponent<ARTrackedImageManager>();
            _trackedImageManager.referenceLibrary = imageLibrary;
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
                Debug.Log("BDIEBEAK: On Tracked Image Added.");
                DishToImageNode dishToImageNode = menu.dishes.FirstOrDefault(x => x.arImage.name.Equals(addedImage.referenceImage.texture.name));
                if (dishToImageNode == null)
                {
                    Debug.LogError($"BDIEBEAK: Can't find dish for required image {addedImage.name}.");
                    continue;
                }
                GameObject dish = Instantiate(dishToImageNode.dish.prefab, addedImage.transform);
                _spawnedDishes[addedImage.name] = dish;
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
