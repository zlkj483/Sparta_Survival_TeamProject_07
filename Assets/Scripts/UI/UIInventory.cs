using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIInventory : MonoBehaviour
{
    public static UIInventory Instance { get; private set; }  // 전체적으로 널 값의 방어코드를 넣었습니다.

    [Header("Inventory Slots")]
    public ItemSlot[] slots;
    public Transform slotPanel;

    [Header("UI")]
    public GameObject inventoryWindow;

    [Header("Selected Item Info")]
    private ItemSlot selectedItem;
    private int selectedItemIndex;
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemStatName;
    public TextMeshProUGUI selectedItemStatValue;

    private int curEquipIndex;
    private PlayerCondition condition;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            this.enabled = false;
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        condition = FindObjectOfType<PlayerCondition>(); // 안전하게 찾기

        if (inventoryWindow != null)
            inventoryWindow.SetActive(false);

        InitializeSlots();
        ClearSelectedItemWindow();
    }

    private void InitializeSlots()
    {
        if (slotPanel == null)
        {
            Debug.LogError("UIInventory: slotPanel이 Inspector에서 연결되지 않았습니다!"); // 방어코드
            return;
        }

        int count = slotPanel.childCount;
        slots = new ItemSlot[count];

        for (int i = 0; i < count; i++)
        {
            ItemSlot slot = slotPanel.GetChild(i).GetComponent<ItemSlot>();

            if (slot == null)
            {
                Debug.LogError($"slotPanel의 {i}번째 오브젝트에 ItemSlot 컴포넌트가 없습니다."); // 방어코드
                continue;
            }

            slots[i] = slot;
            slot.index = i;
            slot.inventory = this;
            slot.Clear();
        }
    }

    public void Toggle()
    {
        if (inventoryWindow == null)
        {
            Debug.LogError("inventoryWindow가 Inspector에서 연결되지 않았습니다!"); // 방어코드
            return;
        }

        inventoryWindow.SetActive(!inventoryWindow.activeSelf);
    }

    public bool IsOpen()
    {
        return inventoryWindow != null && inventoryWindow.activeInHierarchy;
    }

    public void AddItem(ItemData data)
    {
        if (data == null)
        {
            Debug.LogError("AddItem 실패: data가 null입니다."); // 방어코드
            return;
        }

        // 스택 가능한 아이템 처리
        if (data.canStack)
        {
            ItemSlot slot = GetItemStack(data);
            if (slot != null)
            {
                slot.quantity++;
                UpdateUI();
                return;
            }
        }

        // 빈 슬롯 찾기
        ItemSlot empty = GetEmptySlot();

        if (empty != null)
        {
            empty.item = data;
            empty.quantity = 1;
            UpdateUI();
            return;
        }

        Debug.LogWarning("인벤토리가 가득 찼습니다. 아이템을 추가할 수 없습니다.");
    }

    public void UpdateUI()
    {
        if (slots == null)
        {
            Debug.LogError("UpdateUI 실패: slots 배열이 null입니다!"); // 방어코드
            return;
        }

        foreach (var slot in slots)
        {
            if (slot == null) continue;

            if (slot.item != null)
                slot.Set();
            else
                slot.Clear();
        }
    }

    private ItemSlot GetItemStack(ItemData data)
    {
        foreach (var slot in slots)
        {
            if (slot != null &&
                slot.item == data &&
                slot.quantity < data.maxStackAmount)
                return slot;
        }
        return null;
    }

    private ItemSlot GetEmptySlot()
    {
        foreach (var slot in slots)
        {
            if (slot != null && slot.item == null)
                return slot;
        }
        return null;
    }

    public void SelectItem(int index)
    {
        if (slots == null || index < 0 || index >= slots.Length)
            return;

        if (slots[index].item == null)
            return;

        selectedItem = slots[index];
        selectedItemIndex = index;

        selectedItemName.text = selectedItem.item.displayName;

        selectedItemStatName.text = "";
        selectedItemStatValue.text = "";

        foreach (var c in selectedItem.item.consumables)
        {
            selectedItemStatName.text += c.type + "\n";
            selectedItemStatValue.text += c.value + "\n";
        }
    }

    private void ClearSelectedItemWindow()
    {
        selectedItem = null;
        selectedItemName.text = "";
        selectedItemStatName.text = "";
        selectedItemStatValue.text = "";
    }

    public void OnUseButton()
    {
        if (selectedItem == null || selectedItem.item == null)
            return;

        if (selectedItem.item.type != ItemType.Consumable)
            return;

        foreach (var c in selectedItem.item.consumables)
        {
            switch (c.type)
            {
                case ConsumableType.Health: condition.Heal(c.value); break;
                case ConsumableType.Hunger: condition.Eat(c.value); break;
                case ConsumableType.Thirst: condition.Eat(c.value); break;
            }
        }
    }

    public int GetItemCount(ItemData targetItemData)
    {
        if (targetItemData == null)
            return 0;

        int total = 0;

        foreach (var slot in slots)
        {
            if (slot != null &&
                slot.item == targetItemData)
                total += slot.quantity;
        }

        return total;
    }

    public int QuestItemCount(string targetItemName)
    {
        if (string.IsNullOrEmpty(targetItemName))
            return 0;

        int total = 0;

        foreach (var slot in slots)
        {
            if (slot != null &&
                slot.item != null &&
                slot.item.displayName == targetItemName)
                total += slot.quantity;
        }

        return total;
    }

    public void RemoveItem(ItemData item, int amount)
    {
        if (item == null || amount <= 0)
            return;

        int remaining = amount;

        foreach (var slot in slots)
        {
            if (slot != null && slot.item == item)
            {
                int remove = Mathf.Min(slot.quantity, remaining);

                slot.quantity -= remove;
                remaining -= remove;

                if (slot.quantity <= 0)
                    slot.Clear();

                if (remaining <= 0)
                    break;
            }
        }

        UpdateUI();
    }
}
