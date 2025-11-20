using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform cameraBox; // 플레이어 머리 높이
    [SerializeField] private Transform camTr;     // 메인 카메라
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float mouseSensitivity = 1.5f; // 감도 낮춤
    [SerializeField] private float cameraFollowSpeed = 10f;


    private Vector2 moveInput;
    private Vector2 lookInput;
    private float lookUpDownNum = 0f;

    public bool canLook;

    private void Start()
    {
        canLook = true;
        ToggleCursor();
    }

    private void Update()
    {
        HandleLook();
    }

    private void FixedUpdate()
    {
        HandleMove();
    }

    private void LateUpdate()
    {
        if(canLook)
        {
            HandleCameraFollow();
        }
    }

    #region Movement
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (IsInventoryOpen()) return;
        if (context.performed)
        {
            animator.SetTrigger("Jump");
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void HandleMove()
    {
        Vector3 camForward = camTr.forward;
        Vector3 camRight = camTr.right;

        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = camRight * moveInput.x + camForward * moveInput.y;

        animator.SetFloat("MoveSpeed", move.magnitude);

        Vector3 targetPos = rb.position + move * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(targetPos);
    }
    #endregion

    #region Look
    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    private void HandleLook()
    {
        Vector2 mouseDelta = lookInput * mouseSensitivity;

        // 플레이어 위/아래 회전
        lookUpDownNum -= mouseDelta.y;
        lookUpDownNum = Mathf.Clamp(lookUpDownNum, -60f, 60f);
        cameraBox.localRotation = Quaternion.Euler(lookUpDownNum, 0f, 0f);

        // 플레이어 좌/우 회전
        transform.Rotate(Vector3.up * mouseDelta.x);
    }
    #endregion

    #region Camera
    private void HandleCameraFollow()
    {
        // 카메라 위치를 CameraBox 기준으로 Lerp로 부드럽게 이동
        camTr.position = Vector3.Lerp(camTr.position, cameraBox.position, cameraFollowSpeed * Time.deltaTime);

        // 카메라 회전은 CameraBox 로컬 회전 그대로
        camTr.rotation = cameraBox.rotation;
    }
    #endregion

    public void OnToggleInventory(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (UIInventory.Instance != null)
        {
            ToggleCursor();
            canLook = !canLook;
            UIInventory.Instance.Toggle();
        }

        else
            Debug.LogError("UIInventory.Instance가 존재하지 않습니다!");
    }
    private bool IsInventoryOpen()
    {
        return UIInventory.Instance != null && UIInventory.Instance.IsOpen();
    }

    void ToggleCursor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
