using MBT;
using UnityEngine;

[AddComponentMenu("")]
[MBTNode("Example/Monster Move To Destination")]
public class MonsterMoveToDestination : Leaf
{
    public IntReference curState;
    public FloatReference baseSpeed;
    public FloatReference runSpeedModifier;
    public BoolReference haveDestination;
    public BoolReference isDetect;
    public BoolReference isOriginPosition;
    public Vector3Reference destination;

    public Monster monster;
    public float stopDistance = 1f;
    private bool _isMoveFail;

    public override void OnEnter()
    {
        _isMoveFail = false;
        MonsterState state = (MonsterState)curState.Value;
        if (!state.HasFlag(MonsterState.Run))
        {
            curState.Value = (int)MonsterState.Run;
        }
        monster.SetAnimation(false, false, true, false, false);
        monster.agent.speed = baseSpeed.Value * runSpeedModifier.Value;
        monster.agent.isStopped = false;
        _isMoveFail = monster.MoveToDestination(destination.Value);
    }

    public override NodeResult Execute()
    {
        if(_isMoveFail) return NodeResult.failure;
        if(isDetect.Value) return NodeResult.success;
        // if (monster.Sound.GroundChange) monster.Sound.PlayStepSound((MonsterState)curState.Value);
        if (monster.agent.pathPending) return NodeResult.running;
        if (monster.agent.hasPath) return NodeResult.running;
        if (monster.agent.remainingDistance < stopDistance) return NodeResult.success;
        return NodeResult.failure;
    }

    public override void OnExit()
    {
        haveDestination.Value = false;
        isOriginPosition.Value = false;
    }
}
