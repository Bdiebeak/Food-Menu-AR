using ARMenu.Scripts.Runtime.Data;
using ARMenu.Scripts.Runtime.Infrastructure.Constants;
using ARMenu.Scripts.Runtime.Services.AssetProvider;
using ARMenu.Scripts.Runtime.Services.DishTracker;
using ARMenu.Scripts.Runtime.Services.ScreenService;
using ARMenu.Scripts.Runtime.UI.DishDescription;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;

namespace ARMenu.Scripts.Runtime.Infrastructure
{
	public class Bootstrap : MonoBehaviour
	{
		// TODO: GameFactory?
		public ARTrackedImageManager trackedImageManagerPrefab;
		public ARSession arSessionPrefab;
		public CoreRunner coreRunnerPrefab;
		public UIDocument documentPrefab;

		private ARTrackedImageManager _imageManager;
		private ARSession _arSession;
		private UIDocument _uiDocument;

		private IDishTrackerAR _dishTracker;
		private IAssetProvider _assetProvider;
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
			await InitializeAssets();
			InitializeCore();
		}

		private void InstantiatePrefabs()
		{
			_imageManager = Instantiate(trackedImageManagerPrefab);
			_arSession = Instantiate(arSessionPrefab);
			_uiDocument = Instantiate(documentPrefab);
		}

		private void InitializeServices()
		{
			_assetProvider = new AssetProvider();
			_assetProvider.Initialize();
			_dishTracker = new DishTrackerAR(_assetProvider, _imageManager);
			_screenService = new ScreenService();
		}

		private void InitializeScreens()
		{
			_screenService.RegisterScreen(new DishDescriptionScreen(_uiDocument));
		}

		private async Awaitable InitializeAssets()
		{
			DishImageLibrary imageLibrary = await _assetProvider.LoadAssetAsync<DishImageLibrary>(AssetKeys.BurgersLibraryKey);
			_dishTracker.SetImageLibrary(imageLibrary);
		}

		private void InitializeCore()
		{
			CoreRunner coreRunner = Instantiate(coreRunnerPrefab);
			coreRunner.Construct(_dishTracker, _screenService);
		}
	}
}