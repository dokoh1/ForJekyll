using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Marker;
using UnityEngine;
using UnityEngine.Playables;
using NPC;

public class Chapter1_2_SceneInitializer : ChapterBase
{
    #region C12    
    [BoxGroup("Chapter1_2", ShowLabel = true, CenterLabel = true)]

    [BoxGroup("Chapter1_2")]
    [TabGroup("Chapter1_2/Tabs", "Object")] public YHJ_Room_Obj YHJ_Door;
    [TabGroup("Chapter1_2/Tabs", "Object")] public FlashLight_Obj flashLight;
    [TabGroup("Chapter1_2/Tabs", "Object")] public GameObject flashLightAll;
    [TabGroup("Chapter1_2/Tabs", "Object")] public DeadBodyDoor_Obj Shake_Door;
    [TabGroup("Chapter1_2/Tabs", "Object")] public GameObject fireTruckEvent;
    [TabGroup("Chapter1_2/Tabs", "Object")] public GameObject fireTruckBroken;
    
    [BoxGroup("Chapter1_2")]
    [TabGroup("Chapter1_2/Tabs", "Transform")] public Transform playerMinusPos;
    [TabGroup("Chapter1_2/Tabs", "Transform")] public Transform playerTeleportPos;

    [BoxGroup("Chapter1_2")]
    [TabGroup("Chapter1_2/Tabs", "TimeLine")] public GameObject elevatorTimeLine;
    [TabGroup("Chapter1_2/Tabs", "TimeLine")] public GameObject firstMonTimeLine;
    [TabGroup("Chapter1_2/Tabs", "TimeLine")] public ColliderHandler timeLineCollider;
    [TabGroup("Chapter1_2/Tabs", "TimeLine")] public ColliderHandler centerFirstCollider;
    [TabGroup("Chapter1_2/Tabs", "TimeLine")] public ColliderHandler centerSecondCollider;
    [TabGroup("Chapter1_2/Tabs", "TimeLine")] public ColliderHandler elevetorCollider;
    [TabGroup("Chapter1_2/Tabs", "TimeLine")] public PlayableDirector shake_doorTimeLine;
    [TabGroup("Chapter1_2/Tabs", "TimeLine")] public PlayableDirector YHJ_OutCutSCenme;

    [BoxGroup("Chapter1_2")]
    [TabGroup("Chapter1_2/Tabs", "And")] public GameObject[] monsters;
    [TabGroup("Chapter1_2/Tabs", "And")] public Elevator_Obj[] elevator;
    [TabGroup("Chapter1_2/Tabs", "And")] public ColliderHandler elevatorColliderHandler;

    [BoxGroup("Chapter1_2")]
    [TabGroup("Chapter1_2/Npc", "Npc")] public List<NpcInteractBase> yhjInteract;

    [Header("QuestTarget")]
    [SerializeField] public QuestTarget elevatorButtonRight;
    [SerializeField] public QuestTarget elevatorButtonLeft;
    [SerializeField] public QuestTarget flashLightTarget;
    [SerializeField] public QuestTarget Opposite2FTarget;
    [SerializeField] public QuestTarget centerFirstTarget;
    [SerializeField] public QuestTarget centerSecondTarget;
    [SerializeField] public QuestTarget elevatorWayTarget;
    [SerializeField] public QuestTarget firstDoorTarget;
    [SerializeField] public QuestTarget YHJDoorTarget;
    private Chapter1_2_Manager _ch12Manager;

    private bool _flashOriginallyOn = false;

    public NpcUnit yhj;

    #endregion
    public override void SceneClear() { ScenarioManager.Instance.SetAchieve(ScenarioAchieve.Chapter1_2, true); }
    public override void Initialize()
    {
        _ch12Manager = new Chapter1_2_Manager(this);
        yhj.IsInjured = true;

        if (!ScenarioManager.Instance.GetAchieve(ScenarioAchieve.Chapter1_2_WithYHJ))
        {
            DataManager.Instance.sceneDataManager.AutoDataSave(10);
            elevatorTimeLine.SetActive(true);
            GameManager.Instance.MovePlayerTransform(playerMinusPos);
            GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.CutScenePlaying, true);
            flashLight.OffInteract += AddTimeLineEvent;
        }
        else
        {
            _ch12Manager.Monster_NpcSpawn();
            YHJ_Door.IsStoryEnd = true;
            flashLightAll.SetActive(false);
            elevatorTimeLine.SetActive(false);
            if (!GameManager.Instance.Player.CurState.HasFlag(PlayerEnum.PlayerState.Flash)) GameManager.Instance.Player.SetFlash();
        }
    }

    public void DeadBodyDoorCutScene() { StartCoroutine(DeadBodyDoorCutSceneCoroutine()); }

    private IEnumerator DeadBodyDoorCutSceneCoroutine()
    {
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.CutScenePlaying, true);

        Player player = GameManager.Instance.Player;
        FlashLightController flash = player.Flash;

        _flashOriginallyOn = flash.FlashObject.activeSelf;

        if (_flashOriginallyOn)
        {
            player.SetFlash(); 
            yield return new WaitForSeconds(0.5f); 
        }

        shake_doorTimeLine.stopped += OnDeadBodyDoorCutSceneEnd;
        shake_doorTimeLine.gameObject.SetActive(true);
        shake_doorTimeLine.Play();
    }

    private void OnDeadBodyDoorCutSceneEnd(PlayableDirector director)
    {
        shake_doorTimeLine.stopped -= OnDeadBodyDoorCutSceneEnd;
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.CutScenePlaying, false);

        //Player player = GameManager.Instance.Player;
        //
        //if (player.Flash.FlashObject.activeSelf)
        //    player.SetFlash();

        shake_doorTimeLine.gameObject.SetActive(false);
    }

    public void ElevatorTimeLineEnd()
    {
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.CutScenePlaying, false);
        UIManager.Instance.dialogueEnd += InteractOn;
        UIManager.Instance.DialogueOpen(Dialogue.Main, false, 145); // story
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.OnlyInteractLocked, true); // bool
        elevatorTimeLine.SetActive(false);
    }
    private void InteractOn()
    {
        flashLight.GetFlashLight += MoveOppsiteAction;
        UIManager.Instance.objective.ClearAll();
        UIManager.Instance.objective.SetMarkerByKey("Find2F");
        UIManager.Instance.objective.Follow(firstDoorTarget, ObjectiveAction.Push, 1f);
        UIManager.Instance.questUI.AddQuest("Find2F");
        NotebookManager.Instance.NotebookMission.UpdateMission("Find2F");
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.OnlyInteractLocked, false); // bool
    }
    
    private void DoorInteractEnd()
    {
        UIManager.Instance.objective.ClearAll();
        UIManager.Instance.objective.SetMarkerByKey("Find2F");
        UIManager.Instance.objective.Follow(flashLightTarget, ObjectiveAction.PickUp, 1f);
    }

    public void MoveOppsiteAction()
    {
        UIManager.Instance.objective.ClearAll();
        UIManager.Instance.objective.Follow(centerFirstTarget, ObjectiveAction.Follow, 1f);
        UIManager.Instance.objective.SetMarkerByKey("MoveOpposite2F");
    }
    
    private void AddTimeLineEvent()
    {
        centerFirstCollider.OnTriggerEntered += CenterFirstActive;
        timeLineCollider.OnTriggerEntered += TimeLineActive;
        //YHJ_Door.OffInteract += _ch12Manager.Monster_NpcSpawn;
        YHJ_Door.OffInteract += YHJ_OutCutSceneStart;
    }

    public void ElevatorTimeLineAudioActive()
    {
        SoundManager.Instance.PlayBGM(SoundManager.Instance.BGM_Source, BGM_Sound.Chapter1_2_ElevatorAfther, true, 0.4f, 0f, true);
    }

    private void CenterFirstActive()
    {
        UIManager.Instance.objective.Follow(Opposite2FTarget, ObjectiveAction.Follow, 1f);
    }

    #region YHJ_OutCutScene
    private void YHJ_OutCutSceneStart()
    {
        YHJ_OutCutSCenme.gameObject.SetActive(true);
        YHJ_OutCutSCenme.Play();
        GameManager.Instance.Player.Flash.FlashObject.SetActive(false);
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.CutScenePlaying, true);
        YHJ_OutCutSCenme.stopped += YHJ_CutSceneEnd;
    } 
    public void YHJ_OutCutScene_ing() { StartCoroutine(GameManager.Instance.fadeManager.FadeStart(FadeState.FadeOut)); }
    private void YHJ_CutSceneEnd(PlayableDirector p)
    {
        GameManager.Instance.Player.Flash.FlashObject.SetActive(true);
        StartCoroutine(GameManager.Instance.fadeManager.FadeStart(FadeState.FadeIn));
        YHJ_OutCutSCenme.gameObject.SetActive(false);
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.CutScenePlaying, false);
        _ch12Manager.Monster_NpcSpawn();
        fireTruckEvent.SetActive(false);
        fireTruckBroken.SetActive(true);
    }
    #endregion
    
    #region FirstMonTimeLine
    private void TimeLineActive()
    {
        UIManager.Instance.objective.Follow(YHJDoorTarget, ObjectiveAction.Push, 1f);
        UIManager.Instance.objective.SetMarkerByKey("MoveOpposite2F");
        SoundManager.Instance.StopAllBGM();
        UIManager.Instance.DialogueOpen(Dialogue.Main, false, 152);
        UIManager.Instance.dialogueEnd += DialgouEnd;
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.OnlyInteractLocked, true);
        
        //gameManager.fadeManager.fadeComplete += TimeLineStart;
        //StartCoroutine(gameManager.fadeManager.FadeStart(FadeState.FadeOut));
    }

    //임시
    private void DialgouEnd() { GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.OnlyInteractLocked, false); }
    private void TimeLineStart()
    {
        SoundManager.Instance.StopAllBGM();
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.CutScenePlaying, true); // bool
        GameManager.Instance.fadeManager.FadeStart(FadeState.FadeIn, 0.5f);
        if (GameManager.Instance.Player.CurState.HasFlag(PlayerEnum.PlayerState.Flash)) GameManager.Instance.Player.SetFlash();
        firstMonTimeLine.SetActive(true);
    }
    public void TimeLineTalkA() { UIManager.Instance.DialogueOpen(Dialogue.Main, false, 148); }
    public void TimeLineTalkB() { UIManager.Instance.DialogueOpen(Dialogue.Main, false, 150); }
    public void TimeLineTalkC() { UIManager.Instance.DialogueOpen(Dialogue.Main, false, 152); }
    public void TimeLineActiveEnd()
    {
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.CutScenePlaying, false);
        YHJ_Door.Interact();
        firstMonTimeLine.SetActive(false);
    }
    #endregion
}
