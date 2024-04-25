using System.Collections.Generic;
using System.Threading.Tasks;
using ARMenu.Scripts.Runtime.Data;
using ARMenu.Scripts.Runtime.Data.ImageLibrary.Dishes;
using ARMenu.Scripts.Runtime.Infrastructure;
using ARMenu.Scripts.Runtime.Services.AssetProvider;
using ARMenu.Scripts.Runtime.Services.ImageTracker;
using ARMenu.Scripts.Runtime.Services.ScreenService;
using ARMenu.Scripts.Runtime.UI.DishDescription;
using ARMenu.Scripts.Runtime.UI.ScanHint;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace ARMenu.Scripts.Runtime.Logic
{
	public class DishCreator
	{
		private readonly IAssetProvider _assetProvider;
		private readonly IImageTracker _imageTracker;
		private readonly IScreenService _screenService;
		private readonly DishDescriptionViewModel _dishDescriptionModel;
		private readonly HintViewModel _hintScreenModel;

        private readonly Dictionary<XRReferenceImage, TaskCompletionSource<Dish>> _completionSources = new();
        private readonly Dictionary<XRReferenceImage, GameObject> _spawnedDishes = new();
		private DishImageLibrary _imageLibrary;
		private ARTrackedImage _currentImage;

		public DishCreator(AppContext appContext)
		{
			_assetProvider = appContext.Resolve<IAssetProvider>();
			_imageTracker = appContext.Resolve<IImageTracker>();
			_screenService = appContext.Resolve<IScreenService>();
			_dishDescriptionModel = appContext.Resolve<DishDescriptionViewModel>();
			_hintScreenModel = appContext.Resolve<HintViewModel>();
		}

		public void Initialize(DishImageLibrary imageLibrary)
		{
			_imageLibrary = imageLibrary;

			ShowHintScreen("Please, scan a QR code.");

			_imageTracker.Added += OnImageTrackerAdded;
			_imageTracker.Removed += OnImageTrackerRemoved;
			_imageTracker.ActiveTrackingChanged += OnActiveTrackingChanged;
			_imageTracker.AnyTrackingLost += OnAnyTrackingLost;
		}

		public void CleanUp()
		{
			_imageTracker.Added -= OnImageTrackerAdded;
			_imageTracker.Removed -= OnImageTrackerRemoved;
			_imageTracker.ActiveTrackingChanged -= OnActiveTrackingChanged;
			_imageTracker.AnyTrackingLost -= OnAnyTrackingLost;

			_currentImage = null;
			foreach (GameObject spawnedDish in _spawnedDishes.Values)
			{
				Object.Destroy(spawnedDish);
			}
			_spawnedDishes.Clear();
		}

		private async void OnImageTrackerAdded(ARTrackedImage trackedImage)
		{
			XRReferenceImage referenceImage = trackedImage.referenceImage;
			if (_imageLibrary.TryGetLinkedData(referenceImage, out AssetReferenceDish assetReference) == false)
			{
				Debug.LogError($"Can't find required dish by reference image {referenceImage.name}.");
				return;
			}

			// Check if data loading by required reference image was already started.
			if (_completionSources.ContainsKey(referenceImage))
			{
				return;
			}

			// Load linked data by reference image.
			TaskCompletionSource<Dish> taskCompletionSource = new();
			_completionSources.Add(referenceImage, taskCompletionSource);

			Dish loadedAsset = await _assetProvider.LoadAssetAsync<Dish>(assetReference);
			taskCompletionSource.SetResult(loadedAsset);
		}

		private void OnImageTrackerRemoved(ARTrackedImage trackedImage)
		{
			XRReferenceImage referenceImage = trackedImage.referenceImage;
			if (_imageLibrary.TryGetLinkedData(referenceImage, out AssetReferenceDish assetReference) == false)
			{
				Debug.LogError($"Can't find required dish by reference image {referenceImage.name}.");
				return;
			}
			_assetProvider.ReleaseAsset(assetReference);
		}

		private async void OnActiveTrackingChanged(TrackingChangedEventArgs eventArgs)
		{
			_currentImage = eventArgs.Current;
			if (_completionSources.TryGetValue(_currentImage.referenceImage, out TaskCompletionSource<Dish> completionSource) == false)
			{
				Debug.LogError($"Can't find completion source for {_currentImage.referenceImage.name}. It can mean that Added event wasn't handled.");
				return;
			}

			if (completionSource.Task.IsCompleted == false)
			{
				ShowHintScreen("Dish data is loading.");
				await completionSource.Task;
			}

			// Additional check to match the current image after await of async operation.
			if (_currentImage != eventArgs.Current)
			{
				return;
			}

			Dish dish = completionSource.Task.Result;
			foreach (GameObject spawnedDish in _spawnedDishes.Values)
			{
				spawnedDish.SetActive(false);
			}

			if (_spawnedDishes.TryGetValue(_currentImage.referenceImage, out GameObject currentDish) == false)
			{
				currentDish = Object.Instantiate(dish.prefab, _currentImage.transform);
				_spawnedDishes[_currentImage.referenceImage] = currentDish;
			}

			currentDish.SetActive(true);
			ShowDishScreen(dish);
		}

		private void OnAnyTrackingLost()
		{
			foreach (GameObject spawnedDish in _spawnedDishes.Values)
			{
				spawnedDish.SetActive(false);
			}
			_currentImage = null;
			ShowHintScreen("Please, scan a QR code.");
		}

		private void ShowHintScreen(string hint)
		{
			_screenService.Show<HintUxmlScreen>();
			_hintScreenModel.SetHint(hint);
		}

		private void ShowDishScreen(Dish dish)
		{
			_screenService.Show<DishDescriptionUxmlScreen>();
			_dishDescriptionModel.SetDish(dish);
		}
	}
}