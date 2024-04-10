using System;
using System.Collections.Generic;
using UnityEngine;

namespace ARMenu.Scripts.Runtime.Services.ScreenService
{
	public class ScreenService : IScreenService
	{
		private Dictionary<Type, IScreen> _screens;

		public void RegisterScreen<T>(T screen) where T : IScreen
		{
			_screens.Add(typeof(T), screen);
		}

		public T GetScreen<T>() where T : IScreen
		{
			if (_screens.TryGetValue(typeof(T), out IScreen screen) == false)
			{
				Debug.LogError($"Can't find registered screen for type {typeof(T).FullName}.");
			}
			return (T)screen;
		}

		public void HideAll()
		{
			foreach (IScreen screen in _screens.Values)
			{
				screen.Hide();
			}
		}
	}
}