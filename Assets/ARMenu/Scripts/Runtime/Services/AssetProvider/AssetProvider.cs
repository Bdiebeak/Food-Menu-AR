using System.Collections.Generic;
using UnityEngine;
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

		public async Awaitable<T> LoadAssetAsync<T>(string assetKey) where T : class
		{
			if (_completedOperations.TryGetValue(assetKey, out AsyncOperationHandle completeHandle))
			{
				return completeHandle.Result as T;
			}
			return await LoadAndCache<T>(assetKey);
		}

		private async Awaitable<T> LoadAndCache<T>(string assetKey) where T : class
		{
			AsyncOperationHandle<T> loadingOperation = Addressables.LoadAssetAsync<T>(assetKey);

			await loadingOperation.Task;
			_completedOperations.Add(assetKey, loadingOperation);

			return loadingOperation.Result;
		}
	}
}