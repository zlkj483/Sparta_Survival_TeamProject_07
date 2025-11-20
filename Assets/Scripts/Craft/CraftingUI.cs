using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;
using UnityEngine.UI;

public class CraftingUI : MonoBehaviour
{
    public static CraftingUI Instance;

    [Header("재료 아이템 연결")]
    public ItemData woodItem;
    public ItemData stoneItem;

    [Header("재료 표시 (왼쪽)")]
    public TextMeshProUGUI woodText;
    public TextMeshProUGUI stoneText;

    [Header("제작 가능 리스트 (오른쪽)")]
    public Transform craftListContent;
    public GameObject craftItemButtonPrefab;

    [Header("상세 정보 (하단)")]
    public Image itemImage;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemDesc;
    public Button craftButton;

    [Header("전체 레시피")]
    public CraftRecipe[] allRecipes;

    private CraftRecipe selectedRecipe;

    private List<CraftItemButton> craftButtons = new List<CraftItemButton>();
    private bool isInitialized = false;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false); // 처음엔 닫힘
    }

    private void OnEnable()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        RefreshMyMaterialCount();

        if (!isInitialized)
        {
            InitializeCraftButtons();
            isInitialized = true;
        }

        RefreshButtonStates();
        ClearDetail();
    }

    private void RefreshMyMaterialCount()
    {
        if (UIInventory.Instance == null)
        {
            woodText.text = "Wood : 0";
            stoneText.text = "Stone : 0";
            return;
        }

        int wood = UIInventory.Instance.GetItemCount(woodItem);
        int stone = UIInventory.Instance.GetItemCount(stoneItem);

        woodText.text = "Wood : " + wood;
        stoneText.text = "Stone : " + stone;
    }

    private void InitializeCraftButtons()
    {
        foreach (var recipe in allRecipes)
        {
            var btnObj = Instantiate(craftItemButtonPrefab, craftListContent);
            var btn = btnObj.GetComponent<CraftItemButton>();

            btn.Setup(recipe);  // Setup 호출
            craftButtons.Add(btn);
        }
    }

    private void RefreshButtonStates()
    {
        foreach (var btn in craftButtons)
        {
            bool craftable = CanCraft(btn.recipe);

            btn.GetComponent<Button>().interactable = craftable;
        }
    }

    private bool CanCraft(CraftRecipe recipe)
    {
        foreach (var m in recipe.materials)
        {
            int have = UIInventory.Instance.GetItemCount(m.item);
            if (have < m.amount)
                return false;
        }
        return true;
    }

    public void ShowDetail(CraftRecipe recipe)
    {
        selectedRecipe = recipe;

        itemName.text = recipe.resultItem.displayName;
        itemDesc.text = recipe.description;

        craftButton.onClick.RemoveAllListeners();
        craftButton.onClick.AddListener(CraftSelectedItem);
    }

    private void CraftSelectedItem()
    {
        if (!CanCraft(selectedRecipe)) return;

        foreach (var m in selectedRecipe.materials)
            UIInventory.Instance.RemoveItem(m.item, m.amount);

        UIInventory.Instance.AddItem(selectedRecipe.resultItem);

        RefreshUI();
    }

    private void ClearDetail()
    {
        itemName.text = "";
        itemDesc.text = "";
    }
}