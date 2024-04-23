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
        private Dictionary<XRReferenceImage, Task<Dish>> _loadingOperations = new();
        private Dictionary<XRReferenceImage, Dish> _linkedDishes = new();

		private async void OnImageTrackerAdded(ARTrackedImage trackedImage)
		{
			Debug.Log($"Added: {trackedImage.referenceImage.name}");

			XRReferenceImage referenceImage = trackedImage.referenceImage;
			if (_imageLibrary.TryGetLinkedData(referenceImage, out AssetReferenceDish assetToLoad) == false)
			{
				Debug.LogError($"Can't find required dish by reference image {trackedImage.referenceImage.name}.");
				return;
			}

			if (_loadingOperations.ContainsKey(referenceImage))
			{
				Debug.LogWarning($"Linked data for {referenceImage.name} image is already loading.");
				return;
			}

			Task<Dish> loadingOperation = _assetProvider.LoadAssetAsync<Dish>(assetToLoad);
			_loadingOperations.Add(referenceImage, loadingOperation);

			Dish loadedAsset = await loadingOperation;
			_linkedDishes.Add(referenceImage, loadedAsset);
			_loadingOperations.Remove(referenceImage);
		}

		private void OnImageTrackerUpdated(ARTrackedImage trackedImage)
		{
			Debug.Log($"Updated: {trackedImage.referenceImage.name}");
		}

		private void OnImageTrackerRemoved(ARTrackedImage trackedImage)
		{
			Debug.Log($"Removed: {trackedImage.referenceImage.name}");
			// TODO: unload asset
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
			if (_loadingOperations.TryGetValue(_currentImage, out Task<Dish> operation))
			{
				_screenService.GetScreen<DishDescriptionUxmlScreen>().Hide();
				_screenService.GetScreen<HintUxmlScreen>().Show();
				_hintScreenModel.SetHint("Dish data is loading.");

				await operation;

				// Additional check to match the current image after async operation.
				if (_currentImage != eventArgs.Current.referenceImage)
				{
					return;
				}
			}

			_screenService.GetScreen<HintUxmlScreen>().Hide();
			_screenService.GetScreen<DishDescriptionUxmlScreen>().Show();
			_dishDescriptionModel.SetDish(_linkedDishes[_currentImage]);
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