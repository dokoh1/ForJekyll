using MBT;
using System;
using UnityEngine;

[AddComponentMenu("")]
[MBTNode("Example/EyeType Move To Stand Position")]
public class EyeTypeMoveToStandPosition : Leaf
{
    public IntReference curState;

    public FloatReference baseSpeed;
    public FloatReference walkSpeedModifier;

    public BoolReference isDetect;

    public Vector3Reference originPosition;

    public MonsterV2 monster;
    public float stopDistance = 2f;

    public override void OnEnter()
    {
        MonsterState state = (MonsterState)curState.Value;
        if (!state.HasFlag(MonsterState.Walk))
        {
            curState.Value = (int)MonsterState.Walk;
        }
        monster.agent.speed = baseSpeed.Value * walkSpeedModifier.Value;
        monster.SetAnimation(false, true, false, false, false, false);
        monster.agent.isStopped = false;
        monster.agent.SetDestination(originPosition.Value);
    }

    public override NodeResult Execute()
    {
        if (GameManager.Instance.IsTimeStop) return NodeResult.running;

        if (isDetect.Value) return NodeResult.success;
        if (monster.agent.pathPending) return NodeResult.running;
        if (monster.agent.hasPath) return NodeResult.running;
        if (monster.agent.remainingDistance < stopDistance) return NodeResult.success;
        return NodeResult.failure;
    }

    public override void OnExit()
    {
        curState.Value = (int)MonsterState.Idle;
        monster.SetAnimation(true, false, false, false, false, false);
        monster.agent.isStopped = true;
    }
}
