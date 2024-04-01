using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ARMenu.Scripts.Runtime.Data
{
	[CreateAssetMenu(fileName = nameof(Dish), menuName = "ScriptableObjects/" + nameof(Dish))]
	public class Dish : ScriptableObject
	{
		public string title;
		public string description;
		public float weight;
		public NutritionalInfo nutritionalInfo;
		public List<Ingredient> ingredients;
		[Space]
		public AssetReferenceGameObject prefab;
		public AssetReferenceSprite image;
	}
}