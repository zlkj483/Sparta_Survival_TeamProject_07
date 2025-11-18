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
    public bool IsDead;

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

    public bool TryDetectTarget(out Transform target)
    {
        target = null;
        if (Data == null || SelfTransform == null)
            return false;

        Vector3 origin = SelfTransform.position + Vector3.up * 1.5f;
        float sightRange = Data.sightRange;
        float halfFov = Data.sightAngle * 0.5f;

        // 1) 시야 거리 안의 후보들 (레이어: targetLayer)
        Collider[] hits = Physics.OverlapSphere(
            origin,
            sightRange,
            Data.targetLayer
        );

        // 후보 없으면 바로 실패
        if (hits.Length == 0)
            return false;

        foreach (var col in hits)
        {
            Transform t = col.transform;

            // 2) 각도 체크 (FOV 밖이면 스킵)
            Vector3 toTarget = (t.position - origin);
            Vector3 dir = toTarget.normalized;
            float angle = Vector3.Angle(SelfTransform.forward, dir);
            if (angle > halfFov)
                continue;

            float dist = toTarget.magnitude;

            // 3) Raycast로 장애물 체크
            //    obstacleMask + targetLayer만 맞추고, 트리거는 무시
            int mask = Data.obstacleMask | Data.targetLayer;
            if (Physics.Raycast(
                    origin,
                    dir,
                    out RaycastHit hitInfo,
                    dist,
                    mask,
                    QueryTriggerInteraction.Ignore))
            {
                // 첫 번째로 맞은 게 '타겟 레이어'면 성공
                if (((1 << hitInfo.collider.gameObject.layer) & Data.targetLayer) != 0)
                {
                    target = t;

#if UNITY_EDITOR
                    Debug.DrawLine(origin, hitInfo.point, Color.green, 0.1f);
#endif
                    return true;
                }
                else
                {
                    // 장애물에 막혔음
#if UNITY_EDITOR
                    Debug.DrawLine(origin, hitInfo.point, Color.red, 0.1f);
#endif
                    continue;
                }
            }
            else
            {
                // 사이에 아무것도 안 맞았으면 그냥 보이는 걸로 취급해도 됨
                target = t;
#if UNITY_EDITOR
                Debug.DrawLine(origin, origin + dir * dist, Color.yellow, 0.1f);
#endif
                return true;
            }
        }

        return false;
    }

}