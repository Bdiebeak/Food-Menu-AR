using System;

namespace ARMenu.Scripts.Runtime.Data.ImageLibrary
{
	[Serializable]
	public class ImageToDataNode<TData> where TData : class
	{
		public string arImageName;
		public TData linkedData;

		public ImageToDataNode()
		{
			arImageName = string.Empty;
			linkedData = default;
		}

		public ImageToDataNode(string arImageName, TData linkedData)
		{
			this.arImageName = arImageName;
			this.linkedData = linkedData;
		}
	}
}