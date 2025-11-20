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

    private PlayerMovement movement;
    private PlayerAttack attack;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        attack = GetComponent<PlayerAttack>();
    }

    public void OnOpenBuildMenu(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        isBuildMenuOpen = !isBuildMenuOpen;

        buildMenuUI.SetActive(isBuildMenuOpen);
    }


    public void OnCrafting(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        bool open = !craftingUI.activeSelf;
        craftingUI.SetActive(open);

        // UI 켜지면 플레이어 움직임/공격 비활성
        movement.enabled = !open;
        attack.enabled = !open;
        Cursor.visible = open;
        Cursor.lockState = open ? CursorLockMode.None : CursorLockMode.Locked;

        if (open)
            CraftingUI.Instance.RefreshUI();
    }
}
