using MBT;
using NPC;
using UnityEngine;

[AddComponentMenu("")]
[MBTNode("Example/NPC Idle")]
public class NPCIdle : Leaf
{
    public IntReference curState;
    public TransformReference self;

    public Animator animator;

    public override void OnEnter()
    {
        //Debug.Log("NPCIdle - OnEnter() - NPCIdle");
        if (animator.GetBool("Idle")) return;
        base.OnEnter();

        curState.Value = (int)NPCState.Idle;
        INoise noise;
        if (self.Value.gameObject.TryGetComponent<INoise>(out noise)) noise.CurNoiseAmount = 0f;
        animator.SetBool("Walk", false);
        animator.SetBool("Run", false);
        animator.SetBool("Idle", true);
    }

    public override NodeResult Execute()
    {
        return NodeResult.success;
    }
}
