﻿using System;
using System.Text;
using ARMenu.Scripts.Runtime.Data;
using ARMenu.Scripts.Runtime.UI.General.Mvvm;
using Unity.Properties;
using UnityEngine;

namespace ARMenu.Scripts.Runtime.UI.DishDescription
{
	/// <summary>
	/// Своего рода ViewModel из MVVM.
	/// Хранит данные, необходимые только View (состояние UI), а также логику обработки команд со стороны View
	/// для изменения Model.
	/// Так, например, ViewModel будет дублировать только те поля модели, которые нужно отобразить на View.
	/// А также включать в себя обработчики логики, которые вызываются при взаимодействии с UI.
	/// </summary>
	public class DishDescriptionViewModel : ViewModel
	{
		[CreateProperty(ReadOnly = true)] public string DishName { get; private set; }
		[CreateProperty(ReadOnly = true)] public string DishDescription { get; private set; }
		[CreateProperty(ReadOnly = true)] public string IngredientName { get; private set; }
		[CreateProperty(ReadOnly = true)] public string IngredientDescription { get; private set; }
		// TODO: bind images.
		public Texture2D DishImage { get; private set; }
		public Texture2D IngredientImage { get; private set; }

		public event Action<bool> CollapseStateChanged;

		/// <summary>
		/// Своего рода Model из MVVM.
		/// Данные приложения, которые не относятся к состоянию UI.
		/// </summary>
		private Dish _dish;
		private int _ingredientIndex;
		private bool _isCollapsed;

		public void SetDish(Dish newDish)
		{
			_dish = newDish;
			_ingredientIndex = 0;
			DishName = newDish.title;
			DishImage = newDish.previewImage;
			DishDescription = GenerateDescription();
			RefillIngredient();
			CallChangedEvent();
		}

		public override void Initialize()
		{
			CallChangedEvent();
		}

		public override void Clear()
		{
			Collapse();
			DishName = string.Empty;
			DishDescription = string.Empty;
			IngredientName = string.Empty;
			IngredientDescription = string.Empty;
			DishImage = null;
			IngredientImage = null;
		}

		public void NextIngredient()
		{
			_ingredientIndex++;
			if (_ingredientIndex >= _dish.ingredients.Count)
			{
				_ingredientIndex = 0;
			}
			RefillIngredient();
		}

		public void PreviousIngredient()
		{
			_ingredientIndex--;
			if (_ingredientIndex < 0)
			{
				_ingredientIndex = _dish.ingredients.Count - 1;
			}
			RefillIngredient();
		}

		public void SwitchCollapsedState()
		{
			_isCollapsed = !_isCollapsed;
			if (_isCollapsed)
			{
				Collapse();
			}
			else
			{
				Expand();
			}
		}

		public void Expand()
		{
			_isCollapsed = false;
			CollapseStateChanged?.Invoke(false);
		}

		public void Collapse()
		{
			_isCollapsed = true;
			CollapseStateChanged?.Invoke(true);
		}

		private string GenerateDescription()
		{
			StringBuilder descriptionBuilder = new();
			descriptionBuilder.AppendLine(_dish.description);
			descriptionBuilder.AppendLine(_dish.nutritionalInfo.ToString());
			descriptionBuilder.AppendLine($"Weight: {_dish.weight} | Cost: {_dish.cost}");
			return descriptionBuilder.ToString();
		}

		private void RefillIngredient()
		{
			Ingredient currentIngredient = _dish.ingredients[_ingredientIndex];
			IngredientName = currentIngredient.title;
			IngredientDescription = currentIngredient.description;
			IngredientImage = currentIngredient.previewImage;
			CallChangedEvent();
		}
	}
}