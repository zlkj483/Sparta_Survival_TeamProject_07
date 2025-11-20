using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : IAIState
{
    private readonly AIContext _ctx;
    private readonly AIStateMachine _fsm;

    private const float EnemyBodyRadius = 0.8f; // 적 몸통 반경 (감)
    private const float PlayerBodyRadius = 0.8f; // 플레이어 반경 (감)

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
            if (_ctx.Data != null && _ctx.Data.isBoss)
                _fsm.ChangeState(new BossAttackState(_ctx, _fsm));
            else
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

        KeepDistanceFromPlayer();
    }


    private void KeepDistanceFromPlayer()
    {
        if (_ctx.CurrentTarget == null)
            return;

        Vector3 toPlayer = _ctx.CurrentTarget.position - _ctx.SelfTransform.position;
        float dist = toPlayer.magnitude;

        float minDist = EnemyBodyRadius + PlayerBodyRadius; // 이만큼은 떨어져 있어야 한다

        if (dist < 0.0001f)
            return;

        if (dist < minDist)
        {
            // 너무 가까우면 적을 살짝 뒤로 빼서 겹치지 않게
            float pushBack = minDist - dist;
            Vector3 dir = toPlayer.normalized;
            _ctx.SelfTransform.position -= dir * pushBack;
        }
    }

}