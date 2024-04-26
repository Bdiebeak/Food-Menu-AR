using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Build.DataBuilders;
using UnityEditor.XR.ARSubsystems;
using UnityEngine;

namespace ARMenu.Scripts.Editor.Addressables
{
	/// <summary>
	/// An additional build class for calling the necessary AR functions before building Addressables.
	/// </summary>
	[CreateAssetMenu(fileName = nameof(BuildScriptPackedModeWithXR), menuName = "Addressables/Content Builders/XR Build Script")]
	public class BuildScriptPackedModeWithXR : BuildScriptPackedMode
	{
		public override string Name => "Build Script With XR";

		protected override TResult BuildDataImplementation<TResult>(AddressablesDataBuilderInput builderInput)
		{
			ARBuildProcessor.PreprocessBuild(builderInput.Target);
			return base.BuildDataImplementation<TResult>(builderInput);
		}
	}
}