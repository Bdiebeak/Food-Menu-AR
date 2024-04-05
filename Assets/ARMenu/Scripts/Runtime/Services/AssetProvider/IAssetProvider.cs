using UnityEngine;

namespace ARMenu.Scripts.Runtime.Services.AssetProvider
{
	public interface IAssetProvider
	{
		public void Initialize();
		public Awaitable<T> LoadAssetAsync<T>(string assetKey) where T : class;
	}
}