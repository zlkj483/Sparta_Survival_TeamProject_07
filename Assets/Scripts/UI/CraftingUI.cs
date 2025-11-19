using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CraftingUI : MonoBehaviour
{
    public static CraftingUI Instance { get; private set; }

    [Header("Selected Item")]
    private ItemSlot selectedItem;
    private int selectedItemIndex;
    public TextMeshProUGUI selectedItemName; // Item Name
    public TextMeshProUGUI selectedItemStatName; //StatValue
    public TextMeshProUGUI selectedItemStatValue; //StatInfo

    private Crafting Crafting;
}
