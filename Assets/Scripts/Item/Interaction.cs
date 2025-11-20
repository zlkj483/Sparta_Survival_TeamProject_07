using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class Interaction : MonoBehaviour
{
    private IInteractable curInteractable;
    public GameObject curInteractGameObject;

    public float interDistance = 3f;
    public LayerMask itemLayer;
    public TextMeshProUGUI promptText;

    public InputReader playerActions; // Inspector에서 연결!

    void OnEnable()
    {
        if (playerActions == null)
        {
            Debug.LogError("삐빅! InputReader가 Inspector에서 연결되지 않았습니다.");
            return;
        }

        playerActions.Player.Enable();
        playerActions.Player.Interact.performed += OnInteract;
    }

    void OnDisable()
    {
        if (playerActions == null) return;

        playerActions.Player.Interact.performed -= OnInteract;
        playerActions.Player.Disable();
    }
    private void OnInteract(InputAction.CallbackContext context)
    {
        if (curInteractable != null)
        {
            curInteractable.Interact();
        }
    }
    void Update()
    {
        CheckForItem();
    }

    void CheckForItem()
    {
        Camera mainCam = Camera.main;
        if (promptText == null || mainCam == null) return;

        RaycastHit rayHit;
        Vector3 origin = mainCam.transform.position;
        Vector3 direction = mainCam.transform.forward;

        // BoxCast 범위 좀 더 크게
        Vector3 boxExtent = new Vector3(1f, 1f, 1f);
        Quaternion orientation = mainCam.transform.rotation;

        // BoxCast 수행
        if (Physics.BoxCast(origin, boxExtent, direction, out rayHit, orientation, interDistance, itemLayer))
        {
            if (rayHit.collider.gameObject != curInteractGameObject)
            {
                curInteractGameObject = rayHit.collider.gameObject;
                curInteractable = rayHit.collider.GetComponent<IInteractable>();
                SetPromptText();
            }
        }
        else
        {
            curInteractGameObject = null;
            curInteractable = null;
            promptText.gameObject.SetActive(false);
        }
    }
    private void SetPromptText()
    {
        if (curInteractable != null && promptText != null)
        {
            promptText.gameObject.SetActive(true);
            promptText.text = curInteractable.GetInteractPrompt();
        }
        else
        {
            // BoxCast�� ���������� IInteractable�� ���� ���
            promptText.gameObject.SetActive(false);
        }
    }

    public void ViewOnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (curInteractable != null)
        {
            curInteractable.Interact();
        }
    }
}
