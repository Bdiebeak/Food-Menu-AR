namespace ARMenu.Scripts.Runtime.UI.General.Mvvm
{
	public abstract class View<TViewModel> where TViewModel : ViewModel
	{
		protected readonly TViewModel viewModel;

		protected View(TViewModel viewModel)
		{
			this.viewModel = viewModel;
		}
	}
}