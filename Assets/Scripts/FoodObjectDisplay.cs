using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FoodObjectDisplay : MonoBehaviour
{
    /// <summary>
    /// ScriptableObject текущего объекта еды.
    /// </summary>
    [SerializeField]
    private FoodObject foodObject;

    [Header("UI")]
    [SerializeField]
    private GameObject canvasInfoUI;

    [SerializeField]
    private Image foodImageUI;

    [SerializeField]
    private TextMeshProUGUI nameUI;

    [SerializeField]
    private TextMeshProUGUI kkalUI;

    [SerializeField]
    private TextMeshProUGUI fatsUI;

    [SerializeField]
    private TextMeshProUGUI carbUI;

    [SerializeField]
    private TextMeshProUGUI proteinUI;

    private void OnMouseDown()
    {
        canvasInfoUI.SetActive(true);
        foodImageUI.sprite = foodObject.photo;
        nameUI.SetText(foodObject.name);
        kkalUI.SetText(foodObject.kkal);
        fatsUI.SetText(foodObject.fats);
        carbUI.SetText(foodObject.carb);
        proteinUI.SetText(foodObject.protein);
    }
}
