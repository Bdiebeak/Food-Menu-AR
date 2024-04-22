using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

namespace ARMenu.Scripts.Runtime.Data.ImageLibrary
{
	[Serializable]
	public abstract class BaseImageLibrary<TData> : ImageLibraryScriptableObject where TData : class
	{
		[SerializeField]
		private XRReferenceImageLibrary _xrReferenceImageLibrary;
		// Dictionary can be used here, but I don't want to deal with its serialization and custom editor now.
		[SerializeField]
		private List<ImageToDataNode<TData>> _imageToDataNodes = new();

		public XRReferenceImageLibrary XRReferenceImageLibrary => _xrReferenceImageLibrary;

		public void AppendLinkedData(string imageName, TData linkedData)
		{
			_imageToDataNodes.Add(new ImageToDataNode<TData>(imageName, linkedData));
		}

		public bool TryGetLinkedData(string imageName, out TData linkedData)
		{
			ImageToDataNode<TData> node = _imageToDataNodes.FirstOrDefault(x => x.arImageName.Equals(imageName));
			linkedData = node?.linkedData;
			return node != null;
		}

		public override void AppendDataFromImageLibrary()
		{
			bool wasAdded = false;
			for (int i = 0; i < _xrReferenceImageLibrary.count; i++)
			{
				XRReferenceImage referenceImage = _xrReferenceImageLibrary[i];
				if (string.IsNullOrWhiteSpace(referenceImage.name))
				{
					continue;
				}

				if (_imageToDataNodes.Any(x => x.arImageName == referenceImage.name) == false)
				{
					wasAdded = true;
					_imageToDataNodes.Add(new ImageToDataNode<TData>(referenceImage.name, default));
					Debug.Log($"Image data for {referenceImage.name} was added.");
				}
			}

			if (wasAdded == false)
			{
				Debug.Log("Nothing to add, all images are already here.");
			}
		}

		public override void Validate()
		{
			bool isCorrect = true;

			// Check same images.
			for (int i = 0; i < _imageToDataNodes.Count; i++)
			{
				var currentDataNode = _imageToDataNodes[i];
				for (int j = 0; j < _imageToDataNodes.Count; j++)
				{
					if (i == j)
					{
						continue;
					}

					if (_imageToDataNodes[j].arImageName == currentDataNode.arImageName)
					{
						Debug.LogWarning($"Image data for {currentDataNode.arImageName} is set few times. Indexes: {i} and {j}.");
						isCorrect = false;
					}
				}
			}

			// Check missing images.
			for (int i = 0; i < _xrReferenceImageLibrary.count; i++)
			{
				XRReferenceImage referenceImage = _xrReferenceImageLibrary[i];
				if (_imageToDataNodes.Any(x => x.arImageName == referenceImage.name) == false)
				{
					Debug.LogWarning($"Image data for {referenceImage.name} wasn't set.");
					isCorrect = false;
				}
			}

			if (isCorrect)
			{
				Debug.Log("Every reference is set up correctly.");
			}
		}
	}
}