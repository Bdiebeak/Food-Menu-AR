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
		private const string ResolutionField = "ResolutionField";
		private const string CameraField = "CameraField";
		private const string RootObjectField = "RootObjectField";
		private const string AdditionalObjectsListView = "AdditionalObjectsListView";
		private const string FolderPathField = "FolderPathTextField";
		private const string SelectPathButton = "SelectPathButton";
		private const string BakeButton = "BakeButton";
		private const string AdditionalObjectField = "ObjectField";

		public VisualTreeAsset ui;
		public VisualTreeAsset additionalElement;

		private DishBaker _dishBaker = new();

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

			InitializeDefaultValues();
			InitializeResolutionField(root.Q<Vector2IntField>(ResolutionField));
			InitializeCameraField(root.Q<ObjectField>(CameraField));
			InitializeRootObjectField(root.Q<ObjectField>(RootObjectField));
			InitializeListViewForAdditionalObjects(root.Q<ListView>(AdditionalObjectsListView));
			InitializePathTextField(root.Q<TextField>(FolderPathField));
			InitializeSelectPathButton(root.Q<Button>(SelectPathButton));
			InitializeBakeButton(root.Q<Button>(BakeButton));
		}

		private void InitializeDefaultValues()
		{
			_dishBaker.cameraObject = Camera.main;
			_dishBaker.savePath = Application.dataPath;
		}

		private void InitializeResolutionField(Vector2IntField resolutionField)
		{
			resolutionField.SetBinding(nameof(Vector2IntField.value), new DataBinding()
			{
				dataSource = _dishBaker,
				dataSourcePath = new PropertyPath(nameof(DishBaker.resolution)),
				bindingMode = BindingMode.TwoWay
			});
		}

		private void InitializeCameraField(ObjectField cameraField)
		{
			cameraField.SetBinding(nameof(ObjectField.value), new DataBinding()
			{
				dataSource = _dishBaker,
				dataSourcePath = new PropertyPath(nameof(DishBaker.cameraObject)),
				bindingMode = BindingMode.TwoWay
			});
		}

		private void InitializeRootObjectField(ObjectField rootField)
		{
			rootField.SetBinding(nameof(ObjectField.value), new DataBinding()
			{
				dataSource = _dishBaker,
				dataSourcePath = new PropertyPath(nameof(DishBaker.rootObject)),
				bindingMode = BindingMode.TwoWay
			});
		}

		private void InitializeSelectPathButton(Button pathSelectButton)
		{
			pathSelectButton.RegisterCallback<MouseUpEvent>(evt =>
			{
				_dishBaker.savePath = EditorUtility.OpenFolderPanel("Select Folder To Bake", Application.dataPath, "");
			});
		}

		private void InitializePathTextField(TextField textField)
		{
			textField.SetBinding(nameof(TextField.value), new DataBinding()
			{
				dataSource = _dishBaker,
				dataSourcePath = new PropertyPath(nameof(DishBaker.savePath)),
				bindingMode = BindingMode.TwoWay
			});
		}

		private void InitializeListViewForAdditionalObjects(ListView listView)
		{
			listView.itemTemplate = additionalElement;
			listView.itemsSource = _dishBaker.additionalObjects;
			listView.bindItem = (element, i) =>
			{
				ObjectField objectField = element.Q<ObjectField>(AdditionalObjectField);
				objectField.objectType = typeof(Transform);
				objectField.value = _dishBaker.additionalObjects[i];
				objectField.RegisterValueChangedCallback(value =>
				{
					_dishBaker.additionalObjects[i] = (Transform)value?.newValue;
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
						_dishBaker.additionalObjects.Add(gameObject.transform);
					}
				}
				listView.RefreshItems();
			});
		}

		private void InitializeBakeButton(Button bakeButton)
		{
			bakeButton.RegisterCallback<MouseUpEvent>(evt =>
			{
				_dishBaker.BakeImages();
			});
		}
	}
}