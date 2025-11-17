using UnityEngine;

[RequireComponent(typeof(EnemyAIController))]
public class MonsterHealth : MonoBehaviour
{
    public EnemyAIController controller;   // Inspector에서 비워두면 자동 할당
    public float currentHp;

    private AIContext _ctx;
    private bool _isDead;

    private void Awake()
    {
        if (controller == null)
            controller = GetComponent<EnemyAIController>();
    }

    private void Start()
    {
        _ctx = controller.Context;  // EnemyAIController에서 public getter 하나 만들어주면 좋음

        if (_ctx != null && _ctx.Data != null)
            currentHp = _ctx.Data.maxHp;
        else
            currentHp = 100f;
    }

    // MonsterHealth 안에 테스트용 임시 코드
    private void Update()
    {
        // 테스트: K 키 누르면 10 데미지
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(10f);
        }
    }

    public void TakeDamage(float damage)
    {
        if (_isDead) return;

        currentHp -= damage;
        Debug.Log($"[MonsterHealth] {gameObject.name} HP = {currentHp}");

        if (currentHp <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        _isDead = true;
        if (_ctx != null)
            _ctx.IsDead = true;

        // AI/이동 정지
        if (_ctx?.Agent != null)
        {
            _ctx.Agent.isStopped = true;
            _ctx.Agent.ResetPath();
        }

        // Collider 비활성 (원하면)
        var col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // 죽음 애니메이션
        if (_ctx?.Animator != null)
        {
            _ctx.Animator.SetBool("IsMove", false);
            _ctx.Animator.SetTrigger("Die"); // AC_MonsterBase에 "Die" 트리거 필요
        }

        // 드랍 처리
        TryDropItem();

        // 일정 시간 후 오브젝트 제거
        float delay = _ctx?.Data != null ? _ctx.Data.destroyAfterDeath : 3f;
        Destroy(gameObject, delay);
    }

    private void TryDropItem()
    {
        if (_ctx == null || _ctx.Data == null || _ctx.Data.dropTable == null)
            return;

        GameObject dropPrefab = _ctx.Data.dropTable.GetRandomDrop();
        if (dropPrefab == null) return;

        Vector3 spawnPos = transform.position;
        Instantiate(dropPrefab, spawnPos, Quaternion.identity);
    }
}
