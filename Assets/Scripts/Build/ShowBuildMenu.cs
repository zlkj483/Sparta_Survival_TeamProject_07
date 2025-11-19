using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShowBuildMenu : MonoBehaviour
{
    public GameObject buildMenuPanel;

    public void OnBuildMenu(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            bool active = buildMenuPanel.activeSelf;
            buildMenuPanel.SetActive(!active);
        }
    }
}
