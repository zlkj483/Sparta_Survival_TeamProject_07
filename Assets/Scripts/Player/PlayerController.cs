using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public GameObject buildMenuUI;
    public GameObject inventoryUIRoot;

    private bool isBuildMenuOpen = false;

    private PlayerMovement movement;
    private PlayerAttack attack;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        attack = GetComponent<PlayerAttack>();
    }

    // 플레이어 컨트롤 비활성 / 활성 함수
    private void SetPlayerControl(bool enabled)
    {
        movement.enabled = enabled;
        attack.enabled = enabled;

        Cursor.visible = !enabled; // UI 열릴 때 커서 표시
        Cursor.lockState = enabled ? CursorLockMode.Locked : CursorLockMode.None; // UI 닫히고 열릴 시 커서 변경
    }


    public void OnOpenBuildMenu(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        isBuildMenuOpen = !isBuildMenuOpen;

        buildMenuUI.SetActive(isBuildMenuOpen);

        SetPlayerControl(!isBuildMenuOpen);
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        UIInventory.Instance.Toggle();

        bool isInventoryOpen = UIInventory.Instance.IsOpen();

        SetPlayerControl(!isInventoryOpen);
    }
}
