using MBT;
using UnityEngine;

[AddComponentMenu("")]
[MBTNode("Example/NPC Move To Transform")]
public class NPCMoveToTransform : MoveToTransform
{
    public FloatReference nearDistance;
    public FloatReference baseSpeed;
    public FloatReference speedModifier;
    public IntReference curState;
    public FloatReference distance;
    public FloatReference distanceToplayer;

    public override void OnEnter()
    {
        base.OnEnter();
        stopDistance = nearDistance.Value;
        agent.speed = baseSpeed.Value * speedModifier.Value;
    }
}
