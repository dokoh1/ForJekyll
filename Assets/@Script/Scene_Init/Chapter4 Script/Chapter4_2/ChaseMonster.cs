using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseMonster : EyeTypeMonsterV2
{
    protected override void Start()
    {
        agent.enabled = true;
        base.Start();
        ForceChasePlayer();
    }

    private void ForceChasePlayer()
    {
        if (BB == null) return;

        // 플레이어를 강제로 target으로 설정
        SetVariable("target", GameManager.Instance.Player.transform);

        // 플레이어 감지됨
        SetVariable("isDetect", true);
        SetVariable("isLost", false);

        // 몬스터는 계속 일을 하고 있음
        SetVariable("isWork", true);

        // 패트롤 없음
        SetVariable("canPatrol", false);
        SetVariable("haveTarget", false); // NPC 추적 OFF

        // 목적지와 관련된 기능 비활성화
        SetVariable("haveDestination", false);

        Debug.Log($"{name}: EyeTypeMonsterV2 now set to Player-Chase mode.");
    }
}