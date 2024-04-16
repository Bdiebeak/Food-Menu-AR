using ARMenu.Scripts.Runtime.UI.General.Uxml;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace ARMenu.Scripts.Runtime.UI.DishDescription
{
	/// <summary>
	/// Своего рода View из MVVM.
	/// View - должен обраабатывать пользовательский ввод, изменяя ViewModel и отображать его текущее состояние
	/// на основе биндинга данных.
	/// </summary>
	public class DishDescriptionUxmlScreen : UxmlBaseScreen<DishDescriptionViewModel>
	{
		private readonly DishDescriptionUxmlElements _elements;

		protected override VisualElement ScreenRoot => _elements.ElementRoot;

		public DishDescriptionUxmlScreen(UIDocument document, DishDescriptionViewModel viewModel) : base(viewModel)
		{
			VisualElement screenRoot = document.rootVisualElement.Q<VisualElement>("DishDescriptionScreen");
			_elements = new DishDescriptionUxmlElements(screenRoot);
			BindData();
		}

		private void BindData()
		{
			ScreenRoot.dataSource = viewModel;
			_elements.DishElement.Name.SetBinding(nameof(Label.text), new DataBinding()
			{
				dataSourcePath = new PropertyPath(nameof(DishDescriptionViewModel.DishName)),
				bindingMode = BindingMode.ToTarget
			});

			_elements.DishElement.Description.SetBinding(nameof(Label.text), new DataBinding()
			{
				dataSourcePath = new PropertyPath(nameof(DishDescriptionViewModel.DishDescription)),
				bindingMode = BindingMode.ToTarget
			});

			_elements.IngredientElement.Name.SetBinding(nameof(Label.text), new DataBinding()
			{
				dataSourcePath = new PropertyPath(nameof(DishDescriptionViewModel.IngredientName)),
				bindingMode = BindingMode.ToTarget
			});

			_elements.IngredientElement.Description.SetBinding(nameof(Label.text), new DataBinding()
			{
				dataSourcePath = new PropertyPath(nameof(DishDescriptionViewModel.IngredientDescription)),
				bindingMode = BindingMode.ToTarget
			});
		}

		protected override void SubscribeEvents()
		{
			viewModel.Changed += OnViewModelChanged;
			_elements.CollapseButton.RegisterCallback<ClickEvent>(OnCollapseButtonClicked);
			_elements.NextButton.RegisterCallback<ClickEvent>(OnNextButtonClicked);
			_elements.PreviousButton.RegisterCallback<ClickEvent>(OnPreviousButtonClicked);
		}

		protected override void UnsubscribeEvents()
		{
			viewModel.Changed -= OnViewModelChanged;
			_elements.CollapseButton.UnregisterCallback<ClickEvent>(OnCollapseButtonClicked);
			_elements.NextButton.UnregisterCallback<ClickEvent>(OnNextButtonClicked);
			_elements.PreviousButton.UnregisterCallback<ClickEvent>(OnPreviousButtonClicked);
		}

		private void OnViewModelChanged()
		{
			OnCollapseStateChanged(viewModel.IsCollapsed);
			SetDishImage(viewModel.DishImage);
			SetIngredientImage(viewModel.IngredientImage);
		}

		private void OnCollapseStateChanged(bool newState)
		{
			if (newState)
			{
				_elements.ScrollView.SetEnabled(false);
				_elements.ScreenContainer.AddToClassList(DishDescriptionStyles.ScreenMinimized);
			}
			else
			{
				_elements.ScrollView.SetEnabled(true);
				_elements.ScreenContainer.RemoveFromClassList(DishDescriptionStyles.ScreenMinimized);
			}
			_elements.ScrollView.ScrollTo(_elements.ScrollView.ElementAt(0)); // TODO: always called.
		}

		private void SetDishImage(Texture2D image)
		{
			_elements.DishElement.Image.style.backgroundImage = new StyleBackground(image);
		}

		private void SetIngredientImage(Texture2D image)
		{
			_elements.IngredientElement.Image.style.backgroundImage = new StyleBackground(image);
		}

		private void OnCollapseButtonClicked(ClickEvent evt)
		{
			viewModel.SwitchCollapsedState();
		}

		private void OnNextButtonClicked(ClickEvent evt)
		{
			viewModel.NextIngredient();
		}

		private void OnPreviousButtonClicked(ClickEvent evt)
		{
			viewModel.PreviousIngredient();
		}
	}
}