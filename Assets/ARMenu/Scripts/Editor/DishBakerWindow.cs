using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace ARMenu.Scripts.Editor
{
	// TODO: refactoring
	public class DishBakerWindow : EditorWindow
	{
		public VisualTreeAsset ui;

		private TextField _pathTextField;

		private Camera _cameraObject;
		private Vector2Int _resolution = new (1024, 1024);
		private Transform _rootObject;
		private List<Transform> _additionalObjects = new();
		private string _savePath;

		[MenuItem("Services/DishBaker")]
		public static void ShowExample()
		{
			DishBakerWindow window = GetWindow<DishBakerWindow>();
			window.titleContent = new GUIContent(nameof(DishBakerWindow));
		}

		public void CreateGUI()
		{
			VisualElement root = rootVisualElement;
			ui.CloneTree(root);

			InitializeDefaultValues();
			InitializeResolutionField(root.Q<Vector2IntField>("ResolutionField"));
			InitializeCameraField(root.Q<ObjectField>("CameraField"));
			InitializeRootObjectField(root.Q<ObjectField>("RootObjectField"));
			InitializeListViewForAdditionalObjects(root.Q<ListView>("AdditionalObjectsListView"));
			InitializePathTextField(root.Q<TextField>("FolderPathTextField"));
			InitializeSelectPathButton(root.Q<Button>("SelectPathButton"));
			InitializeBakeButton(root.Q<Button>("BakeButton"));
		}

		private void InitializeDefaultValues()
		{
			_cameraObject = Camera.main;
			_savePath = Application.dataPath;
		}

		private void InitializeResolutionField(Vector2IntField resolutionField)
		{
			resolutionField.value = _resolution;
			resolutionField.RegisterCallback<ChangeEvent<Vector2Int>>(evt =>
			{
				_resolution = evt.newValue;
			});
		}

		private void InitializeCameraField(ObjectField cameraField)
		{
			cameraField.value = _cameraObject;
			cameraField.RegisterCallback<ChangeEvent<Object>>((evt) =>
			{
				_cameraObject = evt.newValue as Camera;
			});
		}

		private void InitializeRootObjectField(ObjectField rootField)
		{
			rootField.RegisterCallback<ChangeEvent<Object>>((evt) =>
			{
				_rootObject = evt.newValue as Transform;
			});
		}

		private void InitializeSelectPathButton(Button pathSelectButton)
		{
			pathSelectButton.RegisterCallback<MouseUpEvent>(evt =>
			{
				_savePath = EditorUtility.OpenFolderPanel("Select Folder To Bake", Application.dataPath, "");
				_pathTextField.value = _savePath;
			});
		}

		private void InitializePathTextField(TextField textField)
		{
			_pathTextField = textField;
			textField.value = _savePath;
			textField.RegisterCallback<ChangeEvent<string>>((evt) =>
			{
				_savePath = evt.newValue;
			});
		}

		private void InitializeListViewForAdditionalObjects(ListView listView)
		{
			listView.itemsSource = _additionalObjects;
			listView.makeItem = () =>
			{
				return new ObjectField() { objectType = typeof(Transform), allowSceneObjects = true };
			};
			listView.bindItem = (element, i) =>
			{
				((ObjectField)element).value = _additionalObjects[i];
				((ObjectField)element).RegisterValueChangedCallback((value) =>
				{
					_additionalObjects[i] = (Transform)value?.newValue;
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
						_additionalObjects.Add(gameObject.transform);
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

		private void BakeImages()
		{
			// Create texture for camera baker.
			Texture2D image = new(_resolution.x, _resolution.y);
			RenderTexture tempRT = new(_resolution.x, _resolution.y, 24, RenderTextureFormat.ARGB32);

			try
			{
				// Bake all required object into separated files.
				CameraBaker.SaveCameraRender(Path.Combine(_savePath, $"{_rootObject.name}.png"), _cameraObject, image, tempRT);

				// Bake additional objects
				foreach (Transform parent in _rootObject)
				{
					parent.gameObject.SetActive(false);
				}
				foreach (Transform additional in _additionalObjects)
				{
					additional.gameObject.SetActive(true);
					CameraBaker.SaveCameraRender(Path.Combine(_savePath, $"{additional.name}.png"), _cameraObject, image, tempRT);
					additional.gameObject.SetActive(false);
				}
				foreach (Transform parent in _rootObject)
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