using System;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ARMenu.Editor
{
	public static class CameraBaker
	{
		public static void SaveCameraRender(string fullPath, int width = 1024, int height = 1024)
		{
			Texture2D image = new(width, height);
			RenderTexture tempRT = new(width, height, 24, RenderTextureFormat.ARGB32);
			SaveCameraRender(fullPath, image, tempRT);

			Object.DestroyImmediate(image);
			Object.DestroyImmediate(tempRT);
		}

		public static void SaveCameraRender(string fullPath, Texture2D image, RenderTexture tempRT)
		{
			try
			{
				Camera mainCamera = Camera.main;
				if (mainCamera == null)
				{
					Debug.LogError("Can't find main camera.");
					throw new NullReferenceException("Main camera is null.");
				}

				// Cache texture to reset them after process.
				RenderTexture cachedActiveRenderTexture = RenderTexture.active;
				RenderTexture cachedCameraTexture = mainCamera.targetTexture;

				// Assign new temp render texture to save image from camera.
				RenderTexture.active = tempRT;
				mainCamera.targetTexture = tempRT;

				// Bake pixels from camera to image.
				mainCamera.Render();
				image.ReadPixels(new Rect(0, 0, tempRT.width, tempRT.height), 0, 0);
				image.Apply();

				// Reset cached values.
				RenderTexture.active = cachedActiveRenderTexture;
				mainCamera.targetTexture = cachedCameraTexture;

				// Save image as file.
				byte[] bytes = image.EncodeToPNG();
				File.WriteAllBytes(fullPath, bytes);
				Debug.Log($"File was saved into {fullPath}.");
			}
			catch (Exception e)
			{
				Debug.LogException(e);
				throw;
			}
		}
	}
}