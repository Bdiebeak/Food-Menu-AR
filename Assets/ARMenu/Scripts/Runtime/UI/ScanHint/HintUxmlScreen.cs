using ARMenu.Scripts.Runtime.UI.General.UXML;
using Unity.Properties;
using UnityEngine.UIElements;

namespace ARMenu.Scripts.Runtime.UI.ScanHint
{
	public class HintUxmlScreen : UxmlBaseScreen<HintViewModel>
	{
		private readonly HintUxmlElements _elements;

		protected override VisualElement ScreenRoot => _elements.ElementRoot;

		public HintUxmlScreen(UIDocument document, HintViewModel viewModel) : base(viewModel)
		{
			VisualElement screenRoot = document.rootVisualElement.Q<VisualElement>("HintScreen");
			_elements = new HintUxmlElements(screenRoot);
			BindData();
		}

		private void BindData()
		{
			ScreenRoot.dataSource = viewModel;
			_elements.Hint.SetBinding(nameof(Label.text), new DataBinding()
			{
				dataSourcePath = new PropertyPath(nameof(HintViewModel.HintText)),
				bindingMode = BindingMode.ToTarget
			});
		}

		protected override void SubscribeEvents() { }
		protected override void UnsubscribeEvents() { }
	}
}