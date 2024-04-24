using System.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace ARMenu.Scripts.Runtime.Services.AssetProvider
{
	public interface IAssetProvider
	{
		public Task InitializeAsync();
		public void CleanUp();
		public Task<T> LoadAssetAsync<T>(string assetKey) where T : class;
		public Task<T> LoadAssetAsync<T>(AssetReference assetReference) where T : class;
		public void ReleaseAsset(string assetKey);
		public void ReleaseAsset(AssetReference assetReference);
	}
}