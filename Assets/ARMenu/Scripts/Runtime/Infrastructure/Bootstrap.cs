using ARMenu.Scripts.Runtime.Data;
using ARMenu.Scripts.Runtime.Infrastructure.Constants;
using ARMenu.Scripts.Runtime.Services.AssetProvider;
using ARMenu.Scripts.Runtime.Services.DishTracker;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ARMenu.Scripts.Runtime.Infrastructure
{
	[DefaultExecutionOrder(-1000)]
	public class Bootstrap : MonoBehaviour
	{
		public ARTrackedImageManager trackedImageManagerPrefab;
		public ARSession arSessionPrefab;

		private IDishTrackerAR _dishTracker;
		private IAssetProvider _assetProvider;

		private void Awake()
		{
			InitializeServices();
		}

		private void Start()
		{
			StartDishTrackerAsync(AssetKeys.BurgersLibraryKey);
		}

		private void OnDestroy()
		{
			_dishTracker.CleanUp();
		}

		private void InitializeServices()
		{
			ARTrackedImageManager imageManager = Instantiate(trackedImageManagerPrefab);
			Instantiate(arSessionPrefab);

			_assetProvider = new AssetProvider();
			_assetProvider.Initialize();
			_dishTracker = new DishTrackerAR(_assetProvider, imageManager);
		}

		private async void StartDishTrackerAsync(string libraryAssetKey)
		{
			DishImageLibrary imageLibrary = await _assetProvider.LoadAssetAsync<DishImageLibrary>(libraryAssetKey);
			_dishTracker.SetImageLibrary(imageLibrary);
			_dishTracker.Start();
		}
	}
}