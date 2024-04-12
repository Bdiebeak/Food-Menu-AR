using ARMenu.Scripts.Runtime.Data;
using ARMenu.Scripts.Runtime.UI.General;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace ARMenu.Scripts.Runtime.UI.DishDescription
{
	// TODO: MV*-паттерн для работы с экраном
	// Вдруг я захочу сменить UI Toolkit на UGUI интерфейс. В этом вьюшном классе не должно быть бизнес-логики.
	public class DishDescriptionScreen : UxmlBaseScreen
	{
		private readonly DishDescriptionElements _elements;

		// This is business logic
		private bool _isCollapsed = true;
		private Dish _dish;
		private int _ingredientIndex;

		public DishDescriptionScreen(UIDocument document)
		{
			_elements = new DishDescriptionElements(document);
			_elements.Initialize();
			BindData();
		}

		protected override VisualElement GetScreenRoot()
		{
			return _elements.ScreenRoot;
		}

		// TODO: remove temp function
		public void SetDish(Dish newDish)
		{
			_dish = newDish;
			_ingredientIndex = 0;
			RefillDescription();
		}

		private void RefillDescription()
		{
			_elements.DishName.text = _dish.title;
			_elements.DishDescription.text = _dish.description;
			_elements.DishImage.style.backgroundImage = new StyleBackground(Texture2D.whiteTexture);

			_elements.IngredientName.text = _dish.ingredients[_ingredientIndex].title;
			_elements.IngredientDescription.text = _dish.ingredients[_ingredientIndex].description;
			_elements.IngredientImage.style.backgroundImage = new StyleBackground(Texture2D.whiteTexture);
		}

		private void BindData()
		{
			// TODO: fix, doesn't work.
			_elements.DishName.SetBinding(nameof(Label.text), new DataBinding()
			{
				bindingMode = BindingMode.ToTarget,
				dataSource = _dish,
				dataSourcePath = new PropertyPath(nameof(Dish.title)),
				dataSourceType = typeof(string)
			});
		}

		protected override void SubscribeEvents()
		{
			_elements.CollapseButton.RegisterCallback<ClickEvent>(OnCollapseButtonClicked);
			_elements.PreviousButton.RegisterCallback<ClickEvent>(OnPreviousButtonClicked);
			_elements.NextButton.RegisterCallback<ClickEvent>(OnNextButtonClicked);
		}

		protected override void UnsubscribeEvents()
		{
			_elements.CollapseButton.UnregisterCallback<ClickEvent>(OnCollapseButtonClicked);
			_elements.PreviousButton.UnregisterCallback<ClickEvent>(OnPreviousButtonClicked);
			_elements.NextButton.UnregisterCallback<ClickEvent>(OnNextButtonClicked);
		}

		private void OnCollapseButtonClicked(ClickEvent evt)
		{
			SetCollapsedState(!_isCollapsed);
		}

		private void OnPreviousButtonClicked(ClickEvent evt)
		{
			_ingredientIndex--;
			if (_ingredientIndex < 0)
			{
				_ingredientIndex = _dish.ingredients.Count - 1;
			}
			RefillDescription();
		}

		private void OnNextButtonClicked(ClickEvent evt)
		{
			_ingredientIndex++;
			if (_ingredientIndex >= _dish.ingredients.Count)
			{
				_ingredientIndex = 0;
			}
			RefillDescription();
		}

		private void SetCollapsedState(bool state)
		{
			_isCollapsed = state;
			if (state)
			{
				_elements.ScreenRoot.AddToClassList(DishDescriptionStyles.ScreenMinimized);
			}
			else
			{
				_elements.ScreenRoot.RemoveFromClassList(DishDescriptionStyles.ScreenMinimized);
			}
			_elements.ScrollView.SetEnabled(!state);
			_elements.ScrollView.ScrollTo(_elements.ScrollView.ElementAt(0));
		}
	}
}