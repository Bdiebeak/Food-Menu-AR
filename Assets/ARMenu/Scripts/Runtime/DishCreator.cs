using System;
using System.Linq;
using ARMenu.Scripts.Runtime.Data;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ARMenu.Scripts.Runtime
{
    public class DishCreator : MonoBehaviour
    {
        [SerializeField]
        private Menu menu;
        [SerializeField]
        private ARTrackedImageManager trackedImageManager;

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
            // TODO: only one object at time.
            // TODO: cleanup on tracked image removed and unfocused.
            foreach (ARTrackedImage addedImage in obj.added)
            {
                // I use GUID here to avoid dependencies from naming.
                DishToImageNode dishToImageNode = menu.dishes.FirstOrDefault(x => Guid.Parse(x.imageGuid) == addedImage.referenceImage.textureGuid);
                if (dishToImageNode == null)
                {
                    continue;
                }
                Instantiate(dishToImageNode.dish.prefab, addedImage.transform);
            }
        }
    }
}
