using MBT;
using UnityEngine;

[AddComponentMenu("")]
[MBTNode("Example/EyeType Rest")]
public class EyeTypeRest : Leaf
{
    public IntReference curState;
    public BoolReference isDetect;
    public MonsterV2 monster;

    public override void OnEnter()
    {
        MonsterState state = (MonsterState)curState.Value;
        if (!state.HasFlag(MonsterState.Idle))
        {
            monster.agent.isStopped = true;
            curState.Value = (int)MonsterState.Idle;
        }
        monster.SetAnimation(true, false, false, false, false, false);
    }

    public override NodeResult Execute()
    {
        if (GameManager.Instance.IsTimeStop) return NodeResult.running;
        if (isDetect.Value) return NodeResult.success;
        return NodeResult.running;
    }
}