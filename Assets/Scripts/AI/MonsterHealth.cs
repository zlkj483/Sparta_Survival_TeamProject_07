using UnityEngine;

public class MonsterHealth : MonoBehaviour, IDamagable
{
    public float currentHp;
    public EnemyAIController controller;

    private AIContext _ctx;
    private bool _isDead;

    private void Awake()
    {
        if (controller == null)
            controller = GetComponent<EnemyAIController>();
    }

    private void Start()
    {
        _ctx = controller.Context;          // EnemyAIController에 Context getter 하나
        currentHp = _ctx.Data.maxHp;
    }

    public void TakePhysicalDamage(float amount)
    {
        if (_isDead) return;

        currentHp -= amount;
        Debug.Log($"[MonsterHealth] {gameObject.name} HP = {currentHp}");

        if (currentHp <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        _isDead = true;
        _ctx.IsDead = true;

        if (_ctx.Agent != null)
        {
            _ctx.Agent.isStopped = true;
            _ctx.Agent.ResetPath();
        }

        var col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        if (_ctx.Animator != null)
        {
            _ctx.Animator.SetBool("IsMove", false);
            _ctx.Animator.SetTrigger("Die");
        }

        // 드랍 / Destroy 등은 너가 이미 만든 로직 쓰면 됨
    }
}
