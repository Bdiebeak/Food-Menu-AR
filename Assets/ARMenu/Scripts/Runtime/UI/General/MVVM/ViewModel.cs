using System;

namespace ARMenu.Scripts.Runtime.UI.General.MVVM
{
	/// <summary>
	/// A kind of ViewModel from MVVM pattern.
	/// It stores data required only by the View e.g. current state of UI.
	/// As well as logic for processing commands from the View to change the Model.
	/// So, for example, a ViewModel would duplicate only those fields from the Model that are needed to be displayed in the View.
	/// To see example implementation, check <seealso cref="ARMenu.Scripts.Runtime.UI.DishDescription.DishDescriptionViewModel"/>
	/// </summary>
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