using MBT;
using UnityEngine;
using UnityEngine.AI;

[AddComponentMenu("")]
[MBTNode("Example/Move To Transform")]
public class MoveToTransform : Leaf
{
    public TransformReference destination;
    public NavMeshAgent agent;
    public float stopDistance = 2f;
    [Tooltip("How often target position should be updated")]
    public float updateInterval = 1f;
    protected float time = 0;

    public override void OnEnter()
    {
        time = 0;
        agent.isStopped = false;
        //Debug.Log("agent.isStopped = false");

        agent.SetDestination(destination.Value.position);
    }

    public override NodeResult Execute()
    {
        time += Time.deltaTime;
        // Update destination every given interval
        if (time > updateInterval)
        {
            // Reset time and update destination
            time = 0;
            agent.SetDestination(destination.Value.position);
        }
        // Check if path is ready
        if (agent.pathPending)
        {
            //Debug.Log("if (agent.pathPending)");
            if (agent.isStopped) agent.isStopped = false;
            return NodeResult.running;
        }
        // Check if agent is very close to destination
        if (agent.remainingDistance < stopDistance)
        {
            //Debug.Log("if (agent.remainingDistance < stopDistance)");
            return NodeResult.success;
        }
        // Check if there is any path (if not pending, it should be set)
        if (agent.hasPath)
        {
            //Debug.Log("if (agent.hasPath)");
            if (agent.isStopped) agent.isStopped = false;
            return NodeResult.running;
        }
        // By default return failure
        return NodeResult.failure;
    }

    public override void OnExit()
    {
        agent.isStopped = true;
        //Debug.Log("agent.isStopped = true");

        // agent.ResetPath();
    }

    public override bool IsValid()
    {
        return !(destination.isInvalid || agent == null);
    }
}
