using MBT;
using UnityEngine;

[AddComponentMenu("")]
[MBTNode("Example/Monster Move To Target")]
public class MonsterMoveToTarget : MoveToTransform
{
    public IntReference curState;
    //public IntReference monsterType;
    public FloatReference baseSpeed;
    public FloatReference runSpeedModifier;
    public BoolReference isDetect;
    public BoolReference haveTarget;
    public BoolReference isOriginPosition;

    public Monster monster;
    private bool _isMoveFail;

    public override void OnEnter()
    {
        _isMoveFail = false;
        //MonsterState state = (MonsterState)curState.Value;
        //if (!state.HasFlag(MonsterState.Run))
        //{
        //    monster.Sound.StopStepAudio();
        //    curState.Value = (int)MonsterState.Run;
        //}
        //monster.Sound.PlayStepSound((MonsterState)curState.Value);

        MonsterState state = (MonsterState)curState.Value;
        if (!state.HasFlag(MonsterState.Walk))
        {
            // monster.Sound.StopStepAudio();
            curState.Value = (int)MonsterState.Walk;
        }

        // monster.Sound.PlayStepSound(MonsterState.Run);


        agent.speed = baseSpeed.Value * runSpeedModifier.Value;
        monster.SetAnimation(false, false, true, false, false);
        base.OnEnter();
    }

    public override NodeResult Execute()
    {
        if (_isMoveFail) return NodeResult.failure;
        if (isDetect.Value) return NodeResult.success;
        // if (monster.Sound.GroundChange) monster.Sound.PlayStepSound(MonsterState.Run);

        time += Time.deltaTime;
        if (time > updateInterval)
        {
            time = 0;
            _isMoveFail = monster.MoveToDestination(destination.Value.position);
        }

        if (agent.pathPending) return NodeResult.running;
        if (agent.hasPath) return NodeResult.running;
        if (agent.remainingDistance < stopDistance) return NodeResult.success;
        return NodeResult.failure;
    }

    public override void OnExit()
    {
        base.OnExit();
        haveTarget.Value = false;
        isOriginPosition.Value = false;
    }
}
