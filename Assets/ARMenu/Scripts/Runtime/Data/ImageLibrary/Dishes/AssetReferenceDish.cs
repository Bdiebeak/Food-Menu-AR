using System;
using UnityEngine.AddressableAssets;

namespace ARMenu.Scripts.Runtime.Data.ImageLibrary.Dishes
{
	[Serializable]
	public class AssetReferenceDish : AssetReferenceT<Dish>
	{
		public AssetReferenceDish(string guid) : base(guid) { }
	}
}