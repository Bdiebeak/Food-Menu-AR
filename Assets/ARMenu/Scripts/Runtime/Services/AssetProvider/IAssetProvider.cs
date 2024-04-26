using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace ARMenu.Scripts.Runtime.Services.AssetProvider
{
	public interface IAssetProvider
	{
		public Task InitializeAsync();
		public void CleanUp();
		public Task<IList<TAsset>> LoadAssetsByLabel<TAsset>(IList<string> label) where TAsset : class;
		public Task<TAsset> LoadAssetAsync<TAsset>(string assetKey) where TAsset : class;
		public Task<TAsset> LoadAssetAsync<TAsset>(AssetReference assetReference) where TAsset : class;
		public void ReleaseAsset(string assetKey);
		public void ReleaseAsset(AssetReference assetReference);
	}
}