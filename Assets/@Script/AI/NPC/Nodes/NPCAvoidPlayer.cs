using MBT;
using NPC;
using System;
using UnityEngine;
using UnityEngine.AI;

[AddComponentMenu("")]
[MBTNode(name = "NPC Avoid Player")]
public class NPCAvoidPlayer : Leaf
{
    public TransformReference self;
    public FloatReference distanceToPlayer;
    public IntReference curState;
    public BoolReference isRest;

    [SerializeField] private float escapeDistance = 2.6f;
    private float sampleRadius = 1.6f;

    public NavMeshAgent agent;
    public Animator animator;

    private static readonly Quaternion[] CachedRotations = new Quaternion[]
    {
        Quaternion.Euler(0, 0, 0),
        Quaternion.Euler(0, 45, 0),
        Quaternion.Euler(0, -45, 0),
        Quaternion.Euler(0, 90, 0),
        Quaternion.Euler(0, -90, 0)
    };

    public override void OnEnter()
    {
        if (GameManager.Instance.Player.StuckDetector.IsStuck && distanceToPlayer.Value <= 2.4f)
        {
            if (AvoidPlayer())
            {
                if (((NPCState)curState.Value & NPCState.Move) != NPCState.Move) curState.Value = (int)((NPCState)curState.Value | NPCState.Move);
                if (((NPCState)curState.Value & NPCState.Run) == NPCState.Run)
                {
                    curState.Value = (int)((NPCState)curState.Value & ~NPCState.Run);
                }
                animator.SetBool("Idle", false);
                animator.SetBool("Run", false);
                animator.SetBool("Walk", true);
            }
        }
    }

    public override NodeResult Execute()
    {
        if (!GameManager.Instance.Player.StuckDetector.IsStuck || distanceToPlayer.Value > 2.4f) return NodeResult.failure;

        if (agent.pathPending)
        {
            if (agent.isStopped) agent.isStopped = false;
            return NodeResult.running;
        }
        if (agent.remainingDistance < 0.2f)
        {
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

    public bool AvoidPlayer()
    {
        Vector3 awayDir = (self.Value.position - GameManager.Instance.Player.transform.position).normalized;

        foreach (var rot in CachedRotations)
        {
            Vector3 dir = rot * awayDir;
            Vector3 targetPos = self.Value.position + dir * escapeDistance;

            if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, sampleRadius, NavMesh.AllAreas))
            {
                //Debug.Log($"NPC Avoid Player: {hit.position} {hit.normal} {hit.distance}");
                agent.isStopped = false;
                agent.SetDestination(hit.position);
                return true;
            }
        }
        return false;
    }

    public override void OnExit()
    {
        agent.isStopped = true;
    }
}