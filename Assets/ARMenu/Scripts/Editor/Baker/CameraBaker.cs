using System.IO;
using UnityEngine;

namespace ARMenu.Scripts.Editor.Baker
{
	public static class CameraBaker
	{
		/// <summary>
		/// This method saves the current camera view to the specified path.
		/// Before using this method, ensure that temporary textures,
		/// <paramref name="texture"/> and <paramref name="renderTexture"/>, are properly set up.
		/// The creation of these textures is left to you for more fine-tuning and optimal use.
		/// For example, you can initialize them once and bake the camera multiple times.
		/// Afterwards, you can safely clean up and dispose them.
		/// </summary>
		/// <param name="fullPath">The complete path including the file name and extension.</param>
		/// <param name="camera">The camera whose render will be baked.</param>
		/// <param name="texture">Temporary <typeparamref name="Texture2D"/> is used for baking the image and saving.</param>
		/// <param name="renderTexture">Temporary <typeparamref name="RenderTexture"/> is used for baking the image and saving.</param>
		public static void SaveCameraRender(string fullPath, Camera camera, Texture2D texture, RenderTexture renderTexture)
		{
			// Cache active textures to reset them after process.
			RenderTexture cachedActiveRenderTexture = RenderTexture.active;
			RenderTexture cachedCameraTexture = camera.targetTexture;

			try
			{
				// Assign new temp texture to save image from camera.
				RenderTexture.active = renderTexture;
				camera.targetTexture = renderTexture;

				// Bake pixels from camera to image.
				camera.Render();
				texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
				texture.Apply();
			}
			finally
			{
				// Reset cached values.
				RenderTexture.active = cachedActiveRenderTexture;
				camera.targetTexture = cachedCameraTexture;
			}

			SaveTextureAsPNG(texture, fullPath);
		}

		private static void SaveTextureAsPNG(Texture2D texture, string fullPath)
		{
			string directory = Path.GetDirectoryName(fullPath);
			Directory.CreateDirectory(directory);

			byte[] bytes = texture.EncodeToPNG();
			File.WriteAllBytes(fullPath, bytes);
		}
	}
}