using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;
using UnityEngine.UI;

public class CraftingListUI : MonoBehaviour
{
    public Transform content;
    public GameObject buttonPrefab;
    public CraftRecipe[] recipeList;

    private void Start()
    {
        foreach (var recipe in recipeList)
        {
            GameObject btn = Instantiate(buttonPrefab, content);
            btn.GetComponent<CraftingListButton>().Setup(recipe);
        }
    }
}