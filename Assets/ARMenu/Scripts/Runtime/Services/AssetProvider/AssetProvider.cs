using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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
		}

		public async Task<T> LoadAssetAsync<T>(string assetKey) where T : class
		{
			return await Load<T>(assetKey);
		}

		public async Task<T> LoadAssetAsync<T>(AssetReference assetReference) where T : class
		{
			return await Load<T>(assetReference.AssetGUID);
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

		private async Task<T> Load<T>(string assetKey) where T : class
		{
			// If asset with required key was already loaded - return result of operation.
			if (_completedHandles.TryGetValue(assetKey, out AsyncOperationHandle completeHandle))
			{
				return completeHandle.Result as T;
			}

			// If asset with required key is loading right now - return this task.
			if (_processingHandles.TryGetValue(assetKey, out AsyncOperationHandle processingHandle))
			{
				return processingHandle.Task as T;
			}

			AsyncOperationHandle<T> loadingOperation = Addressables.LoadAssetAsync<T>(assetKey);
			_processingHandles.Add(assetKey, loadingOperation);

			await loadingOperation.Task;
			await Awaitable.WaitForSecondsAsync(3); // TODO: remove test only.

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