using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crafting : MonoBehaviour
{
    [Header("만들 아이템")]
    public ItemData itemToCraft;       // 작업대에서 만들 수 있는 아이템
    public UIInventory playerInventory; // 플레이어 인벤토리 참조

    [Header("UI")]
    public GameObject craftUI;         // 작업대 UI

    private bool playerInRange = false;

    private void Update()
    {
        // 플레이어가 작업대 범위 안에 있을 때만 U 표시
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

    public void OnCraftButtonClicked()
    {
        if (playerInventory != null && itemToCraft != null)
        {
            playerInventory.AddItem(itemToCraft);
        }
    }

    // 플레이어가 작업대 범위에 들어왔을 때
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("작업대에 접근했습니다. [E] 키를 눌러 제작 가능");
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
