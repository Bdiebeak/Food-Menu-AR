using System;
using System.Collections.Generic;
using UnityEngine;

namespace ARMenu.Scripts.Runtime.Services.ScreenService
{
	public class ScreenService : IScreenService
	{
		private readonly Dictionary<Type, IScreen> _screens = new();

		public void RegisterScreen<TScreen>(TScreen screen) where TScreen : IScreen
		{
			_screens.Add(typeof(TScreen), screen);
		}

		public TScreen GetScreen<TScreen>() where TScreen : IScreen
		{
			if (_screens.TryGetValue(typeof(TScreen), out IScreen screen) == false)
			{
				Debug.LogError($"Can't find registered screen for type {typeof(TScreen).FullName}.");
			}
			return (TScreen)screen;
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