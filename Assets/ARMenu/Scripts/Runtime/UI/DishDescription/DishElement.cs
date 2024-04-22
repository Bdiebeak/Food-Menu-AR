using ARMenu.Scripts.Runtime.UI.General.UXML;
using UnityEngine.UIElements;

namespace ARMenu.Scripts.Runtime.UI.DishDescription
{
	public class DishElement : UxmlElementsContainer
	{
		public Label Name { get; }
		public VisualElement Image { get; }
		public Label Description { get; }

		public DishElement(VisualElement elementRoot) : base(elementRoot)
		{
			Name = GetElement<Label>("DishName");
			Image = GetElement<VisualElement>("DishImage");
			Description = GetElement<Label>("DishDescription");
		}
	}
}