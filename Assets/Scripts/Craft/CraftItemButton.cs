using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CraftItemButton : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI nameText;

    public CraftRecipe recipe;

    public void Setup(CraftRecipe r)
    {
        recipe = r;

        nameText.text = r.resultItem.displayName;
        icon.sprite = r.resultItem.icon;

        GetComponent<Button>().onClick.AddListener(() =>
        {
            CraftingUI.Instance.ShowDetail(recipe);
        });
    }
}