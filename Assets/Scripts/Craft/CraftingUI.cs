using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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

    [Header("상세 정보 (오른쪽 하단)")]
    public Image itemImage;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemDesc;
    public TextMeshProUGUI itemTypeText;
    public Button craftButton;

    [Header("전체 레시피")]
    public CraftRecipe[] allRecipes;

    private CraftRecipe selectedRecipe;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false); // 처음에는 닫혀 있음
    }

    private void OnEnable()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        RefreshMyMaterialCount();
        RefreshCraftableList();
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

    private void RefreshCraftableList()
    {
        // 기존 버튼 모두 삭제
        foreach (Transform child in craftListContent)
            Destroy(child.gameObject);

        // 새로 생성
        foreach (var recipe in allRecipes)
        {
            if (CanCraft(recipe))
            {
                GameObject btn = Instantiate(craftItemButtonPrefab, craftListContent);
                btn.GetComponent<CraftingListButton>().Setup(recipe);
            }
        }
    }

    private bool CanCraft(CraftRecipe recipe)
    {
        foreach (var mat in recipe.materials)
        {
            int have = UIInventory.Instance.GetItemCount(mat.item);
            if (have < mat.amount)
                return false;
        }
        return true;
    }

    public void ShowDetail(CraftRecipe recipe)
    {
        selectedRecipe = recipe;

        itemImage.sprite = recipe.resultItem.dropPrefab.GetComponentInChildren<SpriteRenderer>()?.sprite;
        itemName.text = recipe.resultItem.displayName;
        itemDesc.text = recipe.description;
        itemTypeText.text = recipe.resultItem.type.ToString();

        craftButton.onClick.RemoveAllListeners();
        craftButton.onClick.AddListener(CraftSelectedItem);
    }

    private void ClearDetail()
    {
        itemName.text = "";
        itemDesc.text = "";
        itemTypeText.text = "";
    }

    private void CraftSelectedItem()
    {
        if (!CanCraft(selectedRecipe))
            return;

        // 재료 소모
        foreach (var m in selectedRecipe.materials)
            UIInventory.Instance.RemoveItem(m.item, m.amount);

        // 제작 아이템 지급
        UIInventory.Instance.AddItem(selectedRecipe.resultItem);

        RefreshUI();
    }
}
