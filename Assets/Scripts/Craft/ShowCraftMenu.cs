using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShowCraftMenu : MonoBehaviour
{
    public GameObject craftMenuPanel;

    public void OnCraftMenu(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        bool active = craftMenuPanel.activeSelf;
        craftMenuPanel.SetActive(!active);
    }
}
