using MBT;
using UnityEngine;

/// <summary>
/// NoiseDetectionGaugeManager의 게이지를 Blackboard에 반영하고,
/// 임계값에 따라 상태 전이(Idle/Detect/Rage) 트리거.
/// </summary>
[AddComponentMenu("")]
[MBTNode("MidBoss/MidBoss Gauge Service")]
public class MidBossGaugeService : Service
{
    public FloatReference detectionGauge;
    public IntReference curState;

    [Header("Thresholds")]
    public FloatReference detectMin; // 30
    public FloatReference rageMin;   // 50

    private float _prevGauge;

    public override void OnEnter()
    {
        base.OnEnter();
        _prevGauge = detectionGauge.Value;
    }

    public override void Task()
    {
        var mgr = NoiseDetectionGaugeManager.Instance;
        if (mgr == null)
            return;

        float gauge = mgr.CurrentGauge;
        detectionGauge.Value = gauge;

        if (curState.Value == (int)MidBossState.Stun)
        {
            _prevGauge = gauge;
            return;
        }

        // Detect → Idle (게이지가 detectMin 아래로 떨어짐)
        if (curState.Value != (int)MidBossState.Idle &&
            _prevGauge > detectMin.Value && _prevGauge < rageMin.Value &&
            gauge < detectMin.Value)
        {
            curState.Value = (int)MidBossState.Idle;
        }

        // Idle → Detect (게이지가 30~49 구간 진입)
        if (curState.Value != (int)MidBossState.Detect &&
            _prevGauge < detectMin.Value &&
            gauge >= detectMin.Value && gauge < rageMin.Value)
        {
            curState.Value = (int)MidBossState.Detect;
        }

        // → Rage (게이지가 50 이상)
        if (curState.Value != (int)MidBossState.Rage &&
            _prevGauge < rageMin.Value &&
            gauge >= rageMin.Value)
        {
            curState.Value = (int)MidBossState.Rage;
        }

        _prevGauge = gauge;
    }
}
