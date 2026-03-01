using MBT;
using UnityEngine;

/// <summary>
/// OverlapSphere로 가장 큰 NoiseCheckAmount를 가진 INoise를 탐색,
/// chargeTargetPos / hasNoiseTarget Blackboard에 기록.
/// </summary>
[AddComponentMenu("")]
[MBTNode("MidBoss/Detect Noise Service")]
public class MBDetectNoiseService : Service
{
    [Header("Blackboard")]
    public TransformReference self;
    public Vector3Reference   chargeTargetPos;
    public BoolReference      hasNoiseTarget;
    public IntReference curState;

    [Header("Detect Settings")]
    public FloatReference detectRangeMax;
    public LayerMask      noiseMask;

    public override void Task()
    {
        if (curState.Value == (int)MidBossState.Stun ||
            curState.Value == (int)MidBossState.Charge)
        {
            return;
        }

        if (NoiseFinder.TryFindLoudest(self.Value.position, detectRangeMax.Value, noiseMask,
                out var bestPos, out _))
        {
            hasNoiseTarget.Value  = true;
            chargeTargetPos.Value = bestPos;
        }
        else
        {
            hasNoiseTarget.Value = false;
        }
    }
}
