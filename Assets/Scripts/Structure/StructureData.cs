using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Structure", menuName = "New Structure")]

public class StructureData : ScriptableObject
{
    [Header("Info")]
    public string displayName;
    public int maxHp;
    public int hp;
    public int df;
    public Item dropItem;
    public int dropAmount;
    public int dropCount;
    public int damagePer;
}
