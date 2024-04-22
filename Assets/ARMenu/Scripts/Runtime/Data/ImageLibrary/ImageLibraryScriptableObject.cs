using System;
using UnityEngine;

namespace ARMenu.Scripts.Runtime.Data.ImageLibrary
{
	/// <summary>
	/// This is a base Scriptable object class for Image library.
	/// It was created mainly to have the ability to call its base functions from a custom editor,
	/// avoiding problems with generic functions.
	/// </summary>
	[Serializable]
	public abstract class ImageLibraryScriptableObject : ScriptableObject
	{
		public virtual void AppendDataFromImageLibrary() { }
		public virtual void Validate() { }
	}
}