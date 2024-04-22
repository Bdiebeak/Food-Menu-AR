using ARMenu.Scripts.Runtime.UI.General.UXML;
using UnityEngine.UIElements;

namespace ARMenu.Scripts.Runtime.UI.DishDescription
{
	public class DishDescriptionUxmlElements : UxmlElementsContainer
	{
		public VisualElement ScreenContainer { get; private set; }
		public ScrollView ScrollView { get; private set; }
		public Button CollapseButton { get; private set; }
		public Button PreviousButton { get; private set; }
		public Button NextButton { get; private set; }
		public DishElement DishElement { get; private set; }
		public DishElement IngredientElement { get; private set; }

		public DishDescriptionUxmlElements(VisualElement visualElement) : base(visualElement)
		{
			InitializeElements();
		}

		private void InitializeElements()
		{
			// P.S. doesn't make constants, because it is used and located only in one place.
			ScreenContainer = GetElement<VisualElement>("ScreenContainer");
			ScrollView = GetElement<ScrollView>("ScrollView");
			CollapseButton = GetElement<Button>("CollapseButton");
			PreviousButton = GetElement<Button>("PreviousButton");
			NextButton = GetElement<Button>("NextButton");
			DishElement = new DishElement(GetElement<VisualElement>("DishContainer"));
			IngredientElement = new DishElement(GetElement<VisualElement>("IngredientsContainer"));
		}
	}
}