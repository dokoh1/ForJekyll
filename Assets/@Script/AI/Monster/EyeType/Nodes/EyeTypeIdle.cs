using MBT;
using UnityEngine;

[AddComponentMenu("")]
[MBTNode("Example/EyeType Idle")]
public class EyeTypeIdle : Wait
{
    public IntReference curState;
    public BoolReference isDetect;

    public MonsterV2 monster;

    public override void OnEnter()
    {
        base.OnEnter();
        monster.agent.isStopped = true;
        monster.agent.ResetPath();
        curState.Value = (int)MonsterState.Idle;

        monster.SetAnimation(true, false, false, false, false, false);
    }

    public override NodeResult Execute()
    {
        if (GameManager.Instance.IsTimeStop) return NodeResult.running;

        if (isDetect.Value) return NodeResult.success;
        return base.Execute();
    }
}