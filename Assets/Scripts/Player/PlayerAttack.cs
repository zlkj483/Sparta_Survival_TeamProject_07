using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public PlayerCondition playerCondition;
    public float staminaCost = 10f;
    public float attackDamage = 10f;
    public float attackRange = 3f;
    public LayerMask targetLayer; // 몬스터 레이어 넣기

    private void Awake()
    {
        if (playerCondition == null)
            playerCondition = GetComponent<PlayerCondition>();
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
            return;
        }
        // 플레이어 앞 방향으로 Ray 발사
        Ray ray = new Ray(transform.position + Vector3.up, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, attackRange, targetLayer))
        {
            MonsterHealth monster = hit.collider.GetComponent<MonsterHealth>();
            if (monster != null)
            {
                monster.TakeDamage(attackDamage);
                Debug.Log("몬스터 타격!");
            }
        }
    }
}
