using ARMenu.Scripts.Runtime.UI.General.UXML;
using UnityEngine.UIElements;

namespace ARMenu.Scripts.Runtime.UI.ScanHint
{
	public class HintUxmlElements : UxmlElementsContainer
	{
		public VisualElement ScreenContainer { get; private set; }
		public Label Hint { get; private set; }

		public HintUxmlElements(VisualElement visualElement) : base(visualElement)
		{
			InitializeElements();
		}

		private void InitializeElements()
		{
			ScreenContainer = GetElement<VisualElement>("ScreenContainer");
			Hint = GetElement<Label>("HintText");
		}
	}
}