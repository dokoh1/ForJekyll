using MBT;
using UnityEngine;

[AddComponentMenu("")]
[MBTNode("Example/Monster Patrol")]
public class MonsterPatrol : Leaf
{
    public BoolReference skipValueIsDetect;
    public FloatReference baseSpeed;
    public FloatReference walkSpeedModifier;
    public IntReference curState;
    public IntReference monsterType;
    public BoolReference canAttack;
    public Monster monster;
    public float stopDistance = 2f;
    private Vector3 _destination;
    private bool _isMoveFail;

    public override void OnEnter()
    {
        _isMoveFail = false;
        MonsterState state = (MonsterState)curState.Value;
        if (state != MonsterState.Walk) curState.Value = (int)MonsterState.Walk;
        if (monsterType.Value == 1) monster.SetAnimation(false, true, false, false, false);
        else monster.SetAnimation(false, true, false, false);
        _destination = monster.GetPatrolPosition();
        monster.agent.speed = baseSpeed.Value * walkSpeedModifier.Value;
        monster.agent.isStopped = false;
        _isMoveFail = monster.MoveToDestination(_destination, 2.5f);
    }

    public override NodeResult Execute()
    {
        if (skipValueIsDetect.Value || canAttack.Value || _isMoveFail) return NodeResult.success;
        if (monster.agent.pathPending) return NodeResult.running;
        if (monster.agent.hasPath) return NodeResult.running;
        if (monster.agent.remainingDistance < stopDistance) return NodeResult.success;
        return NodeResult.running;
    }
}
