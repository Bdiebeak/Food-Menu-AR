using System.Collections.Generic;
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
	/// <summary>
	/// Bootstrap is used to create and register services and other required classes.
	/// CoreRunner contains initialization and general logic of the application.
	/// </summary>
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
			await InitializeMenu();
		}

		/// <summary>
		/// This function can be called to load image library by asset key and initialize logic classes.
		/// But it can be called only once without reloading the scene, because <see cref="UnityEngine.XR.ARFoundation.ARTrackedImageManager"/>
		/// doesn't support reinitialization.
		///
		/// Right now, all menu data paths are hard coded to use the Burger menu.
		/// If you want to add some logic, such as scanning a QR code to get a restaurant key and download general assets,
		/// I recommend to scan QR code on different scene and initialize AR Foundation in another.
		/// Each time, when you want to check new menu, you should go back to QR code scene and start over.
		/// </summary>
		private async Task InitializeMenu()
		{
			_screenService.Show<HintUxmlScreen>();
			_hintScreenModel.SetHint("Loading required data...");

			// TODO: check boxing?
			var generalAssets = await _assetProvider.LoadAssetsByLabel<object>(new List<string> { AssetLabels.BurgersLabel, AssetLabels.GeneralLabel });
			foreach (object asset in generalAssets)
			{
				if (asset is DishImageLibrary library)
				{
					_imageLibrary = library;
					break; // Exit from foreach when library is found.
				}
			}

			if (_imageLibrary == null)
			{
				Debug.LogError("Can't find Dish Image Library in general assets of menu.");
				return;
			}

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