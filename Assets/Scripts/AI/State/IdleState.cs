using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IAIState
{
    private readonly AIContext _ctx;
    private readonly AIStateMachine _fsm;
    private float _idleTime;
    private readonly float _idleDuration;

    public IdleState(AIContext ctx, AIStateMachine fsm, float idleDuration = 1.5f)
    {
        _ctx = ctx;
        _fsm = fsm;
        _idleDuration = idleDuration;
    }

    public void OnEnter()
    {
        _idleTime = 0f;
        if (_ctx.Animator != null)
        {
            _ctx.Animator.SetBool("IsMove", false);
        }
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
        _idleTime += Time.deltaTime;

        if (_ctx.TryDetectTarget(out Transform target))
        {
            _ctx.CurrentTarget = target;
            _fsm.ChangeState(new ChaseState(_ctx, _fsm));
            return;
        }

        // 2) 대기 끝나면 Patrol 시작
        if (_ctx.PatrolPoints != null && _ctx.PatrolPoints.Length > 0 &&
            _idleTime >= _idleDuration)
        {
            _fsm.ChangeState(new PatrolState(_ctx, _fsm));
        }
    }

}
