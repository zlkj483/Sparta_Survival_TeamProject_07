using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAIController : MonoBehaviour
{
    [Header("데이터")]
    public MonsterData monsterData;

    [Header("스킨 붙일 위치 (비어있으면 자기 자신)")]
    public Transform skinParent;

    private AIContext _ctx;
    public AIContext Context => _ctx;

    private AIStateMachine _fsm;

    [Header("패트롤 포인트 (옵션)")]
    public Transform[] patrolPoints;   // ⭐ 인스펙터에서 직접 지정


    private void Awake()
    {
        var agent = GetComponent<NavMeshAgent>();
        var baseAnimator = GetComponent<Animator>();

        // 1) 스킨 생성
        Animator skinAnimator = null;
        if (monsterData != null && monsterData.skinPrefab != null)
        {
            Transform parent = skinParent != null ? skinParent : transform;
            GameObject skin = Instantiate(monsterData.skinPrefab, parent);
            skinAnimator = skin.GetComponentInChildren<Animator>();
        }

        // 2) 에이전트 스탯 세팅
        if (monsterData != null)
        {
            agent.speed = monsterData.moveSpeed;
        }

        if (skinAnimator != null)
        {
            var skinController = skinAnimator.runtimeAnimatorController;

            if (monsterData != null && monsterData.isBoss)
            {
                // 보스는 그냥 스킨 컨트롤러 그대로 사용
                if (skinController != null)
                    baseAnimator.runtimeAnimatorController = skinController;
            }
            else
            {
                // 일반 몹은 Override 사용
                var baseController = baseAnimator.runtimeAnimatorController;
                var sourceClips = skinController != null
                    ? skinController.animationClips
                    : null;

                var overrideController =
                    AnimationOverrideUtility.CreateOverride(baseController, sourceClips);

                if (overrideController != null)
                    baseAnimator.runtimeAnimatorController = overrideController;
            }

            // 아바타는 공통
            if (skinAnimator.avatar != null)
                baseAnimator.avatar = skinAnimator.avatar;

            skinAnimator.enabled = false;
        }


        // 4) AIContext 구성
        _ctx = new AIContext
        {
            SelfTransform = transform,
            Agent = agent,
            Animator = baseAnimator,
            Data = monsterData,
            PatrolPoints = patrolPoints != null
                ? System.Array.ConvertAll(patrolPoints, p => p.position)
                : null,
            PatrolIndex = 0
        };

        _fsm = new AIStateMachine();
    }

    private void Start()
    {
        _fsm.ChangeState(new IdleState(_ctx, _fsm));
    }

    private void Update()
    {
        if (_ctx.IsDead)
            return;

        _fsm.Tick();
    }

    public void DoAttackHit()
    {
        if (_ctx == null || _ctx.IsDead)
            return;

        float radius = _ctx.Data.attackRange * 0.8f;
        Vector3 center = _ctx.SelfTransform.position + _ctx.SelfTransform.forward * 1.0f;

        Collider[] hits = Physics.OverlapSphere(
            center,
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

    private void OnDrawGizmosSelected()
    {
        // 데이터가 없으면 아무것도 그리지 않음
        if (monsterData == null)
            return;

        Vector3 pos = transform.position;

        // 1) 시야 범위(시야각 + 거리) 디버그 ------------------------

        // 시야 거리 전체
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f); // 연두색
        Gizmos.DrawWireSphere(pos, monsterData.sightRange);

        // 시야각 양쪽 경계선
        float halfAngle = monsterData.sightAngle * 0.5f;

        Vector3 leftDir = Quaternion.Euler(0f, -halfAngle, 0f) * transform.forward;
        Vector3 rightDir = Quaternion.Euler(0f, halfAngle, 0f) * transform.forward;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(pos, pos + leftDir * monsterData.sightRange);
        Gizmos.DrawLine(pos, pos + rightDir * monsterData.sightRange);

        // 2) Aggro / Attack / GiveUp 범위 (원하면 같이 표시) --------

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(pos, monsterData.aggroRange);     // 감지 시작 범위

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos, monsterData.attackRange);    // 공격 범위

        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(pos, monsterData.giveUpRange);    // 포기 범위

        // 3) 현재 타겟까지 라인 (실제 감지 확인용) -------------------

        if (_ctx != null && _ctx.CurrentTarget != null)
        {
            Gizmos.color = Color.magenta;
            Vector3 from = pos + Vector3.up * 1.0f;
            Vector3 to = _ctx.CurrentTarget.position + Vector3.up * 1.0f;
            Gizmos.DrawLine(from, to);
        }
    }


}