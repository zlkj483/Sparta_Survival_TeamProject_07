using UnityEngine;
using System;


public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance;
    public UIInventory inventory;
    public Action addItem;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
}
