using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FoodObjectDisplay : MonoBehaviour
{
    [SerializeField] private FoodInfo foodInfo;

    [Header("UI")]
    [SerializeField] private GameObject canvasInfoUI;
    [SerializeField] private Image foodImageUI;
    [SerializeField] private TextMeshProUGUI nameUI;
    [SerializeField] private TextMeshProUGUI kkalUI;
    [SerializeField] private TextMeshProUGUI fatsUI;
    [SerializeField] private TextMeshProUGUI carbUI;
    [SerializeField] private TextMeshProUGUI proteinUI;

    private void OnMouseDown()
    {
        canvasInfoUI.SetActive(true);
        
        foodImageUI.sprite = foodInfo.Photo;
        nameUI.SetText(foodInfo.FoodName);
        kkalUI.SetText(foodInfo.Calories);
        fatsUI.SetText(foodInfo.Fat);
        carbUI.SetText(foodInfo.Carbohydrates);
        proteinUI.SetText(foodInfo.Proteins);
    }
}
