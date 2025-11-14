using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : IAIState
{
    private readonly AIContext _ctx;
    private readonly AIStateMachine _fsm;

    public ChaseState(AIContext ctx, AIStateMachine fsm)
    {
        _ctx = ctx;
        _fsm = fsm;
    }

    public void OnEnter()
    {
        if (_ctx.Animator != null)
        {
            _ctx.Animator.SetBool("IsMove", true);
        }

        if (_ctx.Agent != null)
            _ctx.Agent.isStopped = false;
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
        if (_ctx.CurrentTarget == null)
        {
            Debug.Log("[Chase] CurrentTarget == null → Idle");
            _fsm.ChangeState(new IdleState(_ctx, _fsm));
            return;
        }

        float dist = _ctx.DistanceToTarget;

        Debug.Log($"[Chase] dist={dist:F2}, Aggro={_ctx.AggroRange}, Attack={_ctx.AttackRange}, GiveUp={_ctx.GiveUpRange}");

        // 타겟이 너무 멀어지면 포기
        if (dist > _ctx.GiveUpRange)
        {
            Debug.Log("[Chase] GiveUpRange 초과 → 타겟 포기 + Idle");
            _ctx.CurrentTarget = null;
            _fsm.ChangeState(new IdleState(_ctx, _fsm));
            return;
        }

        // 공격 거리면 AttackState로 전환
        if (dist <= _ctx.AttackRange)
        {
            Debug.Log("[Chase] AttackRange 진입 → AttackState");
            _fsm.ChangeState(new AttackState(_ctx, _fsm));
            return;
        }

        // 계속 추적
        if (_ctx.Agent != null)
            _ctx.Agent.SetDestination(_ctx.CurrentTarget.position);

        // 타겟 쪽 바라보기
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
    }

}