using System.Text;
using ARMenu.Scripts.Runtime.Data;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

namespace ARMenu.Scripts.Runtime
{
	public class GuidDebugger : MonoBehaviour
	{
		public Menu menu;
		public XRReferenceImageLibrary imageLibrary;
		[Space]
		public TMP_Text menuText;
		public TMP_Text libraryText;

		private void OnEnable()
		{
			StringBuilder textBuilder = new();

			textBuilder.AppendLine("MENU");
			foreach (DishToImageNode dishNode in menu)
			{
				textBuilder.AppendLine($"{dishNode.arImage.name}, {dishNode.imageGuid}");
			}
			menuText.SetText(textBuilder.ToString());

			textBuilder.Clear();

			textBuilder.AppendLine("LIBRARY");
			foreach (XRReferenceImage referenceImage in imageLibrary)
			{
				textBuilder.AppendLine($"{referenceImage.name}, {referenceImage.texture.name}, {referenceImage.textureGuid}");
			}
			libraryText.SetText(textBuilder.ToString());
		}
	}
}
