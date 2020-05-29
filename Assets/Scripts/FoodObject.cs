using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Food Object", menuName = "Scriptable Objects/Food Object")]
public class FoodObject : ScriptableObject
{
    public Sprite photo;
    public string name;
    public string kkal;
    public string fats;
    public string carb;
    public string protein;
}
