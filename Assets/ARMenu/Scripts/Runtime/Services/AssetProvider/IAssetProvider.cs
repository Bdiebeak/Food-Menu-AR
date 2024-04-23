using System.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace ARMenu.Scripts.Runtime.Services.AssetProvider
{
	public interface IAssetProvider
	{
		public void Initialize();
		public Task<T> LoadAssetAsync<T>(string assetKey) where T : class;
		public Task<T> LoadAssetAsync<T>(AssetReference assetReference) where T : class;
	}
}