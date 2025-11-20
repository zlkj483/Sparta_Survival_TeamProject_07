using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftingSlot : MonoBehaviour
{
    [Header("데이터")]
    public ItemData item;
    public CraftingUI crafting;          // 가능하면 인스펙터에서 안 건드려도 되게 자동 셋업
    public Image icon;
    public TextMeshProUGUI quatityText;

    public int index;
    public bool equipped;
    public int quantity;
}
