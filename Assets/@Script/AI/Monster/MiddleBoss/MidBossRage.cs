using MBT;
using UnityEngine;

/// <summary>
/// Rage 상태: 포효 애니메이션 + 타깃 방향 회전 후 Charge 전이.
/// </summary>
[AddComponentMenu("")]
[MBTNode("MidBoss/MidBoss Rage Howl")]
public class MidBossRage : Leaf
{
    public BoolReference    hasNoiseTarget;
    public Vector3Reference chargeTargetPos;
    public IntReference     curState;
    public MidBossEarType   midBoss;

    private bool _playing;

    [Header("Animator")]
    [SerializeField] private string rageStateName = "Rage";

    [Header("Fallback Noise Scan")]
    [SerializeField] private LayerMask noiseMask;

    private Vector3 _lockedChargeTarget;

    public override void OnEnter()
    {
        if (curState.Value != (int)MidBossState.Rage)
        {
            _playing = false;
            return;
        }

        _playing = true;

        // Rage 시작 시점에 목표 위치 고정
        if (midBoss.IsComboActive)
        {
            if (midBoss.TrySetNextComboTarget())
            {
                _lockedChargeTarget = chargeTargetPos.Value;
                hasNoiseTarget.Value = true;
            }
            else
            {
                midBoss.EndCombo();
                _playing = false;
                return;
            }
        }
        else
        {
            if (hasNoiseTarget.Value)
            {
                _lockedChargeTarget = chargeTargetPos.Value;
            }
            else if (TryFindNoiseTarget(out Vector3 noisePos))
            {
                _lockedChargeTarget = noisePos;
                hasNoiseTarget.Value = true;
                chargeTargetPos.Value = noisePos;
            }
            else
            {
                _lockedChargeTarget = midBoss.transform.position
                    + midBoss.transform.forward * midBoss.MidBossData.Data.rageFallbackDistance;
            }
        }

        midBoss.agent.isStopped = true;
        midBoss.SetAnimation(rage:true);

        Vector3 lookTarget = _lockedChargeTarget;
        lookTarget.y = midBoss.transform.position.y;
        midBoss.LookAtPosition(lookTarget);

        base.OnEnter();
    }

    public override NodeResult Execute()
    {
        if (!_playing)
            return NodeResult.failure;

        if (curState.Value == (int)MidBossState.Stun)
            return NodeResult.failure;

        Animator anim = midBoss.animator;
        if (anim == null)
            return NodeResult.failure;

        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);

        bool rageFinished = info.IsName(rageStateName) && info.normalizedTime >= 1f;
        bool rotationFinished = !midBoss.IsLooking || midBoss.HasReachedLookTarget;

        if (rageFinished && rotationFinished)
        {
            chargeTargetPos.Value = _lockedChargeTarget;
            curState.Value = (int)MidBossState.Charge;
            return NodeResult.success;
        }
        return NodeResult.running;
    }

    private bool TryFindNoiseTarget(out Vector3 position)
    {
        return NoiseFinder.TryFindLoudest(
            midBoss.transform.position,
            midBoss.MidBossData.Data.detectRangeMax,
            noiseMask,
            out position,
            out _);
    }
}
