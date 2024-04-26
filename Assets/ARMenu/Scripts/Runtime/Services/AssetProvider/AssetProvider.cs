using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace ARMenu.Scripts.Runtime.Services.AssetProvider
{
	public class AssetProvider : IAssetProvider
	{
		private readonly Dictionary<string, AsyncOperationHandle> _completedHandles = new();
		private readonly Dictionary<string, AsyncOperationHandle> _processingHandles = new();

		public async Task InitializeAsync()
		{
			await Addressables.InitializeAsync().Task;
		}

		public void CleanUp()
		{
			foreach (AsyncOperationHandle handle in _completedHandles.Values)
			{
				ReleaseByHandle(handle);
			}
			_completedHandles.Clear();

			foreach (AsyncOperationHandle handle in _processingHandles.Values)
			{
				ReleaseByHandle(handle);
			}
			_processingHandles.Clear();
		}

		public async Task<IList<TAsset>> LoadAssetsByLabel<TAsset>(IList<string> labels) where TAsset : class
		{
			var locationsHandle = Addressables.LoadResourceLocationsAsync(labels, Addressables.MergeMode.Intersection);
			var locations = await locationsHandle.Task;

			List<TAsset> loadedAssets = new();
			foreach (IResourceLocation location in locations)
			{
				TAsset asset = await LoadAssetAsync<TAsset>(location.PrimaryKey);
				loadedAssets.Add(asset);
			}

			Addressables.Release(locationsHandle);
			return loadedAssets;
		}

		public async Task<TAsset> LoadAssetAsync<TAsset>(string assetKey) where TAsset : class
		{
			return await Load<TAsset>(assetKey);
		}

		public async Task<TAsset> LoadAssetAsync<TAsset>(AssetReference assetReference) where TAsset : class
		{
			return await Load<TAsset>(assetReference.AssetGUID);
		}

		public void ReleaseAsset(string assetKey)
		{
			Release(assetKey);
		}

		public void ReleaseAsset(AssetReference assetReference)
		{
			Release(assetReference.AssetGUID);
		}

		private void Release(string assetKey)
		{
			if (_completedHandles.TryGetValue(assetKey, out AsyncOperationHandle completeHandle))
			{
				ReleaseByHandle(completeHandle);
			}
			else if (_processingHandles.TryGetValue(assetKey, out AsyncOperationHandle processingHandle))
			{
				ReleaseByHandle(processingHandle);
			}
		}

		private async Task<TAsset> Load<TAsset>(string assetKey) where TAsset : class
		{
			// If asset with required key was already loaded - return result of operation.
			if (_completedHandles.TryGetValue(assetKey, out AsyncOperationHandle completeHandle))
			{
				return completeHandle.Result as TAsset;
			}

			// If asset with required key is loading right now - return this task.
			if (_processingHandles.TryGetValue(assetKey, out AsyncOperationHandle processingHandle))
			{
				return processingHandle.Task as TAsset;
			}

			AsyncOperationHandle<TAsset> loadingOperation = Addressables.LoadAssetAsync<TAsset>(assetKey);
			_processingHandles.Add(assetKey, loadingOperation);

			await loadingOperation.Task;

			_completedHandles.Add(assetKey, loadingOperation);
			_processingHandles.Remove(assetKey);

			return loadingOperation.Result;
		}

		private void ReleaseByHandle(AsyncOperationHandle operationHandle)
		{
			Addressables.Release(operationHandle);
		}
	}
}