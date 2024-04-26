using System;
using System.Collections.Generic;

namespace ARMenu.Scripts.Runtime.Infrastructure
{
	public class AppContext
	{
		private readonly Dictionary<Type, object> _registered = new();

		public void Register<T>(T instance)
		{
			_registered.Add(typeof(T), instance);
		}

		public T Resolve<T>()
		{
			return (T)_registered[typeof(T)];
		}
	}
}