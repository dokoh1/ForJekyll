using MBT;
using UnityEngine;

/// <summary>
/// Charge(돌진) 상태: 목표 지점까지 NavMeshAgent 최대 속도로 돌진.
/// 충돌 처리는 MidBossEarType.OnChargeHit()에서 담당.
/// </summary>
[AddComponentMenu("")]
[MBTNode("MidBoss/MidBoss Charge")]
public class MidBossCharge : Leaf
{
    public IntReference curState;
    public Vector3Reference chargeTargetPos;
    public FloatReference   chargeSpeed;
    public FloatReference   maxChargeTime;
    public BoolReference    hasNoiseTarget;

    public MidBossEarType   midBoss;

    private float _timer;
    private bool _chargeStarted;

    public override void OnEnter()
    {
        _chargeStarted = false;

        if (curState.Value != (int)MidBossState.Charge)
            return;

        _chargeStarted = true;
        _timer = 0f;

        Vector3 targetPos = chargeTargetPos.Value;

        midBoss.BeginCharge(targetPos);

        midBoss.agent.isStopped = false;
        midBoss.agent.speed     = chargeSpeed.Value;
        midBoss.SetAnimation(charge:true);

        // overshoot: 목표 지점 뒤로 더 보내서 충분히 박치기
        Vector3 dir = (targetPos - midBoss.transform.position).normalized;
        dir.y = 0f;
        Vector3 final = targetPos + dir * midBoss.MidBossData.Data.chargeOvershoot;
        midBoss.agent.SetDestination(final);
    }

    public override NodeResult Execute()
    {
        if (curState.Value == (int)MidBossState.Stun)
            return NodeResult.failure;

        if (curState.Value != (int)MidBossState.Charge)
            return NodeResult.failure;

        _timer += DeltaTime;

        if (!midBoss.IsCharging)
            return ExitCharge();

        if (_timer >= maxChargeTime.Value)
        {
            midBoss.EndCharge();
            return ExitCharge();
        }

        return NodeResult.running;
    }

    private NodeResult ExitCharge()
    {
        midBoss.agent.isStopped = true;

        if (midBoss.IsComboActive)
        {
            curState.Value = (int)MidBossState.Rage;
        }
        else
        {
            curState.Value = (int)MidBossState.Idle;
            chargeTargetPos.Value = Vector3.zero;
            hasNoiseTarget.Value = false;
        }

        return NodeResult.success;
    }

    public override void OnExit()
    {
        midBoss.agent.isStopped = true;

        if (_chargeStarted)
        {
            NoiseDetectionGaugeManager.Instance?.ResetGauge();
            midBoss.SetVariable(BBKeys.MidBoss.DetectionGauge, 0f);
            _chargeStarted = false;
        }
        base.OnExit();
    }
}
