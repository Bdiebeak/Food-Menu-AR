using ARMenu.Scripts.Runtime.Services.ScreenService;
using UnityEngine.UIElements;

namespace ARMenu.Scripts.Runtime.UI.General
{
	public abstract class UxmlBaseScreen : IScreen
	{
		protected abstract VisualElement GetScreenRoot();

		public void Show()
		{
			DoShowLogic();
			SubscribeEvents();
		}

		protected virtual void DoShowLogic()
		{
			GetScreenRoot().RemoveFromClassList(GeneralStyles.Hidden);
		}

		protected abstract void SubscribeEvents();

		public void Hide()
		{
			DoHideLogic();
			UnsubscribeEvents();
		}

		protected virtual void DoHideLogic()
		{
			GetScreenRoot().AddToClassList(GeneralStyles.Hidden);
		}

		protected abstract void UnsubscribeEvents();

		public void Close()
		{
			Hide();
		}
	}
}