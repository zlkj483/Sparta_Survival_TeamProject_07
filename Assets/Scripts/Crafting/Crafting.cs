using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RecipeIngredient
{
    public ItemData item;
    public int quantity;
}

[System.Serializable]
public class CraftingRecipe
{
    public ItemData resultItem;
    public int resultQuantity = 1;
    public List<RecipeIngredient> ingredients;
}

public class Crafting : MonoBehaviour
{
    [Header("작업대 레시피")]
    public List<CraftingRecipe> recipes;

    [Header("플레이어 인벤토리")]
    public UIInventory playerInventory;

    [Header("UI")]
    public GameObject craftUI;

    private bool playerInRange = false;
    private CraftingRecipe selectedRecipe;

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            ToggleCraftUI();
        }
    }

    void ToggleCraftUI()
    {
        if (craftUI != null)
            craftUI.SetActive(!craftUI.activeSelf);
    }

    // UI에서 선택한 레시피 설정
    public void SelectRecipe(int recipeIndex)
    {
        if (recipeIndex < 0 || recipeIndex >= recipes.Count) return;
        selectedRecipe = recipes[recipeIndex];
        // 필요하면 UI에서 재료 표시 업데이트
    }

    // 제작 버튼 클릭
    public void OnCraftButtonClicked()
    {
        if (selectedRecipe == null || playerInventory == null) return;

        // 재료 체크
        foreach (RecipeIngredient ingredient in selectedRecipe.ingredients)
        {
            //if (!playerInventory.HasItem(ingredient.item, ingredient.quantity))
            //{
            //    Debug.Log("재료가 부족합니다: " + ingredient.item.displayName);
            //    return;
            //}
        }

        // 재료 차감
        foreach (RecipeIngredient ingredient in selectedRecipe.ingredients)
        {
            playerInventory.RemoveItem(ingredient.item, ingredient.quantity);
        }

        // 아이템 제작
        for (int i = 0; i < selectedRecipe.resultQuantity; i++)
        {
            playerInventory.AddItem(selectedRecipe.resultItem);
        }

        Debug.Log(selectedRecipe.resultItem.displayName + " 제작 완료!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("작업대 접근: [E] 키로 제작 가능");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (craftUI != null)
                craftUI.SetActive(false);
        }
    }
}
