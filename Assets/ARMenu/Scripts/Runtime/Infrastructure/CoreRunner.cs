using ARMenu.Scripts.Runtime.Data;
using ARMenu.Scripts.Runtime.Data.ImageLibrary;
using ARMenu.Scripts.Runtime.Infrastructure.Constants;
using ARMenu.Scripts.Runtime.Services.AssetProvider;
using ARMenu.Scripts.Runtime.Services.DishTracker;
using ARMenu.Scripts.Runtime.Services.ScreenService;
using ARMenu.Scripts.Runtime.UI.DishDescription;
using ARMenu.Scripts.Runtime.UI.ScanHint;
using UnityEngine;

namespace ARMenu.Scripts.Runtime.Infrastructure
{
	public class CoreRunner : MonoBehaviour
	{
		private IAssetProvider _assetProvider;
		private IDishTracker _dishTracker;
		private IScreenService _screenService;
		private DishDescriptionViewModel _dishDescriptionModel;
		private HintViewModel _hintScreenModel;

		public void Initialize(IAssetProvider assetProvider,
							   IDishTracker dishTracker,
							   IScreenService screenService,
							   DishDescriptionViewModel dishDescriptionModel, HintViewModel hintScreenModel)
		{
			_assetProvider = assetProvider;
			_dishTracker = dishTracker;
			_screenService = screenService;
			_dishDescriptionModel = dishDescriptionModel;
			_hintScreenModel = hintScreenModel;
		}

		private async void Start()
		{
			_dishTracker.TrackingChanged += OnTrackingChanged;
			_dishTracker.TrackingLost += OnTrackingLost;
			DishImageLibrary imageLibrary = await _assetProvider.LoadAssetAsync<DishImageLibrary>(AssetKeys.BurgersLibraryKey);
			_dishTracker.Initialize(imageLibrary);
			_hintScreenModel.SetHint("Please, scan a QR code.");
			_screenService.HideAll();
			_screenService.GetScreen<HintUxmlScreen>().Show();
		}

		private void OnDestroy()
		{
			_dishTracker.TrackingChanged -= OnTrackingChanged;
			_dishTracker.TrackingLost -= OnTrackingLost;
			_dishTracker.CleanUp();
			_screenService.HideAll();
		}

		private void OnTrackingChanged(Dish newDish)
		{
			_dishDescriptionModel.SetDish(newDish);
			_screenService.GetScreen<HintUxmlScreen>().Hide();
			DishDescriptionUxmlScreen dishDescriptionUxmlScreen = _screenService.GetScreen<DishDescriptionUxmlScreen>();
			dishDescriptionUxmlScreen.Show();
		}

		private void OnTrackingLost()
		{
			_screenService.GetScreen<DishDescriptionUxmlScreen>().Hide();
			_hintScreenModel.SetHint("Please, scan a QR code.");
			_screenService.GetScreen<HintUxmlScreen>().Show();
		}
	}
}