using System;

namespace ARMenu.Scripts.Runtime.Data
{
	[Serializable]
	public class NutritionalInfo
	{
		public float calories;
		public float carbohydrates;
		public float fats;
		public float proteins;

		public override string ToString()
		{
			return $"{calories} Cal. | {carbohydrates} Carbs | {fats} Fat | {proteins} Protein";
		}
	}
}