using MBT;
using NPC;
using UnityEngine;
using UnityEngine.AI;
[AddComponentMenu("")]
[MBTNode("Example/NPC Move To Player")]
public class NPCMoveToPlayer : Leaf
{
    public TransformReference self;
    public TransformReference destination;
    public IntReference curState;
    public FloatReference nearDistance;
    public FloatReference farDistance;
    public FloatReference distanceToplayer;
    public FloatReference baseSpeed;
    public FloatReference walkModifier;
    public FloatReference crouchModifier;
    public BoolReference isRest;
    public BoolReference isFollow;
    public NpcUnit npcUnit;

    public float updateInterval = 1f;
    private float time = 0;

    public override void OnEnter()
    {
        time = 0;
        npcUnit.Agent.isStopped = false;
        //Debug.Log($"NPCMoveToPlayer - OnEnter");

        if (((NPCState)curState.Value & NPCState.Move) != NPCState.Move) curState.Value = (int)((NPCState)curState.Value | NPCState.Move);
        if (((NPCState)curState.Value & NPCState.Run) == NPCState.Run)
        {
            curState.Value = (int)((NPCState)curState.Value & ~NPCState.Run);
        }

        if (((NPCState)curState.Value & NPCState.Crouch) == NPCState.Crouch)
        {
            npcUnit.Agent.speed = baseSpeed.Value * crouchModifier.Value;
        }
        else
        {
            npcUnit.Agent.speed = baseSpeed.Value * walkModifier.Value;
        }

        npcUnit.animator.SetBool("Idle", false);
        npcUnit.animator.SetBool("Run", false);
        npcUnit.animator.SetBool("Walk", true);

        npcUnit.Agent.SetDestination(destination.Value.position);
    }

    public override NodeResult Execute()
    {
        if (GameManager.Instance.IsTimeStop) return NodeResult.running;

        if (!isFollow.Value) return NodeResult.success;

        if (distanceToplayer.Value > farDistance.Value)
        {
            return NodeResult.success;
        }

        time += Time.deltaTime;
        if (time > updateInterval)
        {
            time = 0;
            npcUnit.Agent.SetDestination(destination.Value.position);
        }
        if (npcUnit.Agent.pathPending)
        {
            if (npcUnit.Agent.isStopped) npcUnit.Agent.isStopped = false;
            return NodeResult.running;
        }
        if (npcUnit.Agent.remainingDistance < nearDistance.Value)
        {
            isRest.Value = true;
            return NodeResult.success;
        }
        if (npcUnit.Agent.hasPath)
        {
            if (npcUnit.Agent.isStopped) npcUnit.Agent.isStopped = false;
            return NodeResult.running;
        }
        return NodeResult.failure;
    }

    public override void OnExit()
    {
        npcUnit.Agent.isStopped = true;
    }

}
