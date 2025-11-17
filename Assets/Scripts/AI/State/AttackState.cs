using UnityEngine;

public class AttackState : IAIState
{
    private readonly AIContext _ctx;
    private readonly AIStateMachine _fsm;

    private float _attackCooldownTimer;
    private bool _hasHitThisSwing;
    private float _prevNormalizedTime;

    // 애니 한 바퀴 기준(0~1)에서 언제 히트 낼지
    private const float HitStartTime = 0.3f;
    private const float HitEndTime = 0.5f;

    public AttackState(AIContext ctx, AIStateMachine fsm)
    {
        _ctx = ctx;
        _fsm = fsm;
    }

    public void OnEnter()
    {
        _attackCooldownTimer = 0f;
        _hasHitThisSwing = false;
        _prevNormalizedTime = 0f;

        if (_ctx.Animator != null)
        {
            _ctx.Animator.ResetTrigger("Attack");
            _ctx.Animator.SetTrigger("Attack");
        }
    }

    public void OnExit()
    {
        _hasHitThisSwing = false;
    }

    public void Tick()
    {
        if (_ctx.IsDead)
            return;

        if (_ctx.CurrentTarget == null)
        {
            _fsm.ChangeState(new IdleState(_ctx, _fsm));
            return;
        }

        float dist = _ctx.DistanceToTarget;

        // 사거리 밖 → Chase로 복귀
        if (dist > _ctx.AttackRange)
        {
            _fsm.ChangeState(new ChaseState(_ctx, _fsm));
            return;
        }

        var anim = _ctx.Animator;
        if (anim == null)
            return;

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // Attack 태그 상태가 아니면(전이 중 등) 쿨타임 기다리면서 다음 공격 준비
        if (!stateInfo.IsTag("Attack"))
        {
            _attackCooldownTimer += Time.deltaTime;
            if (_attackCooldownTimer >= _ctx.AttackCooldown)
            {
                _attackCooldownTimer = 0f;
                _hasHitThisSwing = false;
                anim.SetTrigger("Attack");
            }
            return;
        }

        // 0~1 구간으로 정규화
        float normalizedTime = stateInfo.normalizedTime % 1f;

        // 새 싸이클로 넘어갔으면 이번 스윙 히트 리셋
        if (normalizedTime < _prevNormalizedTime)
        {
            _hasHitThisSwing = false;
        }
        _prevNormalizedTime = normalizedTime;

        // 히트 타이밍 구간 진입 & 아직 안 때렸으면 → 데미지 적용
        if (!_hasHitThisSwing &&
            normalizedTime >= HitStartTime &&
            normalizedTime <= HitEndTime)
        {
            _hasHitThisSwing = true;
            ApplyDamageInFront();
        }

        // 쿨타임 타면서 다음 공격 준비
        _attackCooldownTimer += Time.deltaTime;
        if (_attackCooldownTimer >= _ctx.AttackCooldown && normalizedTime >= 0.9f)
        {
            _attackCooldownTimer = 0f;
            _hasHitThisSwing = false;
            anim.SetTrigger("Attack");
        }
    }

    private void ApplyDamageInFront()
    {
        if (_ctx.Data == null) return;

        float radius = _ctx.Data.attackRange * 0.8f;
        Vector3 origin = _ctx.SelfTransform.position + _ctx.SelfTransform.forward * (_ctx.Data.attackRange * 0.5f);

        Collider[] hits = Physics.OverlapSphere(
            origin,
            radius,
            _ctx.Data.targetLayer
        );

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out IDamagable damageable))
            {
                damageable.TakePhysicalDamage(_ctx.Data.baseDamage);
            }
        }
    }
}
