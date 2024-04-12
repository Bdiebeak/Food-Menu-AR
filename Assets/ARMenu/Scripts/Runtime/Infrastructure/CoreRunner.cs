using ARMenu.Scripts.Runtime.Data;
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

		public void Initialize(IAssetProvider assetProvider,
							   IDishTracker dishTracker,
							   IScreenService screenService)
		{
			_assetProvider = assetProvider;
			_dishTracker = dishTracker;
			_screenService = screenService;
		}

		private async void Start()
		{
			_dishTracker.TrackingChanged += OnTrackingChanged;
			_dishTracker.TrackingLost += OnTrackingLost;
			DishImageLibrary imageLibrary = await _assetProvider.LoadAssetAsync<DishImageLibrary>(AssetKeys.BurgersLibraryKey);
			_dishTracker.Initialize(imageLibrary);
			_screenService.HideAll();
			_screenService.GetScreen<ScanHintScreen>().Show();
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
			// TODO: если нет отслеживаемого блюда, то показать экран "Scan QR code".
			// TODO: если есть блюда, показывать экран с его информацией.
			_screenService.GetScreen<ScanHintScreen>().Hide();
			DishDescriptionScreen dishDescriptionScreen = _screenService.GetScreen<DishDescriptionScreen>();
			dishDescriptionScreen.Show();
			dishDescriptionScreen.SetDish(newDish);
		}

		private void OnTrackingLost()
		{
			_screenService.GetScreen<DishDescriptionScreen>().Hide();
			_screenService.GetScreen<ScanHintScreen>().Show();
		}
	}
}