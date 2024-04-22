using UnityEngine;

namespace ARMenu.Scripts.Runtime.Data.ImageLibrary.Dishes
{
	[CreateAssetMenu(fileName = nameof(DishImageLibrary), menuName = "ScriptableObjects/" + nameof(DishImageLibrary))]
	public class DishImageLibrary : BaseImageLibrary<AssetReferenceDish> { }
}