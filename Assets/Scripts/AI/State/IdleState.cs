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

        // 1) 타겟 탐색
        if (TryFindTarget())
        {
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

    private bool TryFindTarget()
    {
        // 여기서는 간단히 "주변에 Player 태그 찾기"로 구현
        // 나중에 센서/Perception 시스템으로 교체 가능

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
