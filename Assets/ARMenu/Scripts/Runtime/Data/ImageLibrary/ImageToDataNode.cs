using System;
using UnityEngine.XR.ARSubsystems;

namespace ARMenu.Scripts.Runtime.Data.ImageLibrary
{
	[Serializable]
	public class ImageToDataNode<TData> where TData : class
	{
		public string referenceImageName;
		public TData linkedData;

		public ImageToDataNode()
		{
			referenceImageName = string.Empty;
			linkedData = default;
		}

		public ImageToDataNode(string referenceImageName, TData linkedData)
		{
			this.referenceImageName = referenceImageName;
			this.linkedData = linkedData;
		}

		public ImageToDataNode(XRReferenceImage referenceImage, TData linkedData)
		{
			referenceImageName = referenceImage.name;
			this.linkedData = linkedData;
		}
	}
}