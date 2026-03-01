using MBT;
using UnityEngine;

[AddComponentMenu("")]
[MBTNode("Example/Monster Rest")]
public class MonsterRest : Leaf
{
    public IntReference curState;
    public BoolReference isDetect;

    public Monster monster;
    public bool eat;

    public override void OnEnter()
    {
        //Debug.Log($"MonsterRest - OnEnter");
        MonsterState state = (MonsterState)curState.Value;
        if (!state.HasFlag(MonsterState.Idle))
        {
            monster.agent.isStopped = true;
            // monster.Sound.StopStepAudio();
            curState.Value = (int)MonsterState.Idle;
        }

        if (eat)
        {
            // monster.Sound.OtherSoundPlay("Eat");
        }


        monster.SetAnimation(true, false, false, false, false);
    }

    public override NodeResult Execute()
    {
        if (isDetect.Value) return NodeResult.success;
        return NodeResult.running;
    }

    public override void OnExit()
    {
        //Debug.Log($"MonsterRest - OnExit");
        if (eat)
        {
            // monster.Sound.otherAS.Stop();
        }
    }
}
