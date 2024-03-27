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
            // TODO: refactoring
            foreach (ARTrackedImage addedImage in obj.added)
            {
                Dish dish = menu.dishes.FirstOrDefault(x => string.Equals(x.prefab.name, addedImage.referenceImage.name));
                if (dish == null)
                {
                    continue;
                }
                Instantiate(dish.prefab, addedImage.transform);
            }
        }
    }
}
