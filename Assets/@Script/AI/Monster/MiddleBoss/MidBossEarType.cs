using MBT;
using UnityEngine;

public class MidBossEarType : MonsterV3
{
    [field: SerializeField] public MidBossDataSO MidBossData { get; private set; }
    [SerializeField] private Chapter4_1_SceneInitializer chapterController;
    [SerializeField] private MidBossComboSystem comboSystem;
    [SerializeField] private MidBossImpactFeedback impactFeedback;

    [Header("Player Hit")]
    [SerializeField] private float chargePlayerHitRadius = 1.5f;

    private const string Tag_UnbreakableWall = "UnbreakableWall";

    // 돌진
    public bool   IsCharging    { get; private set; }
    public Vector3 ChargeStartPos { get; private set; }

    public bool IsComboActive => comboSystem.IsActive;

    // 기절
    private float _stunTimer;
    private bool  _isEnraged;
    private int   _wallHitCount;

    // Editor 읽기용
    public MidBossState CurrentState =>
        BB != null ? (MidBossState)BB.GetVariable<Variable<int>>(BBKeys.MidBoss.CurState).Value
                   : MidBossState.Idle;
    public bool IsEnraged => _isEnraged;
    public float StunTimer => _stunTimer;
    public int WallHitCount => _wallHitCount;
    public MidBossComboSystem ComboSystem => comboSystem;
    public float ChargePlayerHitRadius => chargePlayerHitRadius;
    private bool _doorBrokenNotified;
    private int _breakCount;
    private bool _playerHit;

    protected override void Start()
    {
        base.Start();
        Initialization();
    }

    private void Initialization()
    {
        if (BB == null || MidBossData == null || PatrolPosition == null)
            return;

        SetVariable("detectMin", MidBossData.Data.detectMin);
        SetVariable("rageMin",  MidBossData.Data.rageMin);
        SetVariable("chargeSpeed",  MidBossData.Data.chargeSpeed);
        SetVariable("maxChargeTime", MidBossData.Data.maxChargeTime);
        SetVariable("stopDistance", MidBossData.Data.stopDistance);
        SetVariable("detectRangeMax", MidBossData.Data.detectRangeMax);

        SetVariable(BBKeys.MidBoss.DetectionGauge,   0f);
        SetVariable(BBKeys.MidBoss.IsEnraged,        false);
        SetVariable(BBKeys.MidBoss.ChargeTargetPos,  Vector3.zero);
        SetVariable(BBKeys.MidBoss.Self,  transform);
        SetVariable(BBKeys.MidBoss.CurState,  (int)MidBossState.Idle);
    }

    protected override void Update()
    {
        base.Update();

        if (IsCharging && !_playerHit)
            CheckPlayerDuringCharge();

        if (_stunTimer > 0f)
        {
            _stunTimer -= Time.deltaTime;
            if (_stunTimer <= 0f)
            {
                _stunTimer = 0f;

                if (comboSystem.TryActivatePending())
                {
                    NoiseDetectionGaugeManager.Instance?.SetIncreaseBlocked(true);
                    SetVariable(BBKeys.MidBoss.CurState, (int)MidBossState.Rage);
                }
                else
                {
                    SetVariable(BBKeys.MidBoss.CurState, (int)MidBossState.Idle);
                    NoiseDetectionGaugeManager.Instance?.SetIncreaseBlocked(false);
                }
            }
        }
    }

    #region Enrage

    public void EnterEnrage()
    {
        if (_isEnraged)
            return;

        _isEnraged = true;
        SetVariable(BBKeys.MidBoss.IsEnraged, true);
        NoiseDetectionGaugeManager.Instance?.SetGainMultiplier(MidBossData.Data.noiseGainMultiplierOnEnrage);
    }

    public bool TrySetNextComboTarget() => comboSystem.TrySetNextTarget();

    public void EndCombo() => comboSystem.End();

    #endregion

    #region Charge / Stun

    public void BeginCharge(Vector3 targetPos)
    {
        IsCharging    = true;
        ChargeStartPos = transform.position;
        SetVariable(BBKeys.MidBoss.ChargeTargetPos, targetPos);
    }

    public void EndCharge()
    {
        IsCharging = false;
        _breakCount = 0;
        _playerHit = false;
    }

    public void ApplyStun()
    {
        float dur = _isEnraged ? MidBossData.Data.enragedStunDuration : MidBossData.Data.baseStunDuration;

        _stunTimer = dur;
        SetVariable(BBKeys.MidBoss.CurState, (int)MidBossState.Stun);

        agent.isStopped = true;
        SetAnimation(stun:true);

        NoiseDetectionGaugeManager.Instance?.ResetGauge();
        NoiseDetectionGaugeManager.Instance?.SetIncreaseBlocked(true);
        SetVariable(BBKeys.MidBoss.DetectionGauge,  0f);

        // 이전 타겟 정보 초기화 → 다음 Rage 시 새 소음 위치 탐색
        SetVariable(BBKeys.MidBoss.HasNoiseTarget, false);
        SetVariable(BBKeys.MidBoss.ChargeTargetPos, Vector3.zero);
    }

    public void OnChargeHit(Collider other, Vector3 hitPoint, Vector3 hitNormal)
    {
        Debug.Log($"[MidBoss] OnChargeHit() - IsCharging:{IsCharging}, other:{other.name}, impactFeedback null?:{impactFeedback == null}");

        if (!IsCharging)
            return;

        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            HandlePlayerHit(hitPoint);
            return;
        }

        if (other.TryGetComponent(out BreakableDoor door))
        {
            HandleDoorHit(door, Vector3.Distance(ChargeStartPos, hitPoint));
            return;
        }

        if (other.CompareTag(Tag_UnbreakableWall))
        {
            HandleUnbreakableWallHit();
            return;
        }

        if (other.TryGetComponent(out IBreakableEnvironment breakableEnv))
        {
            HandleBreakableHit(breakableEnv, hitPoint, hitNormal);
        }
    }

    private void HandleDoorHit(BreakableDoor door, float distance)
    {
        if (comboSystem.IsActive)
        {
            EndCharge();
            return;
        }

        impactFeedback?.Play(transform.position, 1f);

        bool wasDestroyedBefore = door.IsDestroyed;
        bool validHit = door.OnHitByMidBoss(distance, MidBossData.Data.requiredDoorDistance, this);

        if (validHit && !wasDestroyedBefore && door.CurrentHp == 1)
        {
            EnterEnrage();
            comboSystem.PrepareTargets();
            comboSystem.SetPending(true);
        }
        else if (!wasDestroyedBefore && door.CurrentHp == 0 && !comboSystem.IsActive && !comboSystem.IsPending)
        {
            NotifyDoorBroken();
        }

        ApplyStun();
        EndCharge();
    }

    private void HandleUnbreakableWallHit()
    {
        if (!comboSystem.IsActive)
        {
            impactFeedback?.Play(transform.position, 1f);
            HandleWallHit();
            ApplyStun();
        }
        EndCharge();
    }

    private void HandleBreakableHit(IBreakableEnvironment breakableEnv, Vector3 hitPoint, Vector3 hitNormal)
    {
        breakableEnv.OnHitByMidBoss(hitPoint, hitNormal);
        impactFeedback?.Play(hitPoint, 0.5f);

        if (comboSystem.IsActive)
        {
            if (_breakCount == 0)
            {
                _breakCount = 1;
                return;
            }

            _breakCount = 0;
            EndCharge();
            return;
        }

        if (_breakCount == 0)
        {
            _breakCount = 1;
            return;
        }

        if (_breakCount == 1)
        {
            ApplyStun();
            _breakCount = 0;
        }

        EndCharge();
    }

    private void CheckPlayerDuringCharge()
    {
        Player player = GameManager.Instance.Player;
        if (player == null || player.IsDead)
            return;

        float dist = Vector3.Distance(transform.position, player.transform.position);
        if (dist <= chargePlayerHitRadius)
            HandlePlayerHit(player.transform.position);
    }

    private void HandlePlayerHit(Vector3 hitPoint)
    {
        _playerHit = true;
        impactFeedback?.Play(hitPoint, 1f);
        EndCharge();
        GameManager.Instance.Player.OnHitSuccess(UnitEnum.UnitType.EarTypeMonster);

        // JumpScare 카메라에 MidBoss 모델이 보이지 않도록 렌더러 비활성화
        foreach (var r in GetComponentsInChildren<Renderer>())
            r.enabled = false;
    }

    private void NotifyDoorBroken()
    {
        if (_doorBrokenNotified)
            return;

        _doorBrokenNotified = true;

        if (chapterController != null)
            chapterController.OnMidBossBreakDoor();
        else
            Debug.LogWarning("[MidBoss] chapterController가 연결 안 되어 있음!");
    }

    /// <summary>
    /// 벽 충돌 횟수 누적: 3회 경고1, 4회 경고2, 5회 Game Over.
    /// </summary>
    private void HandleWallHit()
    {
        _wallHitCount++;

        if (_wallHitCount == MidBossData.Data.wallWarn1Count)
        {
            // TODO: 경고1 연출
        }
        else if (_wallHitCount == MidBossData.Data.wallWarn2Count)
        {
            // TODO: 경고2 연출
        }
        else if (_wallHitCount >= MidBossData.Data.wallDeathCount)
        {
            // TODO: 낙석 연출 + 플레이어 처형
            GameManager.Instance.Player.OnHitSuccess(UnitEnum.UnitType.EarTypeMonster);
        }
    }

    #endregion
}
