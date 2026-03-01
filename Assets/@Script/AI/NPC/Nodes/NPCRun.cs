using MBT;
using NPC;
using Unity.Mathematics;
using UnityEngine;

[AddComponentMenu("")]
[MBTNode("Example/NPC Run")]
public class NPCRun : NPCMoveToTransform
{
    public BoolReference isSkipValue;
    public TransformReference self;

    public Animator animator;


    public override void OnEnter()
    {
        //if (animator.GetBool("Run")) return;
        base.OnEnter();

        if (curState.Value != (int)NPCState.Run)
        {
            curState.Value = (int)NPCState.Run;
        }

        INoise noise;
        if (self.Value.gameObject.TryGetComponent<INoise>(out noise)) noise.CurNoiseAmount = 13f;

        animator.SetBool("@Crouch", false);
        animator.SetBool("Idle", false);
        animator.SetBool("Walk", false);
        animator.SetBool("Run", true);
    }

    public override NodeResult Execute()
    {
        if (!isSkipValue.Value) return NodeResult.success;

        if (distanceToplayer.Value < distance.Value)
        {
            return NodeResult.success;
        }

        return base.Execute();
    }
}
