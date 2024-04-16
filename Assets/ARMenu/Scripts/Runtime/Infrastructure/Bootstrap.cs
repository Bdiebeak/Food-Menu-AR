using ARMenu.Scripts.Runtime.Data.ImageLibrary;
using ARMenu.Scripts.Runtime.Infrastructure.Constants;
using ARMenu.Scripts.Runtime.Services.AssetProvider;
using ARMenu.Scripts.Runtime.Services.DishTracker;
using ARMenu.Scripts.Runtime.Services.ScreenService;
using ARMenu.Scripts.Runtime.UI.DishDescription;
using ARMenu.Scripts.Runtime.UI.ScanHint;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;

namespace ARMenu.Scripts.Runtime.Infrastructure
{
	/// <summary>
	/// Isn't the best bootstrap logic.
	/// But this is enough for me and this example project, I don't want to complicate it.
	/// There could be some additional factories e.g. UIFactory or CoreFactory.
	/// </summary>
	public class Bootstrap : MonoBehaviour
	{
		public ARTrackedImageManager trackedImageManagerPrefab;
		public ARSession arSessionPrefab;
		public CoreRunner coreRunnerPrefab;
		public UIDocument coreUIPrefab;

		private ARTrackedImageManager _imageManager;
		private ARSession _arSession;
		private UIDocument _coreUI;

		private IAssetProvider _assetProvider;
		private IDishTracker _dishTracker;
		private IScreenService _screenService;

		private DishDescriptionViewModel _dishScreenModel;
		private HintViewModel _hintScreenModel;

		private void Awake()
		{
			Initialize();
		}

		private async void Initialize()
		{
			InstantiatePrefabs();
			InitializeServices();
			InitializeScreens();
			await PreloadAssets();
			InitializeCore();
		}

		private void InstantiatePrefabs()
		{
			_imageManager = Instantiate(trackedImageManagerPrefab);
			_arSession = Instantiate(arSessionPrefab);
			_coreUI = Instantiate(coreUIPrefab);
		}

		private void InitializeServices()
		{
			_assetProvider = new AssetProvider();
			_assetProvider.Initialize();
			_dishTracker = new DishTracker(_assetProvider, _imageManager);
			_screenService = new ScreenService();
		}

		private void InitializeScreens()
		{
			_dishScreenModel = new DishDescriptionViewModel();
			_hintScreenModel = new HintViewModel();

			_screenService.RegisterScreen(new DishDescriptionUxmlScreen(_coreUI, _dishScreenModel));
			_screenService.RegisterScreen(new HintUxmlScreen(_coreUI, _hintScreenModel));

			_screenService.HideAll();
			_screenService.GetScreen<HintUxmlScreen>().Show();
			_hintScreenModel.SetHint("Loading...");
		}

		private async Awaitable PreloadAssets()
		{
			await _assetProvider.LoadAssetAsync<DishImageLibrary>(AssetKeys.BurgersLibraryKey);
		}

		private void InitializeCore()
		{
			CoreRunner coreRunner = Instantiate(coreRunnerPrefab);
			coreRunner.Initialize(_assetProvider, _dishTracker, _screenService, _dishScreenModel, _hintScreenModel);
		}
	}
}