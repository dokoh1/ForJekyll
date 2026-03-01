using MBT;
using UnityEngine;

[AddComponentMenu("")]
[MBTNode("Example/Focus Around Wait")]
public class FocusAroundWait : Leaf
{
    public IntReference curState;
    public BoolReference isFocusAround;
    public BoolReference isDetect;
    public BoolReference canAttack;
    public FloatReference focusTime;
    public Monster monster;
    private float _timer;

    public override void OnEnter()
    {
        curState.Value = (int)MonsterState.Lost;
        isDetect.Value = false;
        _timer = 0f;
        monster.agent.isStopped = true;
        monster.SetAnimation(false, false, false, true);
    }

    public override NodeResult Execute()
    {
        if (canAttack.Value || isDetect.Value)
        {
            isFocusAround.Value = false;
            return NodeResult.success;
        }

        if (_timer >= focusTime.Value)
        {
            isFocusAround.Value = false;
            return NodeResult.success;
        }

        _timer += this.DeltaTime;
        return NodeResult.running;
    }
}
