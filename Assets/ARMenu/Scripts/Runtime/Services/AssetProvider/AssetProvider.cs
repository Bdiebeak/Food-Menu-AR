using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ARMenu.Scripts.Runtime.Services.AssetProvider
{
	public class AssetProvider : IAssetProvider
	{
		private readonly Dictionary<string, AsyncOperationHandle> _completedOperations = new();

		public void Initialize()
		{
			Addressables.InitializeAsync();
		}

		public async Task<T> LoadAssetAsync<T>(string assetKey) where T : class
		{
			if (_completedOperations.TryGetValue(assetKey, out AsyncOperationHandle completeHandle))
			{
				return completeHandle.Result as T;
			}
			return await LoadAndCache<T>(assetKey);
		}

		public async Task<T> LoadAssetAsync<T>(AssetReference assetReference) where T : class
		{
			if (_completedOperations.TryGetValue(assetReference.AssetGUID, out AsyncOperationHandle completeHandle))
			{
				return completeHandle.Result as T;
			}
			return await LoadAndCache<T>(assetReference);
		}

		private async Task<T> LoadAndCache<T>(string assetKey) where T : class
		{
			await Task.Delay(2500); // TODO: remove it, testing only.

			AsyncOperationHandle<T> loadingOperation = Addressables.LoadAssetAsync<T>(assetKey);
			await loadingOperation.Task;
			_completedOperations.Add(assetKey, loadingOperation);

			return loadingOperation.Result;
		}

		private async Task<T> LoadAndCache<T>(AssetReference assetReference) where T : class
		{
			await Task.Delay(2500); // TODO: remove it, testing only.

			AsyncOperationHandle<T> loadingOperation = Addressables.LoadAssetAsync<T>(assetReference);
			await loadingOperation.Task;
			_completedOperations.Add(assetReference.AssetGUID, loadingOperation);

			return loadingOperation.Result;
		}
	}
}