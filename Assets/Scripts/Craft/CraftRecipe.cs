using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewCraftRecipe", menuName = "Crafting/Recipe")]
public class CraftRecipe : ScriptableObject
{
    [Header("결과 아이템")]
    public ItemData resultItem;
    public int resultAmount = 1;

    [System.Serializable]
    public class Material
    {
        public ItemData item;
        public int amount;
    }

    [Header("필요 재료들")]
    public Material[] materials;

    [TextArea]
    public string description;
}