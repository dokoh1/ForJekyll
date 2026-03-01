using Item;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Marker;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.Serialization;
using static DistributionBox_Obj;

public enum ParkingEventType
{
    CarNoise,
    EarTypeShow,
    BouncerInteract,
    WayPoint2,
}

public enum ParkingIndicatorState
{
    B1_1,
    B1_PowerRoomPoint,
    B1_GenerationPoint,
    B1_7,
    B2_1,
    B2_PowerRoomPoint,
    B2_GenerationPoint,
    B2_PuzzlePoint,
    FindKey,
    B2_9,
    B1_Diary,
}

public class Chapter2_3_SceneInitializer : ChapterBase
{
    [BoxGroup("Chapter2_3", ShowLabel = true, CenterLabel = true)] [TabGroup("Chapter2_3/Tabs", "Script")]
    public DistributionBox_Obj distributionBox1F;

    [TabGroup("Chapter2_3/Tabs", "Script")]
    public DistributionBox_Obj distributionBox2F;

    [TabGroup("Chapter2_3/Tabs", "Script")]
    public Elevator_Obj startElevatorObj;

    [TabGroup("Chapter2_3/Tabs", "Script")]
    public Elevator_Obj[] endElevatorObj;

    [TabGroup("Chapter2_3/Tabs", "Script")]
    public StoryLockDoor_Obj[] endDoorObj;

    [TabGroup("Chapter2_3/Tabs", "Script")]
    public GeneratorPuzzle generatorPuzzle;

    [TabGroup("Chapter2_3/Tabs", "Script")]
    public B2Key_Obj b2keyObj;

    [TabGroup("Chapter2_3/Tabs", "Script")]
    public GameObject clearWall;

    [TabGroup("Chapter2_3/Tabs", "Script")]
    public GeneratorDoor_Obj[] b2Doors;

    [TabGroup("Chapter2_3/Tabs", "Script")]
    public Door_Obj[] b1Doors;

    [TabGroup("Chapter2_3/Tabs", "Script")]
    public Diary_Obj diary;
    
    [TabGroup("Chapter2_3/Tabs", "Script")]
    public EmerganceyDoor_Obj emerganceyDoor;

    [TabGroup("Chapter2_3/Tabs", "Script")]
    public ColliderHandler cutSceneCollider;

    [TabGroup("Chapter2_3/Tabs", "Script")]
    public ColliderHandler B1_7Collider;

    [TabGroup("Chapter2_3/Tabs", "Script")]
    public ColliderHandler B2_1Collider;
    
    [TabGroup("Chapter2_3/Tabs", "Script")]
    
    [BoxGroup("Chapter2_3")] [TabGroup("Chapter2_3/Tabs", "Npc")]
    public List<NpcInteractBase> hmsInteract;

    [TabGroup("Chapter2_3/Tabs", "Npc")] public SerializableDictionary<NpcName, Transform> npcPos;

    [BoxGroup("Chapter2_3")] [TabGroup("Chapter2_3/Tabs", "EarMonster")]
    public EarTypeMonster earMonster;

    [BoxGroup("Chapter2_3")] [TabGroup("Chapter2_3/Tabs", "QuestTarget")]
    public SerializableDictionary<ParkingIndicatorState, QuestTarget> questTarget;
    
    [Title("EventList")] [BoxGroup("Chapter2_3")] [TabGroup("Chapter2_3/Event", "EventList")]
    public SerializableDictionary<ParkingEventType, ColliderHandler> eventHandlers;

    [TabGroup("Chapter2_3/Event", "EventList")]
    public CarObject carNoise;

    [TabGroup("Chapter2_3/Event", "EventList")]
    public GameObject earTypeMonsterShow;

    [TabGroup("Chapter2_3/Event", "EventList")]
    public AudioSource[] speakers;

    [TabGroup("Chapter2_3/Event", "EventList")]
    public AudioClip broadcasting;

    [TabGroup("Chapter2_3/Event", "EventList")]
    public SerializableDictionary<ParkingEventType, DialInteract_Obj> interactHandles;

    [TabGroup("Chapter2_3/Event", "EventList")]
    public AudioClip monsterSound;

    [TabGroup("Chapter2_3/Event", "EventList")]
    public Flicker[] lightsObj;

    [TabGroup("Chapter2_3/Event", "EventList")]
    public CarObject[] carObjects;
    

    [Title("Player")] [SerializeField] private Transform playerPos;
    [SerializeField] private Transform generatorPlayerPos;

    public AnimationCurve lookCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private bool enterRoom = false;

    private Chapter2_3_Manager _ch23Manager;

    public override void SceneClear()
    {
        ScenarioManager.Instance.SetAchieve(ScenarioAchieve.Chapter2_3, true);
    }

    public override void Initialize()
    {
        _ch23Manager = new Chapter2_3_Manager(this, questTarget, B1_7Collider, B2_1Collider);
        _ch23Manager.EventInitialize(eventHandlers);
        DataManager.Instance.sceneDataManager.AutoDataSave(10);
        GameManager.Instance.MovePlayerTransform(playerPos);
        GameManager.Instance.Player.PlayerAngle(90);
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.PlayerMove, true);
        foreach (var n in npcPos)
        {
            GameSceneManager.Instance.psManager.MoveNpc(n.Key, n.Value);
        }

        GameSceneManager.Instance.npcUnits[NpcName.HMS].gameObject.SetActive(true);
        GameSceneManager.Instance.npcUnits[NpcName.HMS].FollowStart();

        StartCoroutine(ElevatorThenSetObjective());
        cutSceneCollider.OnTriggerEntered += CutSceneTrigger;
        diary.OnInteract += _ch23Manager.OnB1DiaryOn;
        distributionBox1F.CheckBox += OnDistributionBoxCheck;
        distributionBox2F.CheckBox += OnDistributionBoxCheck;

        b2keyObj.PowerRoomOpen += OpenRoom;

        foreach (var door in b2Doors)
        {
            door.OnDoorOpened += EnterB2GeneratorRoom;
            door.OnDoorOpened += _ch23Manager.OnEnterB2GeneratorRoomObjective;
            door.OnNotKeyDoor += _ch23Manager.OnFindKey;
        }

        foreach (var door in b1Doors)
            door.endOpen += _ch23Manager.OnEnterB1GeneratorRoom;

        generatorPuzzle.onClear += OnPuzzleClear;
        generatorPuzzle.onClear += _ch23Manager.OnPuzzleClearObjective;
        UIManager.Instance.dialogueEnd += AddInteract;

        interactHandles[ParkingEventType.BouncerInteract].OffInteract += GetSecurityOfficeKey;
    }

    private IEnumerator ElevatorThenSetObjective()
    {
        yield return StartCoroutine(startElevatorObj.ElevatorOpen());
        UIManager.Instance.objective.SetMarkerByKey("FindPowerRoom");
        UIManager.Instance.objective.Follow(questTarget[ParkingIndicatorState.B1_1], ObjectiveAction.Follow, 1f);
    }

    //처음 컷신 트리거 지점
    //아야님 추후에 컷신 이 함수에 추가해주세요.
    private void CutSceneTrigger()
    {
        //컷신 이후
        UIManager.Instance.objective.ClearAll();
        UIManager.Instance.objective.SetMarkerByKey("FindPowerRoom");
        UIManager.Instance.objective.Follow(questTarget[ParkingIndicatorState.B1_PowerRoomPoint], ObjectiveAction.Push,
            1f);
        cutSceneCollider.gameObject.SetActive(false);
    }

    private void AddInteract()
    {
        InitializeInteractData(hmsInteract);
    }

    public void FirstEarTypeCutScene()
    {
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.CutScenePlaying, true);
        Debug.Log("귀쟁이 등장 뿌");
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.CutScenePlaying, false);
    }

    public void LightFlicker()
    {
        foreach (Flicker flicker in lightsObj)
            flicker.StartFlicker();

        foreach (CarObject car in carObjects)
            car.TriggerEffect();

        PManagers.Sound.Play(ESound.SFX, monsterSound);
    }

    private void OnDistributionBoxCheck(DistributionBox_Obj.DistributionBoxType type)
    {
        if (type == DistributionBox_Obj.DistributionBoxType.B1)
            _ch23Manager.OnB1DistributionOn();
        else if (type == DistributionBox_Obj.DistributionBoxType.B2)
            _ch23Manager.OnB2DistributionOn();
    }

    private void OpenRoom()
    {
        foreach (GeneratorDoor_Obj door in b2Doors)
        {
            UIManager.Instance.objective.SetMarkerByKey("FindPowerRoom");
            UIManager.Instance.objective.Follow(questTarget[ParkingIndicatorState.B2_PowerRoomPoint],
                ObjectiveAction.Follow, 1f);
            door.canOpen = true;
            door.storyLock = false;
        }
    }

    public void B1GeneratorInteract() //지하 1층 발전기 상호작용
    {
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.OnlyLook, true);
        UIManager.Instance.DialogueOpen(Dialogue.Interaction, false, 00);
        if (!ScenarioManager.Instance.GetAchieve(ScenarioAchieve.SecurityOfficeKey)) //아직 키 없으면
        {
            UIManager.Instance.dialogueEnd += BouncerInteract;
        }
    }

    private void BouncerInteract() //경비원
    {
        //인디케이터 출력
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.OnlyLook, false);
    }

    private void GetSecurityOfficeKey()
    {
        ScenarioManager.Instance.SetAchieve(ScenarioAchieve.SecurityOfficeKey, true);
    }

    private void EnterB2GeneratorRoom()
    {
        if (enterRoom)
            return;

        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.OnlyLook, true);

        StartCoroutine(EnterRoomSequence());
    }

    private IEnumerator EnterRoomSequence()
    {
        yield return StartCoroutine(LookHMS()); // 회전 끝날 때까지 기다림
        yield return StartCoroutine(HMSDead()); // 그 다음 Dialogue 실행
    }

    private IEnumerator LookHMS()
    {
        yield return new WaitForSeconds(1.0f);

        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.ObjectInteract, true);

        Transform cam = GameManager.Instance.Player.FPView;
        Transform hms = GameSceneManager.Instance.npcUnits[NpcName.HMS].transform;

        yield return new WaitForSeconds(1.0f);

        float startYaw = cam.eulerAngles.y;

        Vector3 dir = hms.position - cam.position;
        float targetYaw = Quaternion.LookRotation(dir).eulerAngles.y;

        float duration = 1f;
        float t = 0f;

        while (t < duration)
        {
            float normalized = t / duration;
            float curved = lookCurve.Evaluate(normalized);

            float yaw = Mathf.LerpAngle(startYaw, targetYaw, curved);

            Vector3 newEuler = new Vector3(cam.eulerAngles.x, yaw, 0);
            cam.rotation = Quaternion.Euler(newEuler);

            t += Time.deltaTime;
            yield return null;
        }

        cam.rotation = Quaternion.Euler(cam.eulerAngles.x, targetYaw, 0);

        GameManager.Instance.Player.PlayerAngle(targetYaw);

        yield return new WaitForSeconds(2.0f);
    }

    private IEnumerator HMSDead()
    {
        bool dialogueEnded = false;
        void OnDialogueEnd() => dialogueEnded = true;

        UIManager.Instance.dialogueEnd += OnDialogueEnd;
        UIManager.Instance.dialogueEnd += EndHMSEvent;
        UIManager.Instance.DialogueOpen(Dialogue.Interaction, false, 139);

        yield return new WaitUntil(() => dialogueEnded);
        UIManager.Instance.dialogueEnd -= OnDialogueEnd;
        UIManager.Instance.dialogueEnd -= EndHMSEvent;

        yield return GameManager.Instance.fadeManager.FadeStart(FadeState.FadeOut);

        yield return StartCoroutine(B2GeneratorDoorClose()); //GeneratorDoor_Obj 를 닫아놓음 

        GameSceneManager.Instance.npcUnits[NpcName.HMS].NpcStop();
        GameSceneManager.Instance.npcUnits[NpcName.HMS].gameObject.SetActive(false);

        GameManager.Instance.MovePlayerTransform(generatorPlayerPos); //플레이어 위치를 발전실 안으로 이동 

        yield return GameManager.Instance.fadeManager.FadeStart(FadeState.FadeIn);
    }

    private IEnumerator B2GeneratorDoorClose()
    {
        float maxDuration = 0f;

        foreach (GeneratorDoor_Obj door in b2Doors)
        {
            door.CloseDoor();
            door.IsInteractable = false;

            float duration = door.GetLockDuration();
            if (duration > maxDuration)
                maxDuration = duration;
        }

        yield return new WaitForSeconds(maxDuration);
    }

    private void EndHMSEvent()
    {
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.ObjectInteract, false);
    }

    private void OnPuzzleClear()
    {
        foreach (var door in b2Doors)
        {
            door.IsInteractable = true;
            door.canOpen = true;
            door.skipStoryLock = true;
        }

        Debug.Log("B2 발전실 퍼즐 완료 → 문 열림");

        foreach (var speaker in speakers)
        {
            //speaker.clip = broadcasting; 추후에 방송 파일 추가하고 해제해야함
            speaker.loop = true;
            speaker.Play();
        }

        enterRoom = true;
        emerganceyDoor.SetHighlight(true);
    }

    public void MonsterSpawn()
    {
        earTypeMonsterShow.gameObject.SetActive(true);
    }

    private void PuzzleClear()
    {
        foreach (var e in endElevatorObj)
        {
            e.IsInteractable = true;
            e.sceneEnum = SceneEnum.Demo;
            e.OffInteract += SceneClear;
        }

        foreach (var d in endDoorObj)
        {
            d.IsInteractable = true;
            d.storyLock = false;
        }

        // 퀘스트 갱신
        foreach (var s in speakers)
        {
            s.Play();
        }

        // Invoke(nameof(Scene), 3f);
    }

    private void Scene()
    {
        GameManager.Instance.fadeManager.MoveScene(SceneEnum.Demo);
    }
}