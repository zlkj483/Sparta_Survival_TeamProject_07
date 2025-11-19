using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ItemType
{
    Resource,// 자원
    Equipable,// 장착 가능
    Consumable// 소비 가능
}
public enum ConsumableType
{
    Health,// 체력
    Thirst,// 갈증
    Hunger,// 배고픔
}

[System.Serializable]
public class ItemDataConsumable
{
    public ConsumableType type;// 소비 아이템 종류
    public float value;// 소비 아이템 효과 값
}
[CreateAssetMenu(fileName = "Item", menuName = "New Item")]

public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string displayName;// 아이템 이름
    public string description;// 아이템 설명
    public ItemType type;// 아이템 종류
    public Sprite icon;// 아이템 아이콘
    public GameObject dropPrefab;// 아이템 드롭 프리팹

    [Header("Stacking")]
    public bool canStack;// 아이템 중첩 가능 여부
    public int maxStackAmount;// 최대 중첩 개수

    [Header("Equip")]
    public GameObject equipPrefab;// 장착 프리팹

    [Header("Consumable")]
    public ItemDataConsumable[] consumables;// 소비 아이템 효과

}
