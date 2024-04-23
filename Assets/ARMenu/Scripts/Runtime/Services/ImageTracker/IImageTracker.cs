using System;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace ARMenu.Scripts.Runtime.Services.ImageTracker
{
	public interface IImageTracker
	{
		public event Action<ARTrackedImage> Added;
		public event Action<ARTrackedImage> Updated;
		public event Action<ARTrackedImage> Removed;
		public event Action<TrackingChangedEventArgs> ActiveTrackingChanged;
		public event Action AnyTrackingLost;

		public void Initialize(XRReferenceImageLibrary imageLibrary);
	}
}