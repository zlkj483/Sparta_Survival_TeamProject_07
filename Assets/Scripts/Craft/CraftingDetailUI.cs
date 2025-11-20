using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CraftingDetailUI : MonoBehaviour
{
    public static CraftingDetailUI Instance;

    [Header("UI Reference")]
    public Image previewImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;

    [Header("재료")]
    public TextMeshProUGUI woodText;
    public TextMeshProUGUI stoneText;

    [Header("Buttons")]
    public Button craftButton;
    public Button cancelButton;

    private CraftRecipe currentRecipe;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void Show(CraftRecipe recipe)
    {
        currentRecipe = recipe;
        gameObject.SetActive(true);

        // 기본 정보 표시
        previewImage.sprite = recipe.resultItem.icon;
        nameText.text = recipe.resultItem.displayName;
        descriptionText.text = recipe.description;

        RefreshMaterialUI();

        craftButton.onClick.RemoveAllListeners();
        craftButton.onClick.AddListener(Craft);

        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
    }

    private void RefreshMaterialUI()
    {
        // 기본값 (없을 수도 있으니까 초기값 0으로)
        int needWood = 0, needStone = 0;
        int haveWood = 0, haveStone = 0;

        foreach (var mat in currentRecipe.materials)
        {
            if (mat.item.displayName.Contains("Wood") || mat.item.displayName.Contains("나무"))
            {
                needWood = mat.amount;
                haveWood = UIInventory.Instance.GetItemCount(mat.item);
            }

            if (mat.item.displayName.Contains("Stone") || mat.item.displayName.Contains("돌"))
            {
                needStone = mat.amount;
                haveStone = UIInventory.Instance.GetItemCount(mat.item);
            }
        }

        woodText.text = $"{haveWood} / {needWood}";
        stoneText.text = $"{haveStone} / {needStone}";

        craftButton.interactable = CanCraft();
    }

    private bool CanCraft()
    {
        foreach (var mat in currentRecipe.materials)
        {
            int have = UIInventory.Instance.GetItemCount(mat.item);
            if (have < mat.amount)
                return false;
        }
        return true;
    }

    private void Craft()
    {
        if (!CanCraft())
            return;

        // 재료 제거
        foreach (var mat in currentRecipe.materials)
        {
            UIInventory.Instance.RemoveItem(mat.item, mat.amount);
        }

        // 아이템 지급
        UIInventory.Instance.AddItem(currentRecipe.resultItem);

        // 갱신
        RefreshMaterialUI();
    }
}