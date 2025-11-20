using UnityEngine;
using UnityEngine.InputSystem;

public class Equipment : MonoBehaviour
{
    public Equip curEquip;
    public Transform equipParent;

    private PlayerMovement playerMovement;
    private PlayerCondition playerCondition;

    public float attackDamage = 10f;
    public float attackRange = 3f;

    private Animator animator;

    private bool isAttacking = false;
    public float attackCooldown = 0.7f;

    [SerializeField] private GameObject defaultHandPrefab;

    private void Awake()
    {
        if (playerCondition == null)
            playerCondition = GetComponent<PlayerCondition>();

        animator = GetComponent<Animator>();
    }

    void Start()
    {
        playerCondition = GetComponent<PlayerCondition>();
        playerMovement = GetComponent<PlayerMovement>();

        // 시작할 때 맨손 장착 (있으면)
        if (defaultHandPrefab != null && curEquip == null)
        {
            curEquip = Instantiate(defaultHandPrefab, equipParent).GetComponent<Equip>();
        }
    }

    public void EquipNew(ItemData data)
    {
        UnEquip();

        // data == null 이면 맨손 장착
        if (data == null)
        {
            if (defaultHandPrefab != null)
            {
                curEquip = Instantiate(defaultHandPrefab, equipParent).GetComponent<Equip>();
            }
            return;
        }

        if (data.equipPrefab == null)
            return;

        curEquip = Instantiate(data.equipPrefab, equipParent).GetComponent<Equip>();
    }

    public void UnEquip()
    {
        if (curEquip != null)
        {
            Destroy(curEquip.gameObject);
            curEquip = null;
        }
    }

    public void OnAttackInput(InputAction.CallbackContext context)
    {
        // 🔹 조건 상관없이 먼저 로그 찍어보기
        Debug.Log($"[Equipment] OnAttackInput 호출됨 - phase={context.phase}, curEquip={(curEquip ? curEquip.name : "null")}, playerMovement={(playerMovement ? "OK" : "null")}");

        // 클릭 입력이 'Performed'일 때만 처리
        if (context.phase != InputActionPhase.Performed)
            return;

        // 상태 체크 로그
        if (curEquip == null)
            Debug.Log("[Equipment] curEquip == null (장착된 장비 없음)");

        if (playerMovement == null)
            Debug.Log("[Equipment] playerMovement == null");

        if (playerMovement != null)
            Debug.Log($"[Equipment] playerMovement.canLook = {playerMovement.canLook}");

        // 실제 조건 체크
        if (curEquip != null && playerMovement != null && playerMovement.canLook)
        {
            Debug.Log("[Equipment] 조건 통과 → TryAttack 호출");
            TryAttack();
        }
        else
        {
            Debug.Log("[Equipment] 조건 불충족으로 TryAttack 미호출");
        }
    }


    public void TryAttack()
    {
        if (isAttacking) return; // 공격 중이면 무시!



        // 공격 시작
        isAttacking = true;

        animator.SetTrigger("Attack");
        curEquip.OnAttackInput();

        // 쿨타임 시작
        Invoke(nameof(ResetAttack), attackCooldown);
    }

    private void ResetAttack()
    {
        isAttacking = false;
    }
}
