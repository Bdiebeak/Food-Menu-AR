using System;
using ARMenu.Scripts.Runtime.Data;

namespace ARMenu.Scripts.Runtime.Services.DishTracker
{
	public interface IDishTrackerAR
	{
		public event Action<Dish> TrackingDishChanged;

		public void SetImageLibrary(DishImageLibrary initialLibrary);
		public void Start();
		public void CleanUp();
	}
}