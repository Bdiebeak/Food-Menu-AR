using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ARMenu.Scripts.Runtime.Services.AssetProvider
{
	public interface IAssetProvider
	{
		public void Initialize();
		public Awaitable<T> LoadAssetAsync<T>(string assetKey) where T : class;
		public Awaitable<T> LoadAssetAsync<T>(AssetReference assetReference) where T : class;
	}
}