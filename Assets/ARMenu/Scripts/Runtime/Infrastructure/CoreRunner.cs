using ARMenu.Scripts.Runtime.Data.ImageLibrary.Dishes;
using ARMenu.Scripts.Runtime.Infrastructure.Constants;
using ARMenu.Scripts.Runtime.Logic;
using ARMenu.Scripts.Runtime.Services.AssetProvider;
using ARMenu.Scripts.Runtime.Services.ImageTracker;
using ARMenu.Scripts.Runtime.Services.ScreenService;
using ARMenu.Scripts.Runtime.UI.DishDescription;
using ARMenu.Scripts.Runtime.UI.ScanHint;
using UnityEngine;

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
		private DishCreator _dishCreator;

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
			_screenService.Show<HintUxmlScreen>();
			_hintScreenModel.SetHint("Loading required data...");

			await _assetProvider.InitializeAsync();
			_imageLibrary = await _assetProvider.LoadAssetAsync<DishImageLibrary>(AssetKeys.BurgersLibraryKey);

			_imageTracker.Initialize(_imageLibrary.XRReferenceImageLibrary);
			_dishCreator = new DishCreator(_assetProvider, _imageTracker, _screenService,
										   _imageLibrary, _dishDescriptionModel, _hintScreenModel);
			_dishCreator.Initialize();
		}

		private void OnDestroy()
		{
			_assetProvider.CleanUp();
			_dishCreator.CleanUp();
			_screenService.HideAll();
		}
	}
}