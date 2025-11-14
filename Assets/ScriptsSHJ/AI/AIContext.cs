using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// AI 상태들이 공유하는 데이터 컨테이너
/// </summary>
public class AIContext
{
    // 레퍼런스
    public Transform SelfTransform;
    public NavMeshAgent Agent;
    public Animator Animator;

    // 데이터
    public MonsterData Data;

    // 런타임 상태
    public Transform CurrentTarget;
    public int PatrolIndex;

    // 편의 프로퍼티
    public float AggroRange => Data != null ? Data.aggroRange : 10f;
    public float AttackRange => Data != null ? Data.attackRange : 2f;
    public float GiveUpRange => Data != null ? Data.giveUpRange : 20f;
    public float PatrolWaitTime => Data != null ? Data.patrolWaitTime : 2f;
    public float AttackCooldown => Data != null ? Data.attackCooldown : 2f;

    public Vector3[] PatrolPoints;
    public float PatrolPointReachThreshold = 0.5f; // 패트롤 포인트 도착 판정 거리

    public float DistanceToTarget
    {
        get
        {
            if (CurrentTarget == null || SelfTransform == null)
                return Mathf.Infinity;
            return Vector3.Distance(SelfTransform.position, CurrentTarget.position);
        }
    }
}