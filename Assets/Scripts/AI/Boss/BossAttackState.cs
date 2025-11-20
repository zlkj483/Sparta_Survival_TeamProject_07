using UnityEngine;

public class BossAttackState : IAIState
{
    private enum Pattern
    {
        Basic,
        Jump,
        Charge
    }

    private readonly AIContext _ctx;
    private readonly AIStateMachine _fsm;

    // Animator 상태 이름 (Animator의 State 이름과 100% 동일해야 함)
    private static readonly int Hash_BossAttack = Animator.StringToHash("Boss_Attack");
    private static readonly int Hash_BossJump = Animator.StringToHash("Boss_Jump");
    private static readonly int Hash_BossCharge = Animator.StringToHash("Boss_Charge");

    private Pattern _currentPattern;
    private int _currentStateHash;

    private float _patternTime;      // 이번 패턴 경과 시간
    private float _patternDuration;  // 이번 패턴 총 길이
    private bool _hasHit;           // 이번 패턴에서 이미 때렸는지

    // ====== 패턴별 쿨타임 (초) ======
    private const float BasicCooldown = 0.8f;
    private const float JumpCooldown = 4.0f;
    private const float ChargeCooldown = 6.0f;

    private float _basicCdTimer;
    private float _jumpCdTimer;
    private float _chargeCdTimer;

    // ====== 거리 기준 ======
    private const float JumpMinDist = 6f;   // 이 이상이면 점프 가능 후보
    private const float ChargeMinDist = 12f;  // 이 이상이면 돌진 후보

    // ====== Charge 전용 변수 ======
    private bool _isCharging;
    private Vector3 _chargeTargetPos;         // 돌진 시작 시 찍은 목표 지점
    private const float ChargeArriveThreshold = 1.0f; // 이 거리 안까지 가면 도착한 걸로
    private const float ChargeSpeedMultiplier = 2.0f; // NavMeshAgent 속도 배율

    private const float EnemyBodyRadius = 0.8f; // 적 몸통 반경 (감)
    private const float PlayerBodyRadius = 0.8f; // 플레이어 반경 (감)

    public BossAttackState(AIContext ctx, AIStateMachine fsm)
    {
        _ctx = ctx;
        _fsm = fsm;
    }

    public void OnEnter()
    {
        Debug.Log("[BossAttack] Enter");

        _patternTime = 0f;
        _hasHit = false;
        _isCharging = false;

        // 이동을 기본적으로 멈춰두고, 패턴별로 다시 설정
        if (_ctx.Agent != null)
        {
            _ctx.Agent.isStopped = true;
            _ctx.Agent.speed = _ctx.Data != null ? _ctx.Data.moveSpeed : _ctx.Agent.speed;
        }

        if (_ctx.Animator != null)
            _ctx.Animator.SetBool("IsMove", false);

        ChoosePattern();
        PlayCurrentPattern();
    }

    public void OnExit()
    {
        Debug.Log("[BossAttack] Exit");

        _isCharging = false;

        if (_ctx.Agent != null)
        {
            _ctx.Agent.isStopped = false;
            _ctx.Agent.speed = _ctx.Data != null ? _ctx.Data.moveSpeed : _ctx.Agent.speed;
        }

        if (_ctx.Animator != null)
            _ctx.Animator.SetBool("IsMove", true);
    }

    public void Tick()
    {
        if (_ctx.IsDead) return;

        // 쿨타임 타이머 증가
        float dt = Time.deltaTime;
        _basicCdTimer += dt;
        _jumpCdTimer += dt;
        _chargeCdTimer += dt;

        if (_ctx.CurrentTarget == null)
        {
            _fsm.ChangeState(new IdleState(_ctx, _fsm));
            return;
        }

        float distToTarget = _ctx.DistanceToTarget;

        // 너무 멀어지면 (멀리 도망가면) 포기
        if (distToTarget > _ctx.GiveUpRange)
        {
            _ctx.CurrentTarget = null;
            _fsm.ChangeState(new IdleState(_ctx, _fsm));
            return;
        }

        if (_ctx.Animator == null)
            return;

        // 패턴 시간 진행
        _patternTime += dt;
        float tNorm = Mathf.Clamp01(_patternTime / Mathf.Max(_patternDuration, 0.01f));

        // ====== Charge 이동 처리 (커밋) ======
        if (_currentPattern == Pattern.Charge)
        {
            HandleChargeMovement();
        }

        // 타격 타이밍 처리
        HandleHitWindow(tNorm);

        // ====== 패턴 종료 처리 ======
        bool patternFinished = false;

        if (_currentPattern == Pattern.Charge)
        {
            // Charge는 "목표 지점 도착" 또는 "시간 초과" 둘 중 하나로 끝남
            float distToChargePos = Vector3.Distance(_ctx.SelfTransform.position, _chargeTargetPos);
            if (distToChargePos <= ChargeArriveThreshold ||
                _patternTime >= _patternDuration)
            {
                patternFinished = true;
            }
        }
        else
        {
            // Basic/Jump는 애니 길이로만 종료
            if (_patternTime >= _patternDuration)
                patternFinished = true;
        }

        if (!patternFinished)
            return;

        // 패턴 끝났으면 Charge 중단
        _isCharging = false;
        if (_ctx.Agent != null)
        {
            _ctx.Agent.isStopped = true;
            _ctx.Agent.speed = _ctx.Data != null ? _ctx.Data.moveSpeed : _ctx.Agent.speed;
        }

        // 도망을 쳤으면 다시 추격
        if (distToTarget > _ctx.AttackRange + 1.0f)
        {
            Debug.Log("[BossAttack] Pattern end -> Chase");
            _fsm.ChangeState(new ChaseState(_ctx, _fsm));
            return;
        }

        // 여전히 근처면 다음 패턴
        Debug.Log("[BossAttack] Pattern end -> Next pattern");
        ChoosePattern();
        PlayCurrentPattern();
        KeepDistanceFromPlayer();
    }

    // ================== 패턴 선택 / 재생 ==================

    private void ChoosePattern()
    {
        float dist = _ctx.DistanceToTarget;

        // 쿨타임 조건 충족 여부
        bool canBasic = _basicCdTimer >= BasicCooldown;
        bool canJump = _jumpCdTimer >= JumpCooldown;
        bool canCharge = _chargeCdTimer >= ChargeCooldown;

        // 1) 거리 + 쿨타임으로 우선 후보 결정
        // 먼 거리에서는 Charge 우선
        if (dist > ChargeMinDist && canCharge)
        {
            _currentPattern = Pattern.Charge;
            _currentStateHash = Hash_BossCharge;
            return;
        }

        // 중거리에서는 Jump/Basic 랜덤
        if (dist > JumpMinDist && (canJump || canBasic))
        {
            if (canJump && Random.value < 0.6f)
            {
                _currentPattern = Pattern.Jump;
                _currentStateHash = Hash_BossJump;
            }
            else
            {
                _currentPattern = Pattern.Basic;
                _currentStateHash = Hash_BossAttack;
            }
            return;
        }

        // 가까우면 Basic 우선
        if (canBasic)
        {
            _currentPattern = Pattern.Basic;
            _currentStateHash = Hash_BossAttack;
            return;
        }

        // 모든 게 쿨타임이면: 가장 쿨타임이 거의 끝난 놈 선택 (fallback)
        if (canJump)
        {
            _currentPattern = Pattern.Jump;
            _currentStateHash = Hash_BossJump;
        }
        else if (canCharge)
        {
            _currentPattern = Pattern.Charge;
            _currentStateHash = Hash_BossCharge;
        }
        else
        {
            _currentPattern = Pattern.Basic;
            _currentStateHash = Hash_BossAttack;
        }
    }

    private void PlayCurrentPattern()
    {
        if (_ctx.Animator == null)
            return;

        _patternTime = 0f;
        _hasHit = false;
        _isCharging = false;

        // 쿨타임 리셋
        switch (_currentPattern)
        {
            case Pattern.Basic: _basicCdTimer = 0f; break;
            case Pattern.Jump: _jumpCdTimer = 0f; break;
            case Pattern.Charge: _chargeCdTimer = 0f; break;
        }

        // Charge의 경우 목표 지점 잠금 + 이동 세팅
        if (_currentPattern == Pattern.Charge)
        {
            _isCharging = true;
            _chargeTargetPos = _ctx.CurrentTarget.position;

            if (_ctx.Agent != null)
            {
                _ctx.Agent.isStopped = false;
                float baseSpeed = _ctx.Data != null ? _ctx.Data.moveSpeed : _ctx.Agent.speed;
                _ctx.Agent.speed = baseSpeed * ChargeSpeedMultiplier;
                _ctx.Agent.SetDestination(_chargeTargetPos);
            }
        }
        else
        {
            // Charge 아닐 땐 그 자리에서 공격만
            if (_ctx.Agent != null)
            {
                _ctx.Agent.isStopped = true;
                _ctx.Agent.speed = _ctx.Data != null ? _ctx.Data.moveSpeed : _ctx.Agent.speed;
            }
        }

        _ctx.Animator.CrossFade(_currentStateHash, 0.1f);
        _patternDuration = GetPatternDuration(_currentPattern);
        if (_patternDuration <= 0f)
            _patternDuration = 1.0f; // 안전빵 디폴트
    }

    private float GetPatternDuration(Pattern pattern)
    {
        string clipName = "CharacterArmature|Attack";

        switch (pattern)
        {
            case Pattern.Jump:
                clipName = "CharacterArmature|Jump";
                break;
            case Pattern.Charge:
                clipName = "CharacterArmature|Run";
                break;
        }

        var ctrl = _ctx.Animator.runtimeAnimatorController;
        if (ctrl == null) return 0f;

        foreach (var c in ctrl.animationClips)
        {
            if (c.name == clipName)
                return c.length;
        }

        return 0f;
    }

    // ================== 타격 처리 ==================

    private void HandleHitWindow(float tNorm)
    {
        if (_hasHit || _ctx.Data == null)
            return;

        switch (_currentPattern)
        {
            case Pattern.Basic:
                // 물어뜯기: 중간 타이밍
                if (tNorm >= 0.3f && tNorm <= 0.6f)
                {
                    _hasHit = true;
                    DoBasicHit();
                }
                break;

            case Pattern.Jump:
                // 착지 후 광역
                if (tNorm >= 0.55f && tNorm <= 0.9f)
                {
                    _hasHit = true;
                    DoJumpAoEHit();
                }
                break;

            case Pattern.Charge:
                // 돌진 중간 구간에 히트
                if (tNorm >= 0.25f && tNorm <= 0.75f)
                {
                    _hasHit = true;
                    DoChargeHit();
                }
                break;
        }
    }

    private void DoBasicHit()
    {
        float radius = _ctx.AttackRange * 0.8f;
        Vector3 origin = _ctx.SelfTransform.position +
                         _ctx.SelfTransform.forward * (_ctx.AttackRange * 0.6f);

        HitSphere(origin, radius, _ctx.Data.baseDamage);
    }

    private void DoJumpAoEHit()
    {
        float radius = _ctx.AttackRange * 1.8f;
        Vector3 origin = _ctx.SelfTransform.position;

        HitSphere(origin, radius, _ctx.Data.baseDamage * 1.5f);
    }

    private void DoChargeHit()
    {
        float radius = _ctx.AttackRange;
        Vector3 origin = _ctx.SelfTransform.position +
                         _ctx.SelfTransform.forward * (_ctx.AttackRange);

        HitSphere(origin, radius, _ctx.Data.baseDamage * 1.2f);
    }

    private void HitSphere(Vector3 origin, float radius, float damage)
    {
        int mask = _ctx.Data.targetLayer;
        var hits = Physics.OverlapSphere(origin, radius, mask);

        foreach (var h in hits)
        {
            if (h.TryGetComponent(out IDamagable d))
            {
                d.TakePhysicalDamage(damage);
            }
        }
    }

    // ====== Charge 이동 처리 ======
    private void HandleChargeMovement()
    {
        if (!_isCharging || _ctx.Agent == null)
            return;

        // NavMeshAgent가 목적지로 계속 달리게 두면 됨.
        // 필요하면 여기서 플레이어 쪽으로 회전만 보정해줘도 됨.
        Vector3 dir = _chargeTargetPos - _ctx.SelfTransform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            _ctx.SelfTransform.rotation =
                Quaternion.Slerp(_ctx.SelfTransform.rotation, targetRot, Time.deltaTime * 10f);
        }
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
