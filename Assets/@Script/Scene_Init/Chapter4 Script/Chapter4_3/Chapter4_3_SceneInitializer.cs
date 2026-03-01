using NPC;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using DG.Tweening;
using System;
using UnityEngine.Playables;

public class Chapter4_3_SceneInitializer : ChapterBase
{
    [TabGroup("Chapter4_3/Tabs", "GameObj")] public Door_Obj[] initializeDoor;
    [TabGroup("Chapter4_3/Tabs", "GameObj")] public StoryLockDoor_Obj EndDoor;

    [BoxGroup("Chapter4_3")]
    [TabGroup("Chapter4_3/Tabs", "Npc")] public List<NpcInteractBase> npcInteract;
    [TabGroup("Chapter4_3/Tabs", "Npc")] public SerializableDictionary<NpcName, Transform> npcPos;

    [Title("Player")][SerializeField] private Transform playerPos;

    private Dictionary<NpcName, bool> npcInteractionStatus = new Dictionary<NpcName, bool>();
    private bool allInteractCompleted = false;

    public override void SceneClear() { ScenarioManager.Instance.SetAchieve(ScenarioAchieve.Chapter4_3, true); }
    public override void Initialize()
    {
        DataManager.Instance.Chapter = 4;
        DataManager.Instance.sceneDataManager.AutoDataSave(10);

        GameManager.Instance.MovePlayerTransform(playerPos);
        GameManager.Instance.Player.PlayerAngle(-90);
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.PlayerMove, true);

        if (!GameManager.Instance.Player.CurState.HasFlag(PlayerEnum.PlayerState.Flash))
        {
            GameManager.Instance.Player.SetFlash();
        }

        foreach (var n in npcPos) { GameSceneManager.Instance.psManager.MoveNpc(n.Key, n.Value); }

        InitializeInteractData(npcInteract);
        InitializeNpcTracking();

        var npcDict = GameSceneManager.Instance.npcUnits;

        npcDict[NpcName.KJM].transform.LookAt(playerPos);
        npcDict[NpcName.KJM].onInteractEnd += InteractAll;

        npcDict[NpcName.PSY].transform.LookAt(
            npcDict[NpcName.PJH].transform.position
        );
        npcDict[NpcName.PSY].onInteractEnd += InteractAll;

        npcDict[NpcName.PJH].transform.LookAt(
            npcDict[NpcName.PSY].transform.position
        );
        npcDict[NpcName.PJH].onInteractEnd += InteractAll;

        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.OnlyLook, true);
        UIManager.Instance.dialogueEnd += StartDialogEnd;
        UIManager.Instance.DialogueOpen(Dialogue.Main, false, 73);

        InitializeDoor();
    }

    private void InitializeNpcTracking()
    {
        npcInteractionStatus.Clear();
        allInteractCompleted = false;

        // 이 챕터에서 interact해야 하는 NPC들
        npcInteractionStatus[NpcName.KJM] = false;
        npcInteractionStatus[NpcName.PSY] = false;
        npcInteractionStatus[NpcName.PJH] = false;

        Debug.Log($"[Chapter4_3] Initialized tracking for {npcInteractionStatus.Count} NPCs");
    }

    private void StartDialogEnd()
    {
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.OnlyLook, false);
    }

    private void InitializeDoor()
    {
        foreach (var door in initializeDoor)
        {
            door.IsInteractable = true;
        }
    }


    private void InteractAll(NpcName npcName)
    {
        // 이미 완료된 경우 무시
        if (allInteractCompleted)
        {
            Debug.Log($"[Chapter4_3] All interactions already completed, ignoring {npcName}");
            return;
        }

        // Dictionary에 해당 NPC가 없는 경우
        if (!npcInteractionStatus.ContainsKey(npcName))
        {
            Debug.LogWarning($"[Chapter4_3] NPC {npcName} not tracked in this chapter!");
            return;
        }

        // 이미 interact한 NPC인 경우
        if (npcInteractionStatus[npcName])
        {
            Debug.Log($"[Chapter4_3] Already interacted with {npcName}");
            return;
        }

        Debug.Log($"[Chapter4_3] Interaction with {npcName}");
        npcInteractionStatus[npcName] = true;

        // 모든 NPC와 interact했는지 체크
        if (!CheckAllNpcInteracted())
            return;

        Debug.Log("[Chapter4_3] All NPC interactions completed!");
        allInteractCompleted = true;
        OnAllNpcInteractEnd();
    }

    private bool CheckAllNpcInteracted()
    {
        foreach (var status in npcInteractionStatus.Values)
        {
            if (!status)
            {
                return false;
            }
        }
        return true;
    }

    private void OnAllNpcInteractEnd()
    {
        Debug.Log("change door");
        EndDoor.IsInteractable = true;
        EndDoor.storyLock = false;
        EndDoor.isFinishChapter = true;
        EndDoor.sceneEnum = SceneEnum.Demo;
    }

    private void CleanupEvents()
    {
        var npcDict = GameSceneManager.Instance.npcUnits;

        if (npcDict.ContainsKey(NpcName.KJM))
            npcDict[NpcName.KJM].onInteractEnd -= InteractAll;

        if (npcDict.ContainsKey(NpcName.PSY))
            npcDict[NpcName.PSY].onInteractEnd -= InteractAll;

        if (npcDict.ContainsKey(NpcName.PJH))
            npcDict[NpcName.PJH].onInteractEnd -= InteractAll;
    }

    private void OnDestroy()
    {
        CleanupEvents();
        UIManager.Instance.dialogueEnd -= StartDialogEnd;
    }
}