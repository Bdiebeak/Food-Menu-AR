using ARMenu.Scripts.Runtime.UI.General;
using UnityEngine.UIElements;

namespace ARMenu.Scripts.Runtime.UI.ScanHint
{
	public class ScanHintScreen : UxmlBaseScreen
	{
		private readonly ScanHintElements _elements;

		public ScanHintScreen(UIDocument document)
		{
			_elements = new ScanHintElements(document);
			_elements.Initialize();
		}

		protected override VisualElement GetScreenRoot()
		{
			return _elements.ScreenRoot;
		}

		protected override void SubscribeEvents()
		{
		}

		protected override void UnsubscribeEvents()
		{
		}
	}
}