using MBT;
using NPC;
using UnityEngine;
using UnityEngine.AI;
[AddComponentMenu("")]
[MBTNode("Example/NPC Run To Player")]
public class NPCRunToPlayer : Leaf
{
    public TransformReference self;
    public TransformReference destination;
    public NavMeshAgent agent;
    public FloatReference nearDistance;
    public FloatReference baseSpeed;
    public FloatReference speedModifier;
    public FloatReference walkModifier;
    public FloatReference crouchModifier;
    public BoolReference isRest;
    public BoolReference isRunning;
    public BoolReference isFollow;
    public IntReference curState;
    public Animator animator;
    //public float stopDistance = 2f;
    public float updateInterval = 1f;
    private float time = 0;
    public NpcUnit npcUnit;

    public override void OnEnter()
    {
        time = 0;
        agent.isStopped = false;
        agent.SetDestination(destination.Value.position);
        agent.speed = baseSpeed.Value * speedModifier.Value;
        if (curState.Value != (int)NPCState.Run) curState.Value = (int)NPCState.Run;
        npcUnit.CurNoiseAmount = 13f;
        animator.SetBool("@Crouch", false);
        animator.SetBool("Idle", false);
        animator.SetBool("Walk", false);
        animator.SetBool("Run", true);
    }

    public override NodeResult Execute()
    {
        if (GameManager.Instance.IsTimeStop) return NodeResult.running;
        if (!isFollow.Value) return NodeResult.success;

        time += Time.deltaTime;
        if (time > updateInterval)
        {
            time = 0;
            agent.SetDestination(destination.Value.position);
        }
        if (agent.pathPending)
        {
            if (agent.isStopped) agent.isStopped = false;
            return NodeResult.running;
        }
        if (agent.remainingDistance < nearDistance.Value)
        {
            isRunning.Value = false;
            isRest.Value = true;
            return NodeResult.success;
        }
        if (agent.hasPath)
        {
            if (agent.isStopped) agent.isStopped = false;
            return NodeResult.running;
        }
        return NodeResult.failure;
    }

    public override void OnExit()
    {
        agent.isStopped = true;
    }
}
