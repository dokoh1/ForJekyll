using MBT;
using UnityEngine;

[AddComponentMenu("")]
[MBTNode("Example/Monster Move To Stand Position")]
public class MonsterMoveToStandPosition : Leaf
{
    public IntReference curState;
    public FloatReference baseSpeed;
    public FloatReference walkSpeedModifier;
    public BoolReference isDetect;
    public Vector3Reference originPosition;

    public Monster monster;
    public float stopDistance = 2f;

    public override void OnEnter()
    {
        MonsterState state = (MonsterState)curState.Value;
        if (!state.HasFlag(MonsterState.Walk))
        {
            // monster.Sound.StopStepAudio();
            curState.Value = (int)MonsterState.Walk;
        }
        // monster.Sound.PlayStepSound((MonsterState)curState.Value);
        monster.agent.speed = baseSpeed.Value * walkSpeedModifier.Value;
        monster.SetAnimation(false, true, false, false, false);
        monster.agent.isStopped = false;
        monster.agent.SetDestination(originPosition.Value);
    }

    public override NodeResult Execute()
    {
        if(isDetect.Value) return NodeResult.success;
        // if (monster.Sound.GroundChange) monster.Sound.PlayStepSound((MonsterState)curState.Value);
        if (monster.agent.pathPending) return NodeResult.running;
        if (monster.agent.hasPath) return NodeResult.running;
        if (monster.agent.remainingDistance < stopDistance) return NodeResult.success;
        return NodeResult.failure;
    }

    public override void OnExit()
    {
        curState.Value = (int)MonsterState.Idle;
        monster.SetAnimation(true, false, false, false, false);
        // monster.Sound.StopStepAudio();
        monster.agent.isStopped = true;
    }
}
