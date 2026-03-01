using NPC;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using DG.Tweening;


public class Chapter3_1_SceneInitializer : ChapterBase
{
    [BoxGroup("Chapter3_1", ShowLabel = true, CenterLabel = true)]

    [TabGroup("Chapter3_1/Tabs", "Script")] public StoryLockDoor_Obj[] controlRoomDoor;
    [TabGroup("Chapter3_1/Tabs", "Script")] public Door_Obj[] broadcastRoomDoor;
    [TabGroup("Chapter3_1/Tabs", "Script")] public Elevator_Obj[] endElevatorObj;

    [BoxGroup("Chapter3_1")]
    [TabGroup("Chapter3_1/Tabs", "Npc")] public List<NpcInteractBase> yhjInteract;
    [TabGroup("Chapter3_1/Tabs", "Npc")] public List<NpcInteractBase> yhj2Interact;
    [TabGroup("Chapter3_1/Tabs", "Npc")] public SerializableDictionary<NpcName, Transform> npcPos;
    [TabGroup("Chapter3_1/Tabs", "Npc")] public NPCDataSO yhjSO;

    [Title("EventList")]
    [BoxGroup("Chapter3_1")]
    [TabGroup("Chapter3_1/Event", "EventList")] public Transform[] YHJDestinationPos;
    [TabGroup("Chapter3_1/Event", "EventList")] public Password_obj password;
    [Title("Player")][SerializeField] private Transform playerPos;


    private Chapter3_1_Manager _ch31Manager;
    private NpcUnit yhjUnit;
    private bool isOpenControlRoom = false;
    private float baseSpeed;


    public override void SceneClear() { ScenarioManager.Instance.SetAchieve(ScenarioAchieve.Chapter3_1, true); }
    public override void Initialize()
    {
        _ch31Manager = new Chapter3_1_Manager(this);
        DataManager.Instance.Chapter = 3;
        DataManager.Instance.sceneDataManager.AutoDataSave(10);

        GameManager.Instance.MovePlayerTransform(playerPos);
        GameManager.Instance.Player.PlayerAngle(-90);
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.PlayerMove, true);

        if (!GameManager.Instance.Player.CurState.HasFlag(PlayerEnum.PlayerState.Flash))
        {
            GameManager.Instance.Player.SetFlash();
        }

        DoorControl();

        foreach (var n in npcPos) { GameSceneManager.Instance.psManager.MoveNpc(n.Key, n.Value); }
        yhjUnit = GameSceneManager.Instance.npcUnits[NpcName.YHJ];
        yhjUnit.gameObject.SetActive(true);
        var kjmUnit = GameSceneManager.Instance.npcUnits[NpcName.KJM];
        kjmUnit.gameObject.SetActive(true);
        kjmUnit.FollowStart();


        yhjUnit.onInteractEnd += YHJInteractionEvent;
        InitializeInteractData(yhjInteract);
        yhjUnit.destination = YHJDestinationPos[0];

        password.OnPasswordUsed += GetPassword;
    }

    private void DoorControl()
    {
        foreach (var d in controlRoomDoor)
        {
            d.storyLock = true;
        }

        foreach (var d in broadcastRoomDoor)
        {
            d.canOpen = true;
        }
    }

    private void YHJInteractionEvent(NpcName name)
    {
        yhjUnit.IsInteractable = false;
        yhjUnit.onInteractEnd -= YHJInteractionEvent;

        foreach (var d in controlRoomDoor)
        {
            d.storyLock = false;
            d.canOpen = true;
            d.IsInteractable = true;
            d.endOpen += () => { ControlRoomEvent(); };
        }

        if (yhjSO != null)
        {
            baseSpeed = yhjSO.Data.BaseSpeed;
            yhjSO.Data.RunSpeedModifier = 2.0f;
        }

        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(2f);
        sequence.AppendCallback(() => { GameSceneManager.Instance.npcUnits[NpcName.YHJ].MoveToDestination(); });
        sequence.Play();
    }

    private void ControlRoomEvent()
    {
        if (isOpenControlRoom)
            return;
        isOpenControlRoom = true;

        yhjUnit.destination = YHJDestinationPos[1];
        GameSceneManager.Instance.psManager.MoveNpc(NpcName.YHJ, YHJDestinationPos[0]);
        GameSceneManager.Instance.npcUnits[NpcName.YHJ].MoveToDestination();
        InitializeInteractData(yhj2Interact);
        yhjUnit.onInteractEnd += ControlRoomTalkEvent;
    }

    private void ControlRoomTalkEvent(NpcName name)
    {
        yhjUnit.IsInteractable = false;
        yhjUnit.onInteractEnd -= ControlRoomTalkEvent;
    }

    public void GetPassword()
    {
        if (yhjSO != null)
        {
            yhjSO.Data.RunSpeedModifier = baseSpeed;
        }

        foreach (var e in endElevatorObj)
        {
            e.IsInteractable = true;
            e.sceneEnum = SceneEnum.Demo;
            e.OffInteract += SceneClear;
        }
    }
}
