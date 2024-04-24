using System.Collections.Generic;
using System.Threading.Tasks;
using ARMenu.Scripts.Runtime.Data;
using ARMenu.Scripts.Runtime.Data.ImageLibrary.Dishes;
using ARMenu.Scripts.Runtime.Infrastructure.Constants;
using ARMenu.Scripts.Runtime.Services.AssetProvider;
using ARMenu.Scripts.Runtime.Services.ImageTracker;
using ARMenu.Scripts.Runtime.Services.ScreenService;
using ARMenu.Scripts.Runtime.UI.DishDescription;
using ARMenu.Scripts.Runtime.UI.ScanHint;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace ARMenu.Scripts.Runtime.Infrastructure
{
	public class CoreRunner : MonoBehaviour
	{
		private IAssetProvider _assetProvider;
		private IImageTracker _imageTracker;
		private IScreenService _screenService;
		private DishDescriptionViewModel _dishDescriptionModel;
		private HintViewModel _hintScreenModel;
		private DishImageLibrary _imageLibrary;

		public void Initialize(IAssetProvider assetProvider,
							   IImageTracker imageTracker,
							   IScreenService screenService,
							   DishDescriptionViewModel dishDescriptionModel, HintViewModel hintScreenModel)
		{
			_assetProvider = assetProvider;
			_imageTracker = imageTracker;
			_screenService = screenService;
			_dishDescriptionModel = dishDescriptionModel;
			_hintScreenModel = hintScreenModel;
		}

		private async void Start()
		{
			_imageTracker.Added += OnImageTrackerAdded;
			_imageTracker.Updated += OnImageTrackerUpdated;
			_imageTracker.Removed += OnImageTrackerRemoved;
			_imageTracker.ActiveTrackingChanged += OnImageTrackerActiveTrackingChanged;
			_imageTracker.AnyTrackingLost += OnImageTrackerAnyTrackingLost;

			_screenService.GetScreen<HintUxmlScreen>().Show();
			_hintScreenModel.SetHint("Loading required data...");

			_imageLibrary = await _assetProvider.LoadAssetAsync<DishImageLibrary>(AssetKeys.BurgersLibraryKey);

			_imageTracker.Initialize(_imageLibrary.XRReferenceImageLibrary);
			_screenService.HideAll();
			_screenService.GetScreen<HintUxmlScreen>().Show();
			_hintScreenModel.SetHint("Please, scan a QR code.");
		}

		private void OnDestroy()
		{
			_imageTracker.Added -= OnImageTrackerAdded;
			_imageTracker.Updated -= OnImageTrackerUpdated;
			_imageTracker.Removed -= OnImageTrackerRemoved;
			_imageTracker.ActiveTrackingChanged -= OnImageTrackerActiveTrackingChanged;
			_imageTracker.AnyTrackingLost -= OnImageTrackerAnyTrackingLost;

			_screenService.HideAll();
		}

		private XRReferenceImage _currentImage;
        private Dictionary<XRReferenceImage, TaskCompletionSource<Dish>> _completionSources = new();

		private async void OnImageTrackerAdded(ARTrackedImage trackedImage)
		{
			Debug.Log($"Added: {trackedImage.referenceImage.name}");

			XRReferenceImage referenceImage = trackedImage.referenceImage;
			if (_imageLibrary.TryGetLinkedData(referenceImage, out AssetReferenceDish assetReference) == false)
			{
				Debug.LogError($"Can't find required dish by reference image {trackedImage.referenceImage.name}.");
				return;
			}

			if (_completionSources.ContainsKey(referenceImage))
			{
				return;
			}

			Task<Dish> loadingOperation = _assetProvider.LoadAssetAsync<Dish>(assetReference);
			TaskCompletionSource<Dish> taskCompletionSource = new();
			_completionSources.Add(referenceImage, taskCompletionSource);

			Dish loadedAsset = await loadingOperation;
			taskCompletionSource.SetResult(loadedAsset);
		}

		private void OnImageTrackerUpdated(ARTrackedImage trackedImage)
		{
			Debug.Log($"Updated: {trackedImage.referenceImage.name}");
		}

		private void OnImageTrackerRemoved(ARTrackedImage trackedImage)
		{
			Debug.Log($"Removed: {trackedImage.referenceImage.name}");
			XRReferenceImage referenceImage = trackedImage.referenceImage;
			if (_imageLibrary.TryGetLinkedData(referenceImage, out AssetReferenceDish assetReference) == false)
			{
				_assetProvider.ReleaseAsset(assetReference);
			}
		}

		private async void OnImageTrackerActiveTrackingChanged(TrackingChangedEventArgs eventArgs)
		{
			if (eventArgs.HasPrevious)
			{
				Debug.Log($"Changed: old {eventArgs.Previous.referenceImage.name} new {eventArgs.Current.referenceImage.name}");
			}
			else
			{
				Debug.Log($"Changed: new {eventArgs.Current.referenceImage.name}");
			}

			// TODO: deactivate previous and activate new.

			_currentImage = eventArgs.Current.referenceImage;
			if (_completionSources.TryGetValue(_currentImage, out TaskCompletionSource<Dish> completionSource) == false)
			{
				Debug.LogError($"Can't find completion source for {_currentImage.name}. It can mean that Added event wasn't handled for this.");
				return;
			}

			if (completionSource.Task.IsCompleted == false)
			{
				_screenService.GetScreen<DishDescriptionUxmlScreen>().Hide();
				_screenService.GetScreen<HintUxmlScreen>().Show();
				_hintScreenModel.SetHint("Dish data is loading.");

				await completionSource.Task;
			}

			// Additional check to match the current image after async operation.
			if (_currentImage != eventArgs.Current.referenceImage)
			{
				return;
			}

			_screenService.GetScreen<HintUxmlScreen>().Hide();
			_screenService.GetScreen<DishDescriptionUxmlScreen>().Show();
			_dishDescriptionModel.SetDish(completionSource.Task.Result);
		}

		private void OnImageTrackerAnyTrackingLost()
		{
			Debug.Log("Any lost");
			_screenService.GetScreen<DishDescriptionUxmlScreen>().Hide();
			_screenService.GetScreen<HintUxmlScreen>().Show();
			_hintScreenModel.SetHint("Please, scan a QR code.");
		}
	}
}