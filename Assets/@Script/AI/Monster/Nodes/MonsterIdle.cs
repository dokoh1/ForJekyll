using MBT;
using UnityEngine;

[AddComponentMenu("")]
[MBTNode("Example/Monster Idle")]
public class MonsterIdle : Wait
{
    public IntReference curState;
    public IntReference monsterType;
    public BoolReference skipValueIsDetect;
    public BoolReference canAttack;
    public Monster monster;

    public override void OnEnter()
    {
        base.OnEnter();
        monster.agent.isStopped = true;
        monster.agent.ResetPath();
        curState.Value = (int)MonsterState.Idle;
        if (monsterType.Value == 1) monster.SetAnimation(true, false, false, false, false);
        else monster.SetAnimation(true, false, false, false);
    }

    public override NodeResult Execute()
    {
        if (skipValueIsDetect.Value || canAttack.Value) return NodeResult.failure;
        return base.Execute();
    }
}
