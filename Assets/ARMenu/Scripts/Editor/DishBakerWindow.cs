using System;
using System.Collections.Generic;
using System.IO;
using Unity.Properties;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace ARMenu.Scripts.Editor
{
	// TODO: use data binding
	public class DishBakerWindow : EditorWindow
	{
		public VisualTreeAsset ui;
		public Camera cameraObject;
		public Vector2Int resolution = new (1024, 1024);
		public Transform rootObject;
		public List<Transform> additionalObjects = new();
		public string savePath;

		[MenuItem("Services/DishBaker")]
		public static void ShowWindow()
		{
			DishBakerWindow window = GetWindow<DishBakerWindow>();
			window.titleContent = new GUIContent(nameof(DishBakerWindow));
		}

		public void CreateGUI()
		{
			VisualElement root = rootVisualElement;
			ui.CloneTree(root);

			// InitializeDefaultValues();
			// InitializeResolutionField(root.Q<Vector2IntField>("ResolutionField"));
			// InitializeCameraField(root.Q<ObjectField>("CameraField"));
			// InitializeRootObjectField(root.Q<ObjectField>("RootObjectField"));
			// InitializeListViewForAdditionalObjects(root.Q<ListView>("AdditionalObjectsListView"));
			// InitializePathTextField(root.Q<TextField>("FolderPathTextField"));
			// InitializeSelectPathButton(root.Q<Button>("SelectPathButton"));
			InitializeBakeButton(root.Q<Button>("BakeButton"));
		}

		private void InitializeDefaultValues()
		{
			cameraObject = Camera.main;
			savePath = Application.dataPath;
		}

		private void InitializeResolutionField(Vector2IntField resolutionField)
		{
			resolutionField.SetBinding(nameof(Vector2IntField.value), new DataBinding()
			{
				dataSourcePath = new PropertyPath(nameof(resolution)),
				bindingMode = BindingMode.TwoWay
			});
		}

		private void InitializeCameraField(ObjectField cameraField)
		{
			cameraField.SetBinding(nameof(ObjectField.value), new DataBinding()
			{
				dataSourcePath = new PropertyPath(nameof(cameraObject)),
				bindingMode = BindingMode.TwoWay
			});
		}

		private void InitializeRootObjectField(ObjectField rootField)
		{
			// rootField.RegisterCallback<ChangeEvent<Object>>((evt) =>
			// {
			// 	_rootObject = evt.newValue as Transform;
			// });
		}

		private void InitializeSelectPathButton(Button pathSelectButton)
		{
			pathSelectButton.RegisterCallback<MouseUpEvent>(evt =>
			{
				savePath = EditorUtility.OpenFolderPanel("Select Folder To Bake", Application.dataPath, "");
			});
		}

		private void InitializePathTextField(TextField textField)
		{
			textField.value = savePath;
		}

		private void InitializeListViewForAdditionalObjects(ListView listView)
		{
			listView.itemsSource = additionalObjects;
			listView.makeItem = () =>
			{
				return new ObjectField() { objectType = typeof(Transform), allowSceneObjects = true };
			};
			listView.bindItem = (element, i) =>
			{
				((ObjectField)element).value = additionalObjects[i];
				((ObjectField)element).RegisterValueChangedCallback((value) =>
				{
					additionalObjects[i] = (Transform)value?.newValue;
				});
			};
			listView.RegisterCallback<DragUpdatedEvent>(evt =>
			{
				DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
			});
			listView.RegisterCallback<DragPerformEvent>(evt =>
			{
				var objects = DragAndDrop.objectReferences;
				foreach (Object obj in objects)
				{
					if (obj is GameObject gameObject)
					{
						additionalObjects.Add(gameObject.transform);
					}
				}
				listView.RefreshItems();
			});
		}

		private void InitializeBakeButton(Button bakeButton)
		{
			bakeButton.RegisterCallback<MouseUpEvent>(evt =>
			{
				BakeImages();
			});
		}

		// TODO: separate logic CameraBaker.Bake(root, additional, path, ...)
		private void BakeImages()
		{
			// Create texture for camera baker.
			Texture2D image = new(resolution.x, resolution.y);
			RenderTexture tempRT = new(resolution.x, resolution.y, 24, RenderTextureFormat.ARGB32);

			try
			{
				// Bake all required object into separated files.
				CameraBaker.SaveCameraRender(Path.Combine(savePath, $"{rootObject.name}.png"), cameraObject, image, tempRT);

				// Bake additional objects
				foreach (Transform parent in rootObject)
				{
					parent.gameObject.SetActive(false);
				}
				foreach (Transform additional in additionalObjects)
				{
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
				DestroyImmediate(image);
				DestroyImmediate(tempRT);
			}
		}
	}
}