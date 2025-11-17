using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DropItem
{
    public ItemData item;  // 드랍될 프리팹
    public float weight = 1f;  // 확률 가중치
}

[CreateAssetMenu(menuName = "Game/Drop Table")]
public class DropTable : ScriptableObject
{
    public DropItem[] items;

    public GameObject GetRandomDrop()
    {
        if (items == null || items.Length == 0)
            return null;

        float total = 0f;
        foreach (var d in items)
            total += d.weight;

        float r = Random.value * total;

        foreach (var d in items)
        {
            if (r < d.weight)
                return d.item.dropPrefab;
            r -= d.weight;
        }

        return null;
    }
}
