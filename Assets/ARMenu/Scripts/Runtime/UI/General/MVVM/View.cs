namespace ARMenu.Scripts.Runtime.UI.General.MVVM
{
	/// <summary>
	/// A kind of View from MVVM.
	/// The View must process user input, call required functions from ViewModel, and
	/// display the current state based on data binding.
	/// </summary>
	public abstract class View<TViewModel> where TViewModel : ViewModel
	{
		protected readonly TViewModel viewModel;

		protected View(TViewModel viewModel)
		{
			this.viewModel = viewModel;
		}
	}
}