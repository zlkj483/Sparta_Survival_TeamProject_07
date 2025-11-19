using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ItemType
{
    Resource,
    Equipable,
    Consumable
}
public enum ConsumableType
{
    Health,
    Thirst,
    Hunger,
}
[CreateAssetMenu(fileName = "Item", menuName = "New Item")]

public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string displayName;
    public string description;
    public GameObject dropPrefab;

    [Header("Stacking")]
    public bool canStack;
    public int maxStackAmount;

    [Header("Equip")]
    public GameObject equipPrefab;
}
