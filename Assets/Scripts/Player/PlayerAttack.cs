using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public PlayerCondition playerCondition;
    public float staminaCost = 10f;
    public float attackDamage = 10f;
    public float attackRange = 3f;
    public LayerMask targetLayer;

    private Animator animator;

    private bool isAttacking = false;
    public float attackCooldown = 0.7f;

    private void Awake()
    {
        if (playerCondition == null)
            playerCondition = GetComponent<PlayerCondition>();

        animator = GetComponent<Animator>();
    }

    public void TryAttack()
    {
        if (isAttacking) return; // 공격 중이면 무시!

        if (!playerCondition.UseStamina(staminaCost))
        {
            Debug.Log("Stamina x!");
            return;
        }

        // 공격 시작
        isAttacking = true;

        animator.SetTrigger("Attack");
        DealDamage();

        // 쿨타임 시작
        Invoke(nameof(ResetAttack), attackCooldown);
    }

    private void ResetAttack()
    {
        isAttacking = false;
    }
    public void DealDamage()
    {

        RaycastHit hit;

        // SphereCast: origin, radius, direction, out hit, range, layer
        float sphereRadius = 0.5f; // 공격 판정 반지름
        Vector3 rayOrigin = transform.position + Vector3.up * 1f; // 플레이어 허리~가슴 높이

        if (Physics.SphereCast(rayOrigin, sphereRadius, transform.forward, out hit, attackRange, targetLayer))
        {
            IDamagable monster = hit.collider.GetComponentInParent<IDamagable>();
            if (monster != null)
            {
                monster.TakePhysicalDamage(attackDamage);
            }
        }
    }

}
