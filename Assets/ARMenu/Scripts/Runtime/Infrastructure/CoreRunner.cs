using ARMenu.Scripts.Runtime.Services.DishTracker;
using UnityEngine;

namespace ARMenu.Scripts.Runtime.Infrastructure
{
	public class CoreRunner : MonoBehaviour
	{
		private IDishTrackerAR _dishTracker;

		public void Construct(IDishTrackerAR dishTracker)
		{
			_dishTracker = dishTracker;
		}

		private void Start()
		{
			_dishTracker.Start();
		}

		private void OnDestroy()
		{
			_dishTracker.CleanUp();
		}
	}
}