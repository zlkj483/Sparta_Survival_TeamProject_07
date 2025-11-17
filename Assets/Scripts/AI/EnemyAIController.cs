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

        // 3) 애니메이션 Override 자동 세팅
        if (skinAnimator != null)
        {
            // 스킨 컨트롤러에서 애니메이션 클립들 가져오기
            var sourceController = skinAnimator.runtimeAnimatorController;
            AnimationClip[] sourceClips = sourceController != null
                ? sourceController.animationClips
                : null;

            // Base 컨트롤러 가져오기 (AC_MonsterBase)
            var baseController = baseAnimator.runtimeAnimatorController;

            // Override 생성
            var overrideController =
                AnimationOverrideUtility.CreateOverride(baseController, sourceClips);

            if (overrideController != null)
            {
                baseAnimator.runtimeAnimatorController = overrideController;
            }

            // Avatar도 스킨쪽 것을 사용
            if (skinAnimator.avatar != null)
            {
                baseAnimator.avatar = skinAnimator.avatar;
            }

            // 스킨쪽 Animator는 비활성화 (선택)
            skinAnimator.enabled = false;


        }

        // 4) AIContext 구성
        _ctx = new AIContext
        {
            SelfTransform = transform,
            Agent = agent,
            Animator = baseAnimator,
            Data = monsterData,

            // 패트롤 포인트 세팅
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
        if (_ctx != null && _ctx.IsDead)
            return;          // 죽었으면 FSM 멈춤

        _fsm.Tick();
    }

    private void OnDrawGizmosSelected()
    {
        // _ctx가 아직 생성되지 않았으면 그리지 않음
        if (_ctx == null || monsterData == null)
            return;

        // 데이터를 MonsterData에서 바로 읽어도 됨
        Gizmos.color = new Color(1f, 1f, 0f, 0.4f); // 반투명 Yellow
        Gizmos.DrawWireSphere(transform.position, monsterData.aggroRange);

        Gizmos.color = new Color(1f, 0f, 0f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, monsterData.attackRange);

        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, monsterData.giveUpRange);
    }

}