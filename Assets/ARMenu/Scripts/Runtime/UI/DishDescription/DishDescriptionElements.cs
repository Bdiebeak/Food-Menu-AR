using UnityEngine.UIElements;

namespace ARMenu.Scripts.Runtime.UI.DishDescription
{
	public class DishDescriptionElements
	{
		public VisualElement RootContainer { get; private set; }
		public ScrollView ScrollView { get; private set; }
		public Button CollapseButton { get; private set; }
		public Button PreviousButton { get; private set; }
		public Button NextButton { get; private set; }

		private readonly VisualElement _rootElement;

		public DishDescriptionElements(UIDocument document)
		{
			_rootElement = document.rootVisualElement;
		}

		public void Initialize()
		{
			// P.S. doesn't make constants, because it is used and located only in one place.
			RootContainer = GetElement<VisualElement>("RootVisualElement");
			ScrollView = GetElement<ScrollView>("ScrollView");
			CollapseButton = GetElement<Button>("CollapseButton");
			PreviousButton = GetElement<Button>("PreviousButton");
			NextButton = GetElement<Button>("NextButton");
		}

		private T GetElement<T>(string key) where T : VisualElement
		{
			return _rootElement.Q<T>(key);
		}
	}
}