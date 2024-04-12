using UnityEngine.UIElements;

namespace ARMenu.Scripts.Runtime.UI.ScanHint
{
	public class ScanHintElements
	{
		public VisualElement ScreenRoot { get; private set; }

		private readonly UIDocument _uiDocument;

		public ScanHintElements(UIDocument uiDocument)
		{
			_uiDocument = uiDocument;
		}

		public void Initialize()
		{
			ScreenRoot = GetElement<VisualElement>("HintScreen", "ScreenContainer");
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