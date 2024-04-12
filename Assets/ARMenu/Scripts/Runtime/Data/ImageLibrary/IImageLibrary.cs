using UnityEngine.XR.ARSubsystems;

namespace ARMenu.Scripts.Runtime.Data
{
	public interface IImageLibrary<TData> where TData : class
	{
		public bool TryGetLinkedDish(string imageName, out TData dish);
		public IReferenceImageLibrary GetReferenceImageLibrary(); // TODO: is it ok?
	}
}