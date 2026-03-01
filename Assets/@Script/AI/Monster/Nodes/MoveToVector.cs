using MBT;
using UnityEngine;
using UnityEngine.AI;

[AddComponentMenu("")]
[MBTNode("Example/Move To Vector")]
public class MoveToVector : Leaf
{
    public Vector3Reference destination;
    public NavMeshAgent agent;
    public float stopDistance = 2f;// SO
    public float updateInterval = 1f;
    protected float time = 0;

    //public IntReference state;

    public override void OnEnter()
    {
        time = 0;
        agent.isStopped = false;
        agent.SetDestination(destination.Value);
    }

    public override NodeResult Execute()
    {
        time += Time.deltaTime;
        if (time > updateInterval)
        {
            time = 0;
            agent.SetDestination(destination.Value);
        }
        if (agent.pathPending)
        {
            return NodeResult.running;
        }
        if (agent.remainingDistance < stopDistance)
        {
            //Debug.Log("MoveToVector - Execute() - 도착 성공");
            return NodeResult.success;
        }
        if (agent.hasPath)
        {
            return NodeResult.running;
        }

        //Debug.Log("MoveToVector - Execute() - 이동 중");
        return NodeResult.running;
    }

    public override void OnExit()
    {
        agent.isStopped = true;
        // agent.ResetPath();

        //Debug.Log("MoveNavmeshAgent - OnExit()");
    }
}
