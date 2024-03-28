using ARMenu.Scripts.Runtime.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using Menu = ARMenu.Scripts.Runtime.Data.Menu;

namespace ARMenu.Scripts.Editor
{
	public static class TexturesDebug
	{
		[MenuItem ("CONTEXT/XRReferenceImageLibrary/Print Info")]
		public static void PrintReferenceImagesInfo (MenuCommand command)
		{
			XRReferenceImageLibrary imageLibrary = (XRReferenceImageLibrary)command.context;
			foreach (XRReferenceImage referenceImage in imageLibrary)
			{
				Debug.Log($"Name: {referenceImage.name}, GUID: {referenceImage.guid}, T-GUID: {referenceImage.textureGuid}.");
			}
		}

		[MenuItem ("CONTEXT/Menu/Print Info")]
		public static void PrintMenuImagesInfo (MenuCommand command)
		{
			Menu menu = (Menu)command.context;
			foreach (DishToImageNode dishToImage in menu)
			{
				string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(dishToImage.arImage));
				Debug.Log($"Name: {dishToImage.arImage.name}, GUID: {guid}");
			}
		}

		[MenuItem ("CONTEXT/Menu/Bake")]
		public static void BakeMenuImagesInfo (MenuCommand command)
		{
			Menu menu = (Menu)command.context;
			foreach (DishToImageNode dishToImage in menu)
			{
				string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(dishToImage.arImage));
				dishToImage.imageGuid = guid;
			}
		}
	}
}