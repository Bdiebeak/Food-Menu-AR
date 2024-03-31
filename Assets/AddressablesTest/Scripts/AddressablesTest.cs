using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace AddressablesTest.Scripts
{
	public class AddressablesTest : MonoBehaviour
	{
		private const string BurgerTag = "Burgers";
		private const string BurgerPath = "Burgers/First/BurgerFirst.txt";

		[ContextMenu("Initialize")]
		public async void Initialize()
		{
			Task<IResourceLocator> initializeTask = Addressables.InitializeAsync().Task;
			await initializeTask;
			if (initializeTask.IsCompleted)
			{
				Debug.Log($"Initialization result {initializeTask.IsCompletedSuccessfully}.");
			}
		}

		[ContextMenu("Few Assets")]
		public async void LoadAssets()
		{
			await LoadAssetsAsync<TextAsset>(BurgerTag, Debug.Log);
		}

		[ContextMenu("1 Asset")]
		public async void LoadAsset()
		{
			TextAsset textAsset = await LoadAssetAsync<TextAsset>(BurgerPath);
			Debug.Log(textAsset.text);
		}

		private async Task<T> LoadAssetAsync<T>(string key)
		{
			AsyncOperationHandle<T> loadOperation = Addressables.LoadAssetAsync<T>(key);
			await loadOperation.Task;
			if (loadOperation.Status == AsyncOperationStatus.Succeeded)
			{
				return loadOperation.Result;
			}

			Debug.LogError($"Something wrong with {key} Addressable loading.");
			return default;
		}

		private async Task<IList<T>> LoadAssetsAsync<T>(string label, Action<T> callback)
		{
			AsyncOperationHandle<IList<T>> loadOperation = Addressables.LoadAssetsAsync<T>(label, callback);
			await loadOperation.Task;
			if (loadOperation.Status == AsyncOperationStatus.Succeeded)
			{
				return loadOperation.Result;
			}

			Debug.LogError($"Something wrong with {label} Addressables loading.");
			return default;
		}
	}
}