using Unity.Properties;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace ARMenu.Scripts.Editor.Baker
{
	public class DishBakerWindow : EditorWindow
	{
		public VisualTreeAsset windowUXML;
		public VisualTreeAsset additionalElementUXML;

		private readonly ObjectsBaker _objectsBaker = new();

		[MenuItem("Services/DishBaker")]
		public static void ShowWindow()
		{
			DishBakerWindow window = GetWindow<DishBakerWindow>();
			window.titleContent = new GUIContent(nameof(DishBakerWindow));
		}

		public void CreateGUI()
		{
			VisualElement root = rootVisualElement;
			windowUXML.CloneTree(root);

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
			_objectsBaker.cameraObject = Camera.main;
			_objectsBaker.savePath = Application.dataPath;
		}

		private void InitializeResolutionField(Vector2IntField resolutionField)
		{
			resolutionField.SetBinding(nameof(Vector2IntField.value), new DataBinding()
			{
				dataSource = _objectsBaker,
				dataSourcePath = new PropertyPath(nameof(ObjectsBaker.resolution)),
				bindingMode = BindingMode.TwoWay
			});
		}

		private void InitializeCameraField(ObjectField cameraField)
		{
			cameraField.SetBinding(nameof(ObjectField.value), new DataBinding()
			{
				dataSource = _objectsBaker,
				dataSourcePath = new PropertyPath(nameof(ObjectsBaker.cameraObject)),
				bindingMode = BindingMode.TwoWay
			});
		}

		private void InitializeRootObjectField(ObjectField rootField)
		{
			rootField.SetBinding(nameof(ObjectField.value), new DataBinding()
			{
				dataSource = _objectsBaker,
				dataSourcePath = new PropertyPath(nameof(ObjectsBaker.rootObject)),
				bindingMode = BindingMode.TwoWay
			});
		}

		private void InitializeSelectPathButton(Button pathSelectButton)
		{
			pathSelectButton.RegisterCallback<MouseUpEvent>(evt =>
			{
				_objectsBaker.savePath = EditorUtility.OpenFolderPanel("Select Folder To Bake", Application.dataPath, "");
			});
		}

		private void InitializePathTextField(TextField textField)
		{
			textField.SetBinding(nameof(TextField.value), new DataBinding()
			{
				dataSource = _objectsBaker,
				dataSourcePath = new PropertyPath(nameof(ObjectsBaker.savePath)),
				bindingMode = BindingMode.TwoWay
			});
		}

		private void InitializeListViewForAdditionalObjects(ListView listView)
		{
			listView.itemTemplate = additionalElementUXML;
			listView.itemsSource = _objectsBaker.additionalObjects;
			listView.bindItem = (element, i) =>
			{
				ObjectField objectField = element.Q<ObjectField>("ObjectField");
				objectField.objectType = typeof(Transform);
				objectField.value = _objectsBaker.additionalObjects[i];
				objectField.RegisterValueChangedCallback(value =>
				{
					_objectsBaker.additionalObjects[i] = (Transform)value?.newValue;
				});
			};

			// Add drag to list view.
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
						_objectsBaker.additionalObjects.Add(gameObject.transform);
					}
				}
				listView.RefreshItems();
			});
		}

		private void InitializeBakeButton(Button bakeButton)
		{
			bakeButton.RegisterCallback<MouseUpEvent>(evt =>
			{
				_objectsBaker.BakeImages();
			});
		}
	}
}