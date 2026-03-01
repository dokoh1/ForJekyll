using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Chapter3_3_SceneInitializer : ChapterBase
{
    [BoxGroup("Chapter3_3", ShowLabel = true, CenterLabel = true)]
    [BoxGroup("Chapter3_3")]
    [TabGroup("Chpater3_3/Tabs", "Transform")] public Transform playerSpawnPoint;
    [TabGroup("Chapter3_3/Tabs", "Transform")] public float playerStartAngle = 0f;
    
    [BoxGroup("Chapter3_3")] 
    [TabGroup("Chapter3_3/Tabs", "Npc")]
    public SerializableDictionary<NpcName, Transform> npcTransforms;
    [TabGroup("Chapter3_3/Taps", "Npc")]
    public List<NpcInteractBase> npcInteractBase;
    // 처음 상호작용 세팅
    public override void Initialize()
    {
        GameManager.Instance.MovePlayerTransform(playerSpawnPoint);
        GameManager.Instance.playerSettingManager.ResetBool();
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.PlayerMove, true);
        //GameManager.Instance.Player.PlayerAngle();
        DataManager.Instance.sceneDataManager.AutoDataSave(10);
        foreach (var n in npcTransforms)
        {
            ConforenceScene_Manager.Instance.psManager.MoveNpc(n.Key, n.Value);
        }
        InitializeInteractData(npcInteractBase);
        var npcDict = ConforenceScene_Manager.Instance.npcUnits;
        foreach (var n in npcDict)
        {
            if (!npcDict.ContainsKey(n.Key))
                continue;
            
            n.Value.transform.LookAt(playerSpawnPoint.transform);
            if (n.Key is NpcName.KJM or NpcName.YHJ or NpcName.PSY)
            {
                n.Value.onInteractEnd += DataManager.Instance.FavorEvent;
            }
        }

        foreach (var n in ConforenceScene_Manager.Instance.npcUnits)
        {
            n.Value.onInteractEnd += InteractAll;
        }
        /*UIManager.Instance.dialogueEnd += BoolReset;*/
    }
    // 모든 NPC하고 상호작용 했는지 체크
    private void InteractAll(NpcName npcName)
    {
        ScenarioManager.Instance.npcInteracts[npcName] = true;
        if (!ScenarioManager.Instance.CheckNpcInteract())
            return;
        // 여기다가 벤트 상호작용 추후에 추가 예정
        Debug.Log("Vent 상호작용 하세요");
    }

    public override void SceneClear()
    {
        UIManager.Instance.objective.ClearAll();
        ScenarioManager.Instance.SetAchieve(ScenarioAchieve.Chapter3_3, true);
    }
}

