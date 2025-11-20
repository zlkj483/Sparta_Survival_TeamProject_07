using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public GameObject buildMenuUI;
    public GameObject inventoryUIRoot;
    public GameObject craftingUI;

    private bool isBuildMenuOpen = false;
    private bool isCraftingOpen = false;

    private PlayerMovement movement;
    private PlayerAttack attack;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        attack = GetComponent<PlayerAttack>();
    }
}
