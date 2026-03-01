using MBT;
using UnityEngine;

[AddComponentMenu("")]
[MBTNode("Example/Monster Move Noise Target")]
public class MonsterMoveNoiseTarget : Leaf
{
    public IntReference curState;
    public FloatReference baseSpeed;
    public FloatReference runSpeedModifier;
    public BoolReference canAttack;
    public BoolReference isFocusAround;
    public BoolReference isNoiseDetect;
    public BoolReference isTargetPlayer;
    public Vector3Reference destination;
    public TransformReference player;
    public float stopDistance = 1.5f;
    public float updateInterval = 1f;
    public Monster monster;
    private float time = 0;
    private bool _isMoveFail;

    public override void OnEnter()
    {
        _isMoveFail = false;
        MonsterState state = (MonsterState)curState.Value;
        if (state != MonsterState.Run) curState.Value = (int)MonsterState.Run;
        monster.SetAnimation(false, false, true, false);
        time = 0;
        monster.agent.isStopped = false;
        monster.agent.speed = baseSpeed.Value * runSpeedModifier.Value;
        _isMoveFail = monster.MoveToDestination(destination.Value, 2.5f);
    }

    public override NodeResult Execute()
    {
        if (canAttack.Value) return NodeResult.success;
        time += Time.deltaTime;
        if (time > updateInterval)
        {
            time = 0;
            _isMoveFail = monster.MoveToDestination(destination.Value, 2.5f);
        }
        if (_isMoveFail) return NodeResult.failure;
        if (monster.agent.pathPending) return NodeResult.running;
        if (monster.agent.hasPath) return NodeResult.running;
        if (monster.agent.remainingDistance < stopDistance)
        {
            isNoiseDetect.Value = false;
            isFocusAround.Value = true;
            return NodeResult.failure;
        }
        return NodeResult.failure;
    }
}
