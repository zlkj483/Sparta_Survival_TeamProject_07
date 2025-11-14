using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : IAIState
{
    private readonly AIContext _ctx;
    private readonly AIStateMachine _fsm;

    private float _attackCooldown = 2f; // 예시
    private float _attackTimer;

    public AttackState(AIContext ctx, AIStateMachine fsm)
    {
        _ctx = ctx;
        _fsm = fsm;
    }

    public void OnEnter()
    {
        _attackTimer = 0f;

        if (_ctx.Agent != null)
        {
            _ctx.Agent.isStopped = true;
        }

        if (_ctx.Animator != null)
        {
            _ctx.Animator.SetBool("IsMove", false);
            _ctx.Animator.SetTrigger("Attack");
        }

        // 여기서 실제 데미지 적용은 AnimationEvent나 Timer로
        // 나중에 전투 시스템 붙일 때 구현
    }

    public void OnExit()
    {
        if (_ctx.Agent != null)
            _ctx.Agent.isStopped = false;
    }

    public void Tick()
    {
        if (_ctx.CurrentTarget == null)
        {
            _fsm.ChangeState(new IdleState(_ctx, _fsm));
            return;
        }

        float dist = _ctx.DistanceToTarget;

        // 공격 거리 벗어나면 Chase로
        if (dist > _ctx.AttackRange * 1.2f) // 약간 여유
        {
            _fsm.ChangeState(new ChaseState(_ctx, _fsm));
            return;
        }

        // 타겟 바라보기
        Vector3 dir = _ctx.CurrentTarget.position - _ctx.SelfTransform.position;
        dir.y = 0;
        if (dir != Vector3.zero)
        {
            _ctx.SelfTransform.rotation = Quaternion.Slerp(
                _ctx.SelfTransform.rotation,
                Quaternion.LookRotation(dir),
                Time.deltaTime * 10f
            );
        }

        _attackTimer += Time.deltaTime;

        if (_attackTimer >= _attackCooldown)
        {
            _attackTimer = 0f;
            if (_ctx.Animator != null)
                _ctx.Animator.SetTrigger("Attack");

            // 여기에서 IAttackAction 사용해서 데미지 적용 가능 (나중에 확장)
        }
    }
}