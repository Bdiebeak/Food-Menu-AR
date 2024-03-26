using UnityEngine;

namespace ARMenu.Runtime.Data
{
	[CreateAssetMenu(fileName = nameof(Ingredient), menuName = "ScriptableObjects/" + nameof(Ingredient))]
	public class Ingredient : ScriptableObject
	{
		public Sprite image;
		public string title;
	}
}