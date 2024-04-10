using ARMenu.Scripts.Runtime.Data;
using ARMenu.Scripts.Runtime.Services.DishTracker;
using ARMenu.Scripts.Runtime.Services.ScreenService;
using ARMenu.Scripts.Runtime.UI.DishDescription;
using UnityEngine;

namespace ARMenu.Scripts.Runtime.Infrastructure
{
	public class CoreRunner : MonoBehaviour
	{
		private IDishTrackerAR _dishTracker;
		private IScreenService _screenService;

		public void Construct(IDishTrackerAR dishTracker, IScreenService screenService)
		{
			_dishTracker = dishTracker;
			_screenService = screenService;
		}

		private void Start()
		{
			_dishTracker.TrackingDishChanged += OnTrackingDishChanged;
			_dishTracker.Start();
			_screenService.GetScreen<DishDescriptionScreen>().Show();
		}

		private void OnDestroy()
		{
			_dishTracker.TrackingDishChanged -= OnTrackingDishChanged;
			_dishTracker.CleanUp();
			_screenService.HideAll();
		}

		private void OnTrackingDishChanged(Dish newDish)
		{
			// TODO: если нет отслеживаемого блюда, то показать экран "Scan QR code".
			// TODO: елси есть блюда, показывать экран с его информацией.
		}
	}
}