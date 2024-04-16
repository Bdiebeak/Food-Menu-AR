using System;

namespace ARMenu.Scripts.Runtime.UI.General.Mvvm
{
	public abstract class ViewModel
	{
		public event Action Changed;

		public abstract void Initialize();

		public abstract void Clear();

		protected void CallChangedEvent()
		{
			Changed?.Invoke();
		}
	}
}