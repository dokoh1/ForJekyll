using MBT;
using UnityEngine;

[AddComponentMenu("")]
[MBTNode("MidBoss/MidBoss Idle")]
public class MidBossIdle : Leaf
{
    public IntReference curState;
    public MidBossEarType midBoss;

    private bool _playing;

    public override void OnEnter()
    {
        if (curState.Value != (int)MidBossState.Idle)
        {
            _playing = false;
            return;
        }
        
        _playing = true;
        midBoss.agent.isStopped = true;
        midBoss.SetAnimation(idle: true);
    }

    public override NodeResult Execute()
    {
        if (!_playing)
            return NodeResult.failure;
        
        if (curState.Value != (int)MidBossState.Idle)
            return NodeResult.failure;
        
        return NodeResult.running;
    }
}