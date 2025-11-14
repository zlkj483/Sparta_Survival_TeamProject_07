using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : IAIState
{
    private readonly AIContext _ctx;
    private readonly AIStateMachine _fsm;

    private float _waitTimer;
    private bool _waiting;

    public PatrolState(AIContext ctx, AIStateMachine fsm)
    {
        _ctx = ctx;
        _fsm = fsm;
    }

    public void OnEnter()
    {
        _waiting = false;
        _waitTimer = 0f;
        MoveToNextPoint();

        if (_ctx.Animator != null)
        {
            _ctx.Animator.SetBool("IsMove", true);
        }
    }

    public void OnExit()
    {
        if (_ctx.Agent != null)
            _ctx.Agent.isStopped = false;
    }

    public void Tick()
    {
        // 1) 타겟 탐색 우선
        if (TryFindTarget())
        {
            _fsm.ChangeState(new ChaseState(_ctx, _fsm));
            return;
        }

        // 패트롤 포인트가 없으면 Idle로
        if (_ctx.PatrolPoints == null || _ctx.PatrolPoints.Length == 0)
        {
            _fsm.ChangeState(new IdleState(_ctx, _fsm));
            return;
        }

        if (_waiting)
        {
            _waitTimer += Time.deltaTime;
            if (_waitTimer >= _ctx.PatrolWaitTime)
            {
                _waiting = false;
                MoveToNextPoint();
            }
            return;
        }

        if (_ctx.Agent != null && !_ctx.Agent.pathPending)
        {
            if (_ctx.Agent.remainingDistance <= _ctx.PatrolPointReachThreshold)
            {
                _waiting = true;
                _waitTimer = 0f;
                if (_ctx.Animator != null)
                    _ctx.Animator.SetBool("IsMove", false);
            }
        }
    }

    private void MoveToNextPoint()
    {
        if (_ctx.PatrolPoints == null || _ctx.PatrolPoints.Length == 0)
            return;

        if (_ctx.Agent == null)
            return;

        _ctx.Agent.isStopped = false;
        _ctx.Agent.SetDestination(_ctx.PatrolPoints[_ctx.PatrolIndex]);

        _ctx.PatrolIndex = (_ctx.PatrolIndex + 1) % _ctx.PatrolPoints.Length;

        if (_ctx.Animator != null)
            _ctx.Animator.SetBool("IsMove", true);
    }

    private bool TryFindTarget()
    {
        Collider[] hits = Physics.OverlapSphere(_ctx.SelfTransform.position, _ctx.AggroRange);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                _ctx.CurrentTarget = hit.transform;
                return true;
            }
        }
        return false;
    }
}

