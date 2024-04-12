using ARMenu.Scripts.Runtime.Data;
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
	public class Bootstrap : MonoBehaviour
	{
		// TODO: CoreFactory?
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
			_screenService.RegisterScreen(new DishDescriptionScreen(_coreUI));
			_screenService.RegisterScreen(new ScanHintScreen(_coreUI));
		}

		private async Awaitable PreloadAssets()
		{
			await _assetProvider.LoadAssetAsync<DishImageLibrary>(AssetKeys.BurgersLibraryKey);
		}

		private void InitializeCore()
		{
			CoreRunner coreRunner = Instantiate(coreRunnerPrefab);
			coreRunner.Initialize(_assetProvider, _dishTracker, _screenService);
		}
	}
}