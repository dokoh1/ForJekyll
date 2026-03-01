using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// 중간보스 소음 감지 게이지(0~100) 전역 관리자.
/// 플레이어 이동 / 타이머 오브젝트 / 투척체가 per-sec 또는 즉발로 게이지 변화를 요청.
/// MidBossGaugeService가 이 값을 Blackboard에 복사.
/// </summary>
public class NoiseDetectionGaugeManager : MonoBehaviour
{
    public static NoiseDetectionGaugeManager Instance { get; private set; }
    public float CurrentGauge { get; private set; }

    [TabGroup("ScoreSettings", "Score")] private float _maxGauge              = 100f;
    [TabGroup("ScoreSettings", "Score")] private float _crouchOrIdleDeltaPerSec = -5f;
    [TabGroup("ScoreSettings", "Score")] private float _walkDeltaPerSec        =  5f;
    [TabGroup("ScoreSettings", "Score")] private float _runDeltaPerSec         = 10f;
    [TabGroup("ScoreSettings", "Score")] private float _timerNoiseDeltaPerSec  = 30f;
    [TabGroup("ScoreSettings", "Score")] private float _gainMultiplier         = 1f;

    [Header("Decrease Delay")]
    [SerializeField] private float decreaseDelayAfterIncrease = 3f;
    private float _timeSinceLastIncrease;

    [Header("Post-Reset Cooldown")]
    [SerializeField] private float postResetCooldown = 5f;
    private float _resetCooldownTimer;

    public float DebugPlayerDeltaPerSec  => _playerDeltaPerSec;
    public int   DebugActiveTimerCount   => _activeTimerNoiseCount;
    public float DebugGainMultiplier     => _gainMultiplier;
    public bool  DebugBlockIncrease      => _blockIncrease;

    public enum PlayerNoiseState { IdleOrCrouch, Walk, Run }

    private float _playerDeltaPerSec;
    private int _activeTimerNoiseCount;
    private bool _blockIncrease;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        if (_resetCooldownTimer > 0f)
        {
            _resetCooldownTimer -= Time.deltaTime;
            return;
        }

        float timerDelta = _activeTimerNoiseCount * _timerNoiseDeltaPerSec;
        float total = (_playerDeltaPerSec + timerDelta) * _gainMultiplier;
        float delta = total * Time.deltaTime;

        if (_blockIncrease && delta > 0f)
            delta = 0f;

        if (delta > 0f)
        {
            _timeSinceLastIncrease = 0f;
        }
        else if (delta < 0f)
        {
            _timeSinceLastIncrease += Time.deltaTime;
            if (_timeSinceLastIncrease < decreaseDelayAfterIncrease)
                delta = 0f;
        }

        CurrentGauge = Mathf.Clamp(CurrentGauge + delta, 0f, _maxGauge);
    }

    #region Player movement
    public void SetPlayerNoiseState(PlayerNoiseState state)
    {
        switch (state)
        {
            case PlayerNoiseState.IdleOrCrouch:
                _playerDeltaPerSec = _crouchOrIdleDeltaPerSec;
                break;
            case PlayerNoiseState.Run:
                _playerDeltaPerSec = _runDeltaPerSec;
                break;
            case PlayerNoiseState.Walk:
                _playerDeltaPerSec = _walkDeltaPerSec;
                break;
        }
    }
    #endregion

    #region Timer / Broken objects (per-sec noise)
    public void RegisterTimerNoise()   => _activeTimerNoiseCount++;
    public void UnregisterTimerNoise() => _activeTimerNoiseCount = Mathf.Max(0, _activeTimerNoiseCount - 1);
    #endregion

    #region Instant noise
    public void AddInstant(float amount)
    {
        if (_resetCooldownTimer > 0f)
            return;

        CurrentGauge = Mathf.Clamp(CurrentGauge + amount, 0f, _maxGauge);
        _timeSinceLastIncrease = 0f;
    }

    public void ResetGauge()
    {
        CurrentGauge = 0f;
        _timeSinceLastIncrease = 0f;
        _resetCooldownTimer = postResetCooldown;
    }
    #endregion

    #region Boss control
    public void SetIncreaseBlocked(bool block) => _blockIncrease = block;
    public void SetGainMultiplier(float multiplier) => _gainMultiplier = multiplier;
    #endregion
}
