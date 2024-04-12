using System;
using ARMenu.Scripts.Runtime.Data;

namespace ARMenu.Scripts.Runtime.Services.DishTracker
{
	// TODO: general IArTracker
	public interface IDishTracker
	{
		public event Action<Dish> TrackingChanged;
		public event Action TrackingLost;

		public void Initialize(IImageLibrary<Dish> dishImageLibrary);
		public void CleanUp();
	}
}