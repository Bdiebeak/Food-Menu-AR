using UnityEngine;

[CreateAssetMenu(fileName = "Food Object", menuName = "Scriptable Objects/Food Object")]
public class FoodInfo : ScriptableObject
{
    [SerializeField] private Sprite photo;
    [SerializeField] private string foodName;
    [SerializeField] private string calories;
    [SerializeField] private string fat;
    [SerializeField] private string carbohydrates;
    [SerializeField] private string proteins;

    public Sprite Photo => photo;
    public string FoodName => foodName;
    public string Calories => calories;
    public string Fat => fat;
    public string Carbohydrates => carbohydrates;
    public string Proteins => proteins;
}
