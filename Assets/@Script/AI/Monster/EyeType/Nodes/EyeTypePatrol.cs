using MBT;
using System;
using UnityEngine;

[AddComponentMenu("")]
[MBTNode("Example/EyeType Patrol")]
public class EyeTypePatrol : Leaf
{
    public BoolReference isDetect;

    public FloatReference baseSpeed;
    public FloatReference walkSpeedModifier;

    public IntReference curState;

    public MonsterV2 monster;
    public float stopDistance = 2f;
    private Vector3 _destination;
    private bool _isMoveFail;

    public override void OnEnter()
    {
        _isMoveFail = false;
        MonsterState state = (MonsterState)curState.Value;
        if (state != MonsterState.Walk)
        {
            curState.Value = (int)MonsterState.Walk;
        }

        monster.SetAnimation(false, true, false, false, false, false);
  
        _destination = monster.GetPatrolPosition();
        monster.agent.speed = baseSpeed.Value * walkSpeedModifier.Value;
        monster.agent.isStopped = false;
        _isMoveFail = monster.MoveToDestination(_destination);
    }

    public override NodeResult Execute()
    {
        if (GameManager.Instance.IsTimeStop) return NodeResult.running;

        if (isDetect.Value || _isMoveFail)
        {
            return NodeResult.success;
        }

        if (monster.agent.pathPending)
        {
            //Debug.Log($"MonsterPatrol - Execute() - monster.agent.pathPending : {monster.agent.pathPending}");
            return NodeResult.running;
        }

        if (monster.agent.hasPath)
        {
            //Debug.Log($"MonsterPatrol - Execute() - monster.agent.hasPath : {monster.agent.hasPath}");
            return NodeResult.running;
        }

        if (monster.agent.remainingDistance < stopDistance)
        {
            //Debug.Log($"MonsterPatrol - Execute() - monster.agent.remainingDistance : {monster.agent.remainingDistance} < stopDistance : {stopDistance}");
            return NodeResult.success;
        }

        return NodeResult.running;
    }
}