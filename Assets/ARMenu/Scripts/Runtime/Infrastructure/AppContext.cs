using System;
using System.Collections.Generic;

namespace ARMenu.Scripts.Runtime.Infrastructure
{
	public class AppContext
	{
		private Dictionary<Type, object> _registeredTypes = new();

		public void Register<T>(T instance)
		{
			_registeredTypes.Add(typeof(T), instance);
		}

		public T Resolve<T>()
		{
			return (T)_registeredTypes[typeof(T)];
		}
	}
}