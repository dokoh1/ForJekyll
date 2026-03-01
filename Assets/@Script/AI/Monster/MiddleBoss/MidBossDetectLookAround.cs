using MBT;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Detect 상태(탐지 돌입):
/// - needDetectState == true일 때만 실행.
/// - 일정 시간 제자리에서 두리번거리는 애니메이션.
/// - 실행 도중 게이지가 Rage 임계 이상이 되면 success → Rage 브랜치로.
/// </summary>
[AddComponentMenu("")]
[MBTNode("MidBoss/MidBoss Detect Look Around")]
public class MidBossDetectLookAround : Leaf
{
    public IntReference   curState;          // MonsterState용 blackboard 값
    public MidBossEarType midBoss;           // 실제 몬스터 레퍼런스

    private float _timer;  // Detect 진행 시간
    private bool  _playing; // 현재 Detect 중인지 여부

    public override void OnEnter()
    {
        if (curState.Value != (int)MidBossState.Detect)
        {
            _playing = false;
            return;
        }

        _playing = true;
        _timer   = 0f;
        
        midBoss.agent.isStopped = true;
        // LostTargetParameter를 true로 해서 "두리번" 애니메이션으로 재사용
        midBoss.SetAnimation(detect:true);
        
        base.OnEnter();
    }

    public override NodeResult Execute()
    {
        if (!_playing)
            return NodeResult.failure;
        if (curState.Value != (int)MidBossState.Detect)
            return NodeResult.success;
        return NodeResult.running;
    }
}
