using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    //캐릭터가 움직이기 위한 물리 컴포넌트 = 리지드바디
    [SerializeField]
    private Rigidbody rb;
    //이동값을 저장하기 위한 2차원 벡터
    private Vector2 moveInput;
    //이동할 속도
    public float moveSpeed = 5.0f;
    //점프할 파워
    public float jumpForce = 5.0f;
    void Start()
    {

    }


    void Update()
    {
        HandleLook();
    }

    private void FixedUpdate()
    {
        HandleMove();
    }

    public void Move()
    {
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);

        rb.MovePosition(this.transform.position + move * moveSpeed * Time.fixedDeltaTime);
    }

    //input system에서 호출하는 함수
    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //입력값을 moveInput에 저장
            moveInput = context.ReadValue<Vector2>();
        }
        else
        {
            //입력이 멈추면 0으로 초기화
            moveInput = Vector2.zero;
        }
    }



    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        else
        {

        }
    }

    private Vector2 lookInput;

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public Transform cameraBox;
    private float lookUpDownNum = 0;
    [SerializeField]
    private float mouseSensitivity = 2f;

    private void HandleLook()
    {
        Vector2 mouseDelta = lookInput * mouseSensitivity;

        lookUpDownNum -= mouseDelta.y;
        lookUpDownNum = Mathf.Clamp(lookUpDownNum, -90, 90);

        cameraBox.localRotation = Quaternion.Euler(lookUpDownNum, 0f, 0f);

        transform.Rotate(Vector3.up * mouseDelta.x);
    }

    [SerializeField]
    private Transform camTr;

    private void HandleMove()
    {
        Vector3 camForward = camTr.forward;
        Vector3 camRight = camTr.right;

        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = camRight * moveInput.x + camForward * moveInput.y;

        Vector3 handleVelocity = new Vector3(move.x * moveSpeed, rb.velocity.y, move.z * moveSpeed);

        rb.velocity = handleVelocity;
    }
}
