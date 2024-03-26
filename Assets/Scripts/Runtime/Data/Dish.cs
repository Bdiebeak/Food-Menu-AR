using UnityEngine;

namespace ARMenu.Runtime.Data
{
	[CreateAssetMenu(fileName = nameof(Dish), menuName = "ScriptableObjects/" + nameof(Dish))]
	public class Dish : ScriptableObject
	{
		public Sprite image;
		public string title;
		public string description;
		public float calories;
		public float weight;
		public Macronutrients macronutrients;
	}
}