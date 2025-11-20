using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public ItemData item;
    public UIInventory inventory;
    public Image icon;
    public TextMeshProUGUI quatityText;
    private Outline outline;

    public int index;
    public bool equipped;
    public int quantity;
    public Transform equipPoint; // public으로 받아 Inspector에서 지정 가능
    public PlayerEquip playerEquip;

    private void Awake()
    {
        outline = GetComponent<Outline>();
    }

    private void OnEnable()
    {
        outline.enabled = equipped;
    }

    public void Set()
    {
        //icon.gameObject.SetActive(true);
        //icon.sprite = item.icon;
        quatityText.text = quantity > 1 ? quantity.ToString() : string.Empty;

        if (outline != null)
        {
            outline.enabled = equipped;
        }
    }

    public void Clear()
    {
        item = null;
        //icon.gameObject.SetActive(false);

        if (quatityText != null)
            quatityText.text = string.Empty;

        if (icon != null)
            icon.gameObject.SetActive(false);
    }

    public void OnClickButton()
    {
        inventory.SelectItem(index);

        if (item != null && item.type == ItemType.Equipable && playerEquip != null && equipPoint != null)
        {
            playerEquip.EquipItem(item, equipPoint);
        }
    }
}
