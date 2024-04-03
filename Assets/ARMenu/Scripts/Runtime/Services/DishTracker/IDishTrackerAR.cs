using ARMenu.Scripts.Runtime.Data;

namespace ARMenu.Scripts.Runtime.Services.DishTracker
{
	public interface IDishTrackerAR
	{
		public void SetImageLibrary(DishImageLibrary initialLibrary);
		public void Start();
		public void CleanUp();
	}
}