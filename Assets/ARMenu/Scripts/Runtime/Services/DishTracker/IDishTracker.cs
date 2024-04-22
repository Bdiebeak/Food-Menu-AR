using System;
using ARMenu.Scripts.Runtime.Data;
using ARMenu.Scripts.Runtime.Data.ImageLibrary.Dishes;

namespace ARMenu.Scripts.Runtime.Services.DishTracker
{
	public interface IDishTracker
	{
		public event Action<Dish> TrackingChanged;
		public event Action TrackingLost;

		public void Initialize(DishImageLibrary dishImageLibrary);
		public void CleanUp();
	}
}