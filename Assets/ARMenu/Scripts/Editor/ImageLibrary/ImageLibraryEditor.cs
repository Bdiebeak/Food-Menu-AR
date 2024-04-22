using ARMenu.Scripts.Runtime.Data.ImageLibrary;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace ARMenu.Scripts.Editor.ImageLibrary
{
	[CustomEditor(typeof(BaseImageLibrary<>), true)]
	public class ImageLibraryEditor : UnityEditor.Editor
	{
		public VisualTreeAsset inspectorUXML;

		private ImageLibraryScriptableObject _imageLibrarySO;

		public override VisualElement CreateInspectorGUI()
		{
			VisualElement inspector = new();
			inspectorUXML.CloneTree(inspector);

			VisualElement defaultInspectorView = inspector.Q("DefaultInspectorView");
			InspectorElement.FillDefaultInspector(defaultInspectorView, serializedObject, this);

			InitializeDefaultValues();
			InitializeAutoFillButton(inspector.Q<Button>("AutoAppendButton"));
			InitializeValidateButton(inspector.Q<Button>("ValidateButton"));

			return inspector;
		}

		private void InitializeDefaultValues()
		{
			_imageLibrarySO = target as ImageLibraryScriptableObject;
		}

		private void InitializeAutoFillButton(Button fillButton)
		{
			fillButton.RegisterCallback<MouseUpEvent>(evt =>
			{
				_imageLibrarySO.AppendDataFromImageLibrary();
			});
		}

		private void InitializeValidateButton(Button validateButton)
		{
			validateButton.RegisterCallback<MouseUpEvent>(evt =>
			{
				_imageLibrarySO.Validate();
			});
		}
	}
}