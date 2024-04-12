using UnityEngine.UIElements;

namespace ARMenu.Scripts.Runtime.UI.DishDescription
{
	public class DishDescriptionElements
	{
		public VisualElement ScreenRoot { get; private set; }
		public ScrollView ScrollView { get; private set; }
		public Button CollapseButton { get; private set; }

		// TODO: create FoodDescriptionElement in UXML with custom controls.
		public Label DishName { get; private set; }
		public VisualElement DishImage { get; private set; }
		public Label DishDescription { get; private set; }

		public Button PreviousButton { get; private set; }
		public Button NextButton { get; private set; }
		public Label IngredientName { get; private set; }
		public VisualElement IngredientImage { get; private set; }
		public Label IngredientDescription { get; private set; }

		private readonly UIDocument _uiDocument;

		public DishDescriptionElements(UIDocument uiDocument)
		{
			_uiDocument = uiDocument;
		}

		public void Initialize()
		{
			// P.S. doesn't make constants, because it is used and located only in one place.
			ScreenRoot = GetElement<VisualElement>("DishDescriptionScreen", "ScreenContainer");
			ScrollView = GetElement<ScrollView>("DishDescriptionScreen", "ScrollView");
			CollapseButton = GetElement<Button>("DishDescriptionScreen", "CollapseButton");

			DishName = GetElement<Label>("DishContainer", "DishName");
			DishImage = GetElement<VisualElement>("DishContainer", "DishImage");
			DishDescription = GetElement<Label>("DishContainer", "DishDescription");

			PreviousButton = GetElement<Button>("IngredientsContainer", "PreviousButton");
			NextButton = GetElement<Button>("IngredientsContainer", "NextButton");
			IngredientName = GetElement<Label>("IngredientsContainer", "DishName");
			IngredientImage = GetElement<VisualElement>("IngredientsContainer", "DishImage");
			IngredientDescription = GetElement<Label>("IngredientsContainer", "DishDescription");
		}

		private T GetElement<T>(string root, string key) where T : VisualElement
		{
			if (string.IsNullOrWhiteSpace(root))
			{
				return _uiDocument.rootVisualElement.Q<T>(key);
			}
			return _uiDocument.rootVisualElement.Q<VisualElement>(root).Q<T>(key);
		}
	}
}