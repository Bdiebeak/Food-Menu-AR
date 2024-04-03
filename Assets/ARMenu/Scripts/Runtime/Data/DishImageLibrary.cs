using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

namespace ARMenu.Scripts.Runtime.Data
{
	[CreateAssetMenu(fileName = nameof(DishImageLibrary), menuName = "ScriptableObjects/" + nameof(DishImageLibrary))]
	public class DishImageLibrary : XRReferenceImageLibrary
	{
		// Dictionary can be used here, but I don't want to deal with its serialization and custom editor now.
		public List<ReferenceImageToDishNode> imageToDishNodes = new();

		public bool TryGetLinkedDish(string imageName, out Dish dish)
		{
			ReferenceImageToDishNode node = imageToDishNodes.FirstOrDefault(x => x.arImage.name.Equals(imageName));
			dish = node?.dish;
			return node != null;
		}
	}
}