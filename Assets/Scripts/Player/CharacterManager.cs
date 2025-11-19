using UnityEngine;
using System;


public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance { get; private set; }
    public UIInventory inventory;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
}
