using ARMenu.Scripts.Runtime.Services.AssetProvider;
using ARMenu.Scripts.Runtime.Services.ImageTracker;
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
	///
	/// Bootstrap only do registration of services.
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
		private CoreRunner _coreRunner;

		private IAssetProvider _assetProvider;
		private IImageTracker _imageTracker;
		private IScreenService _screenService;

		private DishDescriptionViewModel _dishScreenModel;
		private HintViewModel _hintScreenModel;

		private void Awake()
		{
			Initialize();
		}

		private void Initialize()
		{
			InstantiatePrefabs();
			InitializeServices();
			InitializeScreens();
			InitializeCore();
		}

		private void InstantiatePrefabs()
		{
			_imageManager = Instantiate(trackedImageManagerPrefab);
			_arSession = Instantiate(arSessionPrefab);
			_coreUI = Instantiate(coreUIPrefab);
			_coreRunner = Instantiate(coreRunnerPrefab);
		}

		private void InitializeServices()
		{
			_assetProvider = new AssetProvider();
			_imageTracker = new SingleImageTracker(_imageManager);
			_screenService = new ScreenService();
		}

		private void InitializeScreens()
		{
			_dishScreenModel = new DishDescriptionViewModel();
			_hintScreenModel = new HintViewModel();
			_screenService.RegisterScreen(new DishDescriptionUxmlScreen(_coreUI, _dishScreenModel));
			_screenService.RegisterScreen(new HintUxmlScreen(_coreUI, _hintScreenModel));
		}

		private void InitializeCore()
		{
			_coreRunner.Initialize(_assetProvider, _imageTracker, _screenService, _dishScreenModel, _hintScreenModel);
		}
	}
}