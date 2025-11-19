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

    // Animator 상태 이름과 1:1로 맞출 것
    private static readonly int Hash_BossAttack = Animator.StringToHash("Boss_Attack");
    private static readonly int Hash_BossJump = Animator.StringToHash("Boss_Jump");
    private static readonly int Hash_BossCharge = Animator.StringToHash("Boss_Charge");

    private Pattern _currentPattern;
    private int _currentStateHash;

    private float _patternTime;      // 이번 패턴 경과시간
    private float _patternDuration;  // 이번 패턴 총 길이(초)
    private bool _hasHit;           // 이번 패턴에서 데미지 1번만 주게

    // 거리 기준(적당히 조절)
    private const float JumpMinDist = 6f;
    private const float ChargeMinDist = 12f;
    private const float ExitGraceTime = 0.15f; // 끝나기 약간 전에 준비

    public BossAttackState(AIContext ctx, AIStateMachine fsm)
    {
        _ctx = ctx;
        _fsm = fsm;
    }

    public void OnEnter()
    {
        Debug.Log("[BossAttack] Enter");

        if (_ctx.Agent != null)
            _ctx.Agent.isStopped = true;

        if (_ctx.Animator != null)
            _ctx.Animator.SetBool("IsMove", false);

        ChoosePattern();
        PlayCurrentPattern();
    }

    public void OnExit()
    {
        Debug.Log("[BossAttack] Exit");

        if (_ctx.Agent != null)
            _ctx.Agent.isStopped = false;

        if (_ctx.Animator != null)
            _ctx.Animator.SetBool("IsMove", true);
    }

    public void Tick()
    {
        if (_ctx.CurrentTarget == null)
        {
            _fsm.ChangeState(new IdleState(_ctx, _fsm));
            return;
        }

        float dist = _ctx.DistanceToTarget;

        // 너무 멀어지면 포기
        if (dist > _ctx.GiveUpRange)
        {
            _ctx.CurrentTarget = null;
            _fsm.ChangeState(new IdleState(_ctx, _fsm));
            return;
        }

        if (_ctx.Animator == null)
            return;

        // 시간을 기준으로 패턴 진행 관리 (normalizedTime 안 믿음)
        _patternTime += Time.deltaTime;
        float tNorm = Mathf.Clamp01(_patternTime / Mathf.Max(_patternDuration, 0.01f));

        // 타격 타이밍
        HandleHitWindow(tNorm);

        // 패턴이 거의 끝났으면 다음 행동 결정
        if (_patternTime >= _patternDuration - ExitGraceTime)
        {
            // 애니를 Idle 쪽으로 돌려놓기 (트랜지션이 있으면 Idle로, 없으면 바로 Idle 상태)
            if (_ctx.Animator != null)
                _ctx.Animator.CrossFade("CharacterArmature|Idle", 0.1f);

            if (dist > _ctx.AttackRange + 1.0f)
            {
                Debug.Log("[BossAttack] Pattern end -> Chase");
                _fsm.ChangeState(new ChaseState(_ctx, _fsm));
            }
            else
            {
                Debug.Log("[BossAttack] Pattern end -> Next pattern");
                ChoosePattern();
                PlayCurrentPattern();
            }
        }
    }

    // -------- 패턴 선택 / 재생 --------

    private void ChoosePattern()
    {
        float dist = _ctx.DistanceToTarget;

        if (dist > ChargeMinDist)
        {
            _currentPattern = Pattern.Charge;
            _currentStateHash = Hash_BossCharge;
        }
        else if (dist > JumpMinDist)
        {
            // 중거리면 점프/기본 랜덤
            _currentPattern = (Random.value < 0.5f) ? Pattern.Jump : Pattern.Basic;
            _currentStateHash = (_currentPattern == Pattern.Jump)
                ? Hash_BossJump
                : Hash_BossAttack;
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

        _ctx.Animator.CrossFade(_currentStateHash, 0.1f);

        // 애니 클립 길이를 가져와서 패턴 길이로 사용
        _patternDuration = GetPatternDuration(_currentPattern);
        if (_patternDuration <= 0f)
            _patternDuration = 1.0f; // 안전빵 기본값
    }

    private float GetPatternDuration(Pattern pattern)
    {
        // TRex FBX 안의 실제 클립 이름 기준
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

    // -------- 데미지 타이밍 --------

    private void HandleHitWindow(float tNorm)
    {
        if (_hasHit || _ctx.Data == null)
            return;

        switch (_currentPattern)
        {
            case Pattern.Basic:
                // 물어뜯기: 중간쯤
                if (tNorm >= 0.30f && tNorm <= 0.60f)
                {
                    _hasHit = true;
                    DoBasicHit();
                }
                break;

            case Pattern.Jump:
                // 착지 직후
                if (tNorm >= 0.55f && tNorm <= 0.90f)
                {
                    _hasHit = true;
                    DoJumpAoEHit();
                }
                break;

            case Pattern.Charge:
                // 돌진 중간
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
        int targetMask = _ctx.Data.targetLayer;
        var hits = Physics.OverlapSphere(origin, radius, targetMask);

        foreach (var h in hits)
        {
            if (h.TryGetComponent(out IDamagable d))
                d.TakePhysicalDamage(damage);
        }
    }
}
