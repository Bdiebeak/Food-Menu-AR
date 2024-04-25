using System.Threading.Tasks;
using ARMenu.Scripts.Runtime.Data.ImageLibrary.Dishes;
using ARMenu.Scripts.Runtime.Infrastructure.Constants;
using ARMenu.Scripts.Runtime.Logic;
using ARMenu.Scripts.Runtime.Services.AssetProvider;
using ARMenu.Scripts.Runtime.Services.ImageTracker;
using ARMenu.Scripts.Runtime.Services.ScreenService;
using ARMenu.Scripts.Runtime.UI.ScanHint;
using UnityEngine;

namespace ARMenu.Scripts.Runtime.Infrastructure
{
	public class CoreRunner : MonoBehaviour
	{
		private IAssetProvider _assetProvider;
		private IImageTracker _imageTracker;
		private IScreenService _screenService;
		private HintViewModel _hintScreenModel;

		private DishImageLibrary _imageLibrary;
		private DishCreator _dishCreator;

		public void Construct(AppContext appContext)
		{
			_assetProvider = appContext.Resolve<IAssetProvider>();
			_imageTracker = appContext.Resolve<IImageTracker>();
			_screenService = appContext.Resolve<IScreenService>();
			_hintScreenModel = appContext.Resolve<HintViewModel>();
			_dishCreator = new DishCreator(appContext);
		}

		private async void Start()
		{
			await _assetProvider.InitializeAsync();
			await InitializeMenu(AssetKeys.BurgersLibraryKey);
		}

		private async Task InitializeMenu(string menuAssetKey)
		{
			_screenService.Show<HintUxmlScreen>();
			_hintScreenModel.SetHint("Loading required data...");

			_imageLibrary = await _assetProvider.LoadAssetAsync<DishImageLibrary>(menuAssetKey);

			_imageTracker.Initialize(_imageLibrary.XRReferenceImageLibrary);
			_dishCreator.Initialize(_imageLibrary);
		}

		private void OnDestroy()
		{
			_assetProvider.CleanUp();
			_dishCreator.CleanUp();
			_screenService.HideAll();
		}
	}
}