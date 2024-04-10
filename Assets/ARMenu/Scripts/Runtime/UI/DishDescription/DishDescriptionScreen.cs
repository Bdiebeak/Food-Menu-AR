using ARMenu.Scripts.Runtime.Services.ScreenService;
using UnityEngine;
using UnityEngine.UIElements;

namespace ARMenu.Scripts.Runtime.UI.DishDescription
{
	// TODO: MV*-паттерн для работы с экраном
	// Вдруг я захочу сменить UI Toolkit на UGUI интерфейс. В этом вьшном классе не должно быть бизнес-логики.
	public class DishDescriptionScreen : IScreen
	{
		private readonly DishDescriptionElements _elements;
		private bool _isCollapsed;

		public DishDescriptionScreen(UIDocument document)
		{
			_elements = new DishDescriptionElements(document);
			_elements.Initialize();
		}

		public void Show()
		{
			RegisterCallbacks();
		}

		public void Hide()
		{
			UnregisterCallbacks();
		}

		private void RegisterCallbacks()
		{
			_elements.CollapseButton.RegisterCallback<ClickEvent>(OnCollapseButtonClicked);
			_elements.PreviousButton.RegisterCallback<ClickEvent>(OnPreviousButtonClicked);
			_elements.NextButton.RegisterCallback<ClickEvent>(OnNextButtonClicked);
		}

		private void UnregisterCallbacks()
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
			Debug.Log("Previous");
		}

		private void OnNextButtonClicked(ClickEvent evt)
		{
			Debug.Log("Next");
		}

		private void SetCollapsedState(bool state)
		{
			_isCollapsed = state;
			if (state)
			{
				_elements.RootContainer.AddToClassList(DishDescriptionStyles.ScreenMinimized);
			}
			else
			{
				_elements.RootContainer.RemoveFromClassList(DishDescriptionStyles.ScreenMinimized);
			}
			_elements.RootContainer.SetEnabled(!state);
		}
	}
}