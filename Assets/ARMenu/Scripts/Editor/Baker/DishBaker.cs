using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ARMenu.Scripts.Editor.Baker
{
	public class DishBaker
	{
		public Camera cameraObject;
		public Vector2Int resolution = new (1024, 1024);
		public Transform rootObject;
		public List<Transform> additionalObjects = new();
		public string savePath;

		public void BakeImages()
		{
			// Create texture for camera baker.
			Texture2D image = new(resolution.x, resolution.y);
			RenderTexture tempRT = new(resolution.x, resolution.y, 24, RenderTextureFormat.ARGB32);

			// Bake all required object into separated files.
			try
			{
				// Bake root object.
				CameraBaker.SaveCameraRender(Path.Combine(savePath, $"{rootObject.name}.png"), cameraObject, image, tempRT);

				// Bake additional objects.
				foreach (Transform parent in rootObject)
				{
					parent.gameObject.SetActive(false);
				}
				foreach (Transform additional in additionalObjects)
				{
					if (additional == null)
					{
						continue;
					}
					additional.gameObject.SetActive(true);
					CameraBaker.SaveCameraRender(Path.Combine(savePath, $"{additional.name}.png"), cameraObject, image, tempRT);
					additional.gameObject.SetActive(false);
				}
				foreach (Transform parent in rootObject)
				{
					parent.gameObject.SetActive(true);
				}
				AssetDatabase.Refresh();
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
			finally
			{
				// Cleanup textures.
				Object.DestroyImmediate(image);
				Object.DestroyImmediate(tempRT);
			}
		}
	}
}