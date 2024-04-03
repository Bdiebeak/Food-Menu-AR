﻿using System.Collections.Generic;
using UnityEngine;

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
		// TODO: auto bake paths
		[Space]
		public string prefabPath;
		public string imagePath;
	}
}