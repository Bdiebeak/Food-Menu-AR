using UnityEngine.UIElements;

namespace ARMenu.Scripts.Runtime.UI.General.Uxml
{
	public abstract class UxmlElementsContainer
	{
		public VisualElement ElementRoot { get; }

		protected UxmlElementsContainer(VisualElement elementRoot)
		{
			ElementRoot = elementRoot;
		}

		protected T GetElement<T>(string key) where T : VisualElement
		{
			return ElementRoot.Q<T>(key);
		}
	}
}