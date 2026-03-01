using MBT;
using NPC;
using UnityEngine;

[AddComponentMenu("")]
[MBTNode("Example/NPC Move")]
public class NPCMove : NPCMoveToTransform
{
    public BoolReference isSkipValue;
    public TransformReference self;

    public Animator animator;
    public INoise noise;

    public override void OnEnter()
    {        
        base.OnEnter();
        if (((NPCState)curState.Value & NPCState.Move) != NPCState.Move) curState.Value = (int)((NPCState)curState.Value | NPCState.Move);
        if (((NPCState)curState.Value & NPCState.Run) == NPCState.Run)
        {
            curState.Value = (int)((NPCState)curState.Value & ~NPCState.Run);
        }
        //Debug.Log($"NPCMove - OnEnter() - {(NPCState)curState.Value}");
        //if (animator.GetBool("Walk")) return;

        if (self.Value.gameObject.TryGetComponent<INoise>(out noise))
        {
            if (((NPCState)curState.Value & NPCState.Crouch) == NPCState.Crouch) 
                noise.CurNoiseAmount = 0;
            else noise.CurNoiseAmount = 7;
        }

        animator.SetBool("Idle", false);
        animator.SetBool("Run", false);
        animator.SetBool("Walk", true);        
    }

    public override NodeResult Execute()
    {
        if (!isSkipValue.Value) return NodeResult.success;

        if (distanceToplayer.Value > distance.Value)
        {
            return NodeResult.success;
        }


        //if (_player.CurState == PlayerEnum.PlayerState.CrouchState)
        //{
        //    //Debug.Log("NPCMove - �÷��̾� ����");

        //    curState.Value = (int)(NPCState.Crouch | (NPCState)curState.Value);
        //    Debug.Log((NPCState)curState.Value);
        //}

        return base.Execute();
    }
}
