using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace ARMenu.Scripts.Runtime.Services.ImageTracker
{
	public class SingleImageTracker : IImageTracker
	{
		public event Action<ARTrackedImage> Added;
		public event Action<ARTrackedImage> Updated;
		public event Action<ARTrackedImage> Removed;
		public event Action<TrackingChangedEventArgs> ActiveTrackingChanged;
		public event Action AnyTrackingLost;

		private XRReferenceImageLibrary _imageLibrary;
		private ARTrackedImage _currentTrackedImage;

		private readonly ARTrackedImageManager _trackedImageManager;

		public SingleImageTracker(ARTrackedImageManager trackedImageManager)
		{
			_trackedImageManager = trackedImageManager;
		}

		public void Initialize(XRReferenceImageLibrary imageLibrary)
		{
			if (_imageLibrary != null)
			{
				Debug.LogError("Initialization was already completed. Reinitialization is not supported.");
				return;
			}

			_imageLibrary = imageLibrary;
			_trackedImageManager.referenceLibrary = imageLibrary;
			_trackedImageManager.trackablesChanged.AddListener(OnTrackedImageChanged);
			_trackedImageManager.enabled = true;
		}

		private void OnTrackedImageChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
		{
			HandleAddedImages(eventArgs.added);
			HandleUpdatedImages(eventArgs.updated);
			HandleRemovedImages(eventArgs.removed);
		}

		/// <summary>
		/// Added is called only once, when the subsystem find that the trackable was scanned for the first time.
		/// </summary>
		/// <param name="addedImages"> Images which were added this frame. </param>
		private void HandleAddedImages(IReadOnlyCollection<ARTrackedImage> addedImages)
		{
			foreach (ARTrackedImage addedImage in addedImages)
			{
				Added?.Invoke(addedImage);
			}
		}

		/// <summary>
		/// Update should be called if the subsystem find that the trackable was changed.
		/// This logic isn't determinate, because we don't know the order of updated images.
		/// </summary>
		/// <param name="updatedImages"> Images which were updated this frame. </param>
		private void HandleUpdatedImages(IReadOnlyCollection<ARTrackedImage> updatedImages)
		{
			// Don't process empty collection.
			if (updatedImages == null || updatedImages.Count == 0)
			{
				return;
			}

			bool hasActiveTracking = false;
			foreach (ARTrackedImage updatedImage in updatedImages)
			{
				Updated?.Invoke(updatedImage);

				// Skip logic below if active image was already found.
				// It's required to call Updated event for each image without duplication of foreach cycle.
				if (hasActiveTracking)
				{
					continue;
				}

				// Skip processing if image is not in the Tracking state.
				if (updatedImage.trackingState != TrackingState.Tracking)
				{
					continue;
				}

				if (_currentTrackedImage != null &&
					_currentTrackedImage.trackingState == TrackingState.Tracking)
				{
					// Skip processing if image is not the same as the current one.
					if (updatedImage != _currentTrackedImage)
					{
						continue;
					}
				}
				else
				{
					// Update tracking if there is no currently tracked image or it's not in the Tracking state.
					UpdateCurrentTracking(updatedImage);
				}

				hasActiveTracking = true;
			}

			// If all tracking images are not in the Tracking state, so we don't have any tracking.
			if (hasActiveTracking == false)
			{
				_currentTrackedImage = null;
				AnyTrackingLost?.Invoke();
			}
		}

		/// <summary>
		/// Remove should be called if the subsystem can't find the trackable again.
		/// But it doesn't seem to remove these at all, but if it does, it would delete linked dish object.
		/// </summary>
		/// <param name="removedImages"> Images which were removed this frame. </param>
		private void HandleRemovedImages(IReadOnlyCollection<ARTrackedImage> removedImages)
		{
			foreach (ARTrackedImage removedImage in removedImages)
			{
				if (removedImage == _currentTrackedImage)
				{
					_currentTrackedImage = null;
				}

				Removed?.Invoke(removedImage);
			}
		}

		private void UpdateCurrentTracking(ARTrackedImage newTrackedImage)
		{
			TrackingChangedEventArgs eventArgs = _currentTrackedImage == null
													 ? new TrackingChangedEventArgs(newTrackedImage)
													 : new TrackingChangedEventArgs(_currentTrackedImage, newTrackedImage);
			_currentTrackedImage = newTrackedImage;
			ActiveTrackingChanged?.Invoke(eventArgs);
		}
	}
}