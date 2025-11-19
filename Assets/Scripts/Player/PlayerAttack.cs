using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public PlayerCondition playerCondition;
    public float staminaCost = 10f;
    public float attackDamage = 10f;
    public float attackRange = 3f;
    public LayerMask targetLayer; // 몬스터 레이어 넣기

    private Animator animator;

    private void Awake()
    {
        if (playerCondition == null)
            playerCondition = GetComponent<PlayerCondition>();

        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogWarning("Animator가 없습니다!");

    }
    public void TryAttack()
    {
        if (playerCondition == null)
        {
            Debug.LogWarning("PlayerCondition이 연결되어 있지 않습니다!");
            return;
        }

        // 스태미나 체크
        if (!playerCondition.UseStamina(staminaCost))
        {
            Debug.Log("스태미나 부족으로 공격 불가");
            animator.ResetTrigger("Attack");
            return;
        }
        animator.SetTrigger("Attack");

        // 플레이어 앞 방향으로 Ray 발사
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
