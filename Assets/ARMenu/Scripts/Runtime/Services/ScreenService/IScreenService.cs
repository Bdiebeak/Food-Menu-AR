namespace ARMenu.Scripts.Runtime.Services.ScreenService
{
	public interface IScreenService
	{
		public void RegisterScreen<T>(T screen) where T : IScreen;
		public T GetScreen<T>() where T : IScreen;
		public void HideAll();
	}
}