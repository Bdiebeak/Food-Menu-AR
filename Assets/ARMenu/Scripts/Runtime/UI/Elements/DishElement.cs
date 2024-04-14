using UnityEngine;
using UnityEngine.UIElements;

namespace ARMenu.Scripts.Runtime.UI.Elements
{
	public class DishElement
	{
		public Label Name { get; private set; }
		public VisualElement Image { get; private set; } // TODO: LoadingImage?
		public Label Description { get; private set; }

		private readonly VisualElement _elementRoot;

		public DishElement(VisualElement elementRoot)
		{
			_elementRoot = elementRoot;
		}

		public void SetName(string name)
		{
			Name.text = name;
		}

		public void SetImage(Texture2D image)
		{
			Image.style.backgroundImage = new StyleBackground(image);
		}

		public void SetDescription(string description)
		{
			Description.text = description;
		}
	}
}