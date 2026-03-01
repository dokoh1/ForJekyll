using UnityEngine;
using MBT;

/// <summary>
/// 스턴 상태 유지 노드
/// - isStunned == true인 동안 running을 반환하여 Selector 상에서 다른 브랜치가 실행되지 않도록 한다.
/// - MidBossEarType.Update()에서 스턴 타이머가 0이 되면 isStunned=false로 바뀌고, 그 다음 틱에서 success를 반환.
/// </summary>
[AddComponentMenu("")]
[MBTNode("MidBoss/MidBossStun")]
public class MidBossStun : Leaf
{
    public IntReference   curState;  // 현재 몬스터 상태
    public MidBossEarType midBoss;

    public override void OnEnter()
    {
        if (curState.Value != (int)MidBossState.Stun)
            return;
        midBoss.agent.isStopped = true;
    }

    public override NodeResult Execute()
    {
        if (curState.Value != (int)MidBossState.Stun)
            return NodeResult.failure;
        
        return NodeResult.running;
    }
    
}