using System.Threading.Tasks;

namespace ARMenu.Scripts.Runtime.Services.AssetProvider
{
	public interface IAssetProvider
	{
		public void Initialize();
		public Task<T> LoadAssetAsync<T>(string assetKey) where T : class;
	}
}