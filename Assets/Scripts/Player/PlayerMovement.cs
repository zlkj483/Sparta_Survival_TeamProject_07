using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    //Ï∫êÎ¶≠?Ñ∞Í∞? ???ÏßÅÏù¥Í∏? ?úÑ?ïú Î¨ºÎ¶¨ Ïª¥Ìè¨?Ñå?ä∏ = Î¶¨Ï???ìúÎ∞îÎîî
    [SerializeField]
    private Rigidbody rb;
    //?ù¥?èôÍ∞íÏùÑ ????û•?ïòÍ∏? ?úÑ?ïú 2Ï∞®Ïõê Î≤°ÌÑ∞
    private Vector2 moveInput;
    //?ù¥?èô?ï† ?Üç?èÑ
    public float moveSpeed = 5.0f;
    //?†ê?îÑ?ï† ?åå?õå
    public float jumpForce = 5.0f;
    private PlayerAttack attack;
    void Awake()
    {
        attack = GetComponent<PlayerAttack>();
    }
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

    //input system?óê?Ñú ?ò∏Ï∂úÌïò?äî ?ï®?àò
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }



    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            animator.SetTrigger("Jump");
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

        animator.SetFloat("MoveSpeed", move.magnitude);

        // Î¨ºÎ¶¨ ?ù¥?èô??? MovePosition?úºÎ°? Î≥?Í≤?
        Vector3 targetPos = rb.position + move * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(targetPos);
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (attack != null)
                attack.TryAttack();
        }
    }
}
