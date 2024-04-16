using ARMenu.Scripts.Runtime.Services.ScreenService;
using ARMenu.Scripts.Runtime.UI.General.Mvvm;
using UnityEngine.UIElements;

namespace ARMenu.Scripts.Runtime.UI.General.Uxml
{
	public abstract class UxmlBaseScreen<TViewModel> : View<TViewModel>, IScreen where TViewModel : ViewModel
	{
		protected abstract VisualElement ScreenRoot { get; }

		protected UxmlBaseScreen(TViewModel viewModel) : base(viewModel) { }

		public void Show()
		{
			SubscribeEvents();
			DoShowLogic();
			viewModel.Initialize();
		}

		protected abstract void SubscribeEvents();

		protected virtual void DoShowLogic()
		{
			ScreenRoot.RemoveFromClassList(GeneralStyles.Hidden);
		}

		public void Close()
		{
			Hide();
		}

		public void Hide()
		{
			viewModel.Clear();
			DoHideLogic();
			UnsubscribeEvents();
		}

		protected abstract void UnsubscribeEvents();

		protected virtual void DoHideLogic()
		{
			ScreenRoot.AddToClassList(GeneralStyles.Hidden);
		}
	}
}