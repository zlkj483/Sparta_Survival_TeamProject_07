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

        // 쿨타임 시작
        Invoke(nameof(ResetAttack), attackCooldown);
    }

    private void ResetAttack()
    {
        isAttacking = false;
    }
    public void DealDamage()
    {
        Ray ray = new Ray(transform.position + Vector3.up, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, attackRange, targetLayer))
        {
            IDamagable monster = hit.collider.GetComponent<IDamagable>();
            if (monster != null)
            {
                monster.TakePhysicalDamage(attackDamage);
                Debug.Log("몬스터 타격!");
            }
        }
    }

}
