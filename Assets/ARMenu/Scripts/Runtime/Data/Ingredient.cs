using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ARMenu.Scripts.Runtime.Data
{
	[CreateAssetMenu(fileName = nameof(Ingredient), menuName = "ScriptableObjects/" + nameof(Ingredient))]
	public class Ingredient : ScriptableObject
	{
		public string title;
		public string description;
		[Space]
		public AssetReferenceSprite image;
	}
}