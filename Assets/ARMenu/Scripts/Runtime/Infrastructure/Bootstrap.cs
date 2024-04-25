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
	/// There could be some additional factories or etc. And some DI-Container.
	/// </summary>
	public class Bootstrap : MonoBehaviour
	{
		public ARTrackedImageManager trackedImageManagerPrefab;
		public ARSession arSessionPrefab;
		public CoreRunner coreRunnerPrefab;
		public UIDocument coreUIPrefab;

		private AppContext _appContext;

		private void Awake()
		{
			Initialize();
		}

		private void Initialize()
		{
			_appContext = new AppContext();

			InstantiatePrefabs();
			InitializeServices();
			InitializeScreens();
			InitializeCore();
		}

		private void InstantiatePrefabs()
		{
			_appContext.Register(Instantiate(trackedImageManagerPrefab));
			_appContext.Register(Instantiate(arSessionPrefab));
			_appContext.Register(Instantiate(coreUIPrefab));
			_appContext.Register(Instantiate(coreRunnerPrefab));
		}

		private void InitializeServices()
		{
			_appContext.Register<IAssetProvider>(new AssetProvider());
			_appContext.Register<IImageTracker>(new SingleImageTracker(_appContext));
			_appContext.Register<IScreenService>(new ScreenService());
		}

		private void InitializeScreens()
		{
			_appContext.Register(new DishDescriptionViewModel());
			_appContext.Register(new HintViewModel());

			IScreenService screenService = _appContext.Resolve<IScreenService>();
			screenService.RegisterScreen(new DishDescriptionUxmlScreen(_appContext.Resolve<UIDocument>(),
																	   _appContext.Resolve<DishDescriptionViewModel>()));
			screenService.RegisterScreen(new HintUxmlScreen(_appContext.Resolve<UIDocument>(),
															_appContext.Resolve<HintViewModel>()));
		}

		private void InitializeCore()
		{
			_appContext.Resolve<CoreRunner>().Construct(_appContext);
		}
	}
}