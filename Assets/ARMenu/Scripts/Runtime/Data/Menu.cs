using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARMenu.Scripts.Runtime.Data
{
	[Serializable]
	public class DishToImageNode
	{
		public Texture2D arImage;
		public string imageGuid;
		public Dish dish;
	}

	[CreateAssetMenu(fileName = nameof(Menu), menuName = "ScriptableObjects/" + nameof(Menu))]
	public class Menu : ScriptableObject, IEnumerable<DishToImageNode>
	{
		public List<DishToImageNode> dishes;

		public IEnumerator<DishToImageNode> GetEnumerator()
		{
			return dishes.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}