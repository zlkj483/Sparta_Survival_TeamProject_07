using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CraftingListButton : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI nameText;

    private CraftRecipe recipe;

    public void Setup(CraftRecipe recipe)
    {
        this.recipe = recipe;

        iconImage.sprite = recipe.resultItem.icon;
        nameText.text = recipe.resultItem.displayName;

        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(OpenDetail);
    }

    private void OpenDetail()
    {
        CraftingDetailUI.Instance.Show(recipe);
    }
}