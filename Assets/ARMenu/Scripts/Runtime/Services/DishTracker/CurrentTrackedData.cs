using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ARMenu.Scripts.Runtime.Services.DishTracker
{
	public class CurrentTrackedData
	{
		public ARTrackedImage TrackedImage { get; private set; }
		public GameObject TrackedObject { get; private set; }

		public void Set(ARTrackedImage trackedImage, GameObject trackedObject)
		{
			TrackedImage = trackedImage;
			TrackedObject = trackedObject;
		}

		public void Clean()
		{
			TrackedImage = null;
			TrackedObject = null;
		}
	}
}