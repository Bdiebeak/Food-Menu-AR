namespace ARMenu.Scripts.Runtime.Services.ScreenService
{
	public interface IScreenService
	{
		public void RegisterScreen<TScreen>(TScreen screen) where TScreen : IScreen;
		public TScreen GetScreen<TScreen>() where TScreen : IScreen;
		public void Show<TScreen>() where TScreen : IScreen;
		public void Hide<TScreen>() where TScreen : IScreen;
		public void HideAll();
	}
}