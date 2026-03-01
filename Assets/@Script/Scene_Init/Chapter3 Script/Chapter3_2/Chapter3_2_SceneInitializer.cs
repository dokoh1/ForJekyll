using NPC;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using DG.Tweening;
using System;
using UnityEngine.Playables;

public class Chapter3_2_SceneInitializer : ChapterBase
{
    [BoxGroup("Chapter3_2", ShowLabel = true, CenterLabel = true)]
    [TabGroup("Chapter3_2/Tabs", "GameObj")] public PlayableDirector cutScene;
    [TabGroup("Chapter3_2/Tabs", "GameObj")] public ConferenceRoomDoor_Obj[] conferenceRoomDoor;

    [TabGroup("Chapter3_2/Tabs", "Script")] public Elevator_Obj startElevatorObj;

    [BoxGroup("Chapter3_2")]
    [TabGroup("Chapter3_2/Tabs", "Monsters")] public GameObject Monsters;




    [BoxGroup("Chapter3_2")]
    [TabGroup("Chapter3_2/Tabs", "Npc")] public List<NpcInteractBase> yhjInteract;
    [TabGroup("Chapter3_2/Tabs", "Npc")] public SerializableDictionary<NpcName, Transform> npcPos;

    [Title("Player")][SerializeField] private Transform playerPos;


    private Chapter3_2_Manager _ch32Manager;

    public override void SceneClear() { ScenarioManager.Instance.SetAchieve(ScenarioAchieve.Chapter3_2, true); }
    public override void Initialize()
    {
        _ch32Manager = new Chapter3_2_Manager(this);

        DataManager.Instance.Chapter = 3;
        DataManager.Instance.sceneDataManager.AutoDataSave(10);

        GameManager.Instance.MovePlayerTransform(playerPos);
        GameManager.Instance.Player.PlayerAngle(-90);
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.PlayerMove, true);

        if (!GameManager.Instance.Player.CurState.HasFlag(PlayerEnum.PlayerState.Flash))
        {
            GameManager.Instance.Player.SetFlash();
        }

        // StartCoroutine(startElevatorObj.ElevatorOpen());

        foreach (var n in npcPos) { GameSceneManager.Instance.psManager.MoveNpc(n.Key, n.Value); }
        var yhjUnit = GameSceneManager.Instance.npcUnits[NpcName.YHJ];
        yhjUnit.gameObject.SetActive(true);
        yhjUnit.FollowStart();

        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.OnlyLook, true);
        UIManager.Instance.dialogueEnd += StartDialogEnd;
        UIManager.Instance.DialogueOpen(Dialogue.Main, false, 73);
    }

    private void StartDialogEnd()
    {
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.OnlyLook, false);
    }

    public void CutSceneEvent()
    {
        // GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.CutScenePlaying, false);
        // cutScene.gameObject.SetActive(true);
        // cutScene.Play(true);
        // cutScene.stopped += AfterCutScene;
        AfterCutScene(cutScene);
    }

    private void AfterCutScene(PlayableDirector playable)
    {
        // GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.CutScenePlaying, false);
        // cutScene.gameObject.SetActive(false);
        Monsters.SetActive(true);

        foreach (var d in conferenceRoomDoor)
        {
            d.IsInteractable = true;
            d.AddMaterial();
        }
    }
}
