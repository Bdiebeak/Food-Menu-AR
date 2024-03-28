using System;
using System.Collections.Generic;
using UnityEngine;

namespace ARMenu.Scripts.Runtime.Data
{
	[Serializable]
	public class DishToImageNode
	{
		public Texture2D arImage;
		public Dish dish;
	}

	[CreateAssetMenu(fileName = nameof(Menu), menuName = "ScriptableObjects/" + nameof(Menu))]
	public class Menu : ScriptableObject
	{
		public List<DishToImageNode> dishes;
	}
}