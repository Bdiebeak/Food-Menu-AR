using ARMenu.Scripts.Runtime.Data;
using ARMenu.Scripts.Runtime.Infrastructure.Constants;
using ARMenu.Scripts.Runtime.Services.AssetProvider;
using ARMenu.Scripts.Runtime.Services.DishTracker;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ARMenu.Scripts.Runtime.Infrastructure
{
	public class Bootstrap : MonoBehaviour
	{
		public ARTrackedImageManager trackedImageManagerPrefab;
		public ARSession arSessionPrefab;
		public CoreRunner coreRunnerPrefab;

		private IDishTrackerAR _dishTracker;
		private IAssetProvider _assetProvider;

		private void Awake()
		{
			Initialize();
		}

		private async void Initialize()
		{
			InitializeServices();
			await InitializeAssets();
			InitializeCore();
		}

		private void InitializeServices()
		{
			ARTrackedImageManager imageManager = Instantiate(trackedImageManagerPrefab);
			Instantiate(arSessionPrefab);

			_assetProvider = new AssetProvider();
			_assetProvider.Initialize();
			_dishTracker = new DishTrackerAR(_assetProvider, imageManager);
		}

		private async Awaitable InitializeAssets()
		{
			DishImageLibrary imageLibrary = await _assetProvider.LoadAssetAsync<DishImageLibrary>(AssetKeys.BurgersLibraryKey);
			_dishTracker.SetImageLibrary(imageLibrary);
		}

		private void InitializeCore()
		{
			CoreRunner coreRunner = Instantiate(coreRunnerPrefab);
			coreRunner.Construct(_dishTracker);
		}
	}
}