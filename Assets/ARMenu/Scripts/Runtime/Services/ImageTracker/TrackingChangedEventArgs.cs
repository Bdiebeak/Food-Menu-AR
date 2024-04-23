using System;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace ARMenu.Scripts.Runtime.Services.ImageTracker
{
	public class TrackingChangedEventArgs : EventArgs
	{
		public ARTrackedImage Current { get; private set; }
		public ARTrackedImage Previous { get; private set; }
		public bool HasPrevious { get; private set; }

		public TrackingChangedEventArgs(ARTrackedImage current)
		{
			HasPrevious = false;
			Current = current;
		}

		public TrackingChangedEventArgs(ARTrackedImage previous, ARTrackedImage current)
		{
			HasPrevious = true;
			Current = current;
			Previous = previous;
		}
	}
}