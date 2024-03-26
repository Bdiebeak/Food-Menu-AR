﻿using System.Collections.Generic;
using UnityEngine;

namespace ARMenu.Runtime.Data
{
	[CreateAssetMenu(fileName = nameof(Menu), menuName = "ScriptableObjects/" + nameof(Menu))]
	public class Menu : ScriptableObject
	{
		public List<Dish> dishes;
	}
}