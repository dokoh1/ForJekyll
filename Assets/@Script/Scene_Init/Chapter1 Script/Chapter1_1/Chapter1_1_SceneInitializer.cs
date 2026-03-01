using System.Collections.Generic;
using System.Linq;
using Marker;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Playables;
using UnityEngine.Serialization;

public class Chapter1_1_SceneInitializer : ChapterBase
{
    [BoxGroup("Chapter1_1", ShowLabel = true, CenterLabel = true)]
    [BoxGroup("Chapter1_1")]
    [TabGroup("Chapter1_1/Tabs", "Manager")] public ColliderHandler firstMeetCollider;
    [TabGroup("Chapter1_1/Tabs", "Manager")] public ColliderHandler lightSystemCollider;
    [TabGroup("Chapter1_1/Tabs", "Manager")] public Controll_Blur blur;

    [BoxGroup("Chapter1_1")]
    [TabGroup("Chapter1_1/Tabs", "Object")] public PlayableDirector startCutScene;
    [TabGroup("Chapter1_1/Tabs", "Object")] public PlayableDirector lightOffCutScene;

    [BoxGroup("Chapter1_1")]
    [TabGroup("Chapter1_1/Tabs", "Icon")] public GameObject questIcon;
    [TabGroup("Chapter1_1/Tabs", "Icon")] public GameObject pjhQuestIcon;

    [BoxGroup("Chapter1_1")]
    [TabGroup("Chapter1_1/Pos", "Position")] public SerializableDictionary<NpcName, Transform> npcFirstTransforms;
    [TabGroup("Chapter1_1/Pos", "Position")] public SerializableDictionary<NpcName, Transform> npcSecondTransforms;

    [BoxGroup("Chapter1_1")]
    [TabGroup("Chapter1_1/Interact", "NpcInteract")] public List<NpcInteractBase> kjmInteract;
    [TabGroup("Chapter1_1/Interact", "NpcInteract")] public List<NpcInteractBase> pjhInteract;
    [TabGroup("Chapter1_1/Interact", "NpcInteract")] public List<NpcInteractBase> allInteract;

    [Title("PlayerSpawnPoint")]
    [BoxGroup("Chapter1_1")]
    [TabGroup("Chapter1_1/Pos", "Position")] public Transform playerMinusTransform;
    [TabGroup("Chapter1_1/Pos", "Position")] public Transform playerFirstTransform;
    [TabGroup("Chapter1_1/Pos", "Position")] public Transform playerSecondTransform;
    
    [Title("Sound")]
    public OutdoorSound outdoorSound;
    
    [Title("QuestTarget")]
    [BoxGroup("Chapter1_1")]
    [TabGroup("Chapter1_1/Target", "Target")] public SerializableDictionary<NpcName, QuestTarget> targetsNpc;
    [TabGroup("Chapter1_1/Target", "Target")] public QuestTarget moveHall;
    [TabGroup("Chapter1_1/Target", "Target")] public QuestTarget moveOppositeElevator;
    private Chapter1_1_Manager _ch11Manager;
    public override void Initialize()
    {
        _ch11Manager = new Chapter1_1_Manager(this);
        firstMeetCollider ??= GetComponent<ColliderHandler>();

        DataManager.Instance.Chapter = 1;
        DataManager.Instance.sceneDataManager.AutoDataSave(10);

        foreach (var n in GameSceneManager.Instance.npcUnits.Where(n => n.Key != NpcName.YHJ))
        {
            n.Value.gameObject.SetActive(true);
        }

        if (!ScenarioManager.Instance.GetAchieve(ScenarioAchieve.Chapter1_1_TalkAllEvent))
        {
            GameManager.Instance.Player.PlayerAngle(-90);
            GameManager.Instance.MovePlayerTransform(playerMinusTransform);
            GameSceneManager.Instance.npcUnits[NpcName.KJM].transform.LookAt(playerMinusTransform);
            GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.CutScenePlaying, true); // bool

            startCutScene.stopped += InitCutSceneEnd;

            blur.enabled = true;
            StartCoroutine(blur.CallValue(3));
            firstMeetCollider.OnTriggerEntered += FirstTriggerEnter;
        }
        else
        {
            GameManager.Instance.MovePlayerTransform(playerFirstTransform);
            startCutScene.gameObject.SetActive(false);
            blur.enabled = false;

            var box = GetComponent<BoxCollider>();
            Destroy(box);

            _ch11Manager.Interact3D();

            GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.PlayerMove, true);
        }
    }

    private void InitCutSceneEnd(PlayableDirector obj)
    {
        startCutScene.gameObject.SetActive(false);
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.CutScenePlaying, false); // bool
        InitializeInteractData(kjmInteract);
        questIcon.SetActive(true);
        UIManager.Instance.questUI.AddQuest("MoveHall");
        NotebookManager.Instance.NotebookMission.UpdateMission("MoveHall");
        UIManager.Instance.objective.SetMarkerByKey("MoveHall");
        UIManager.Instance.objective.Follow(moveHall, ObjectiveAction.Follow, 1f);
    }

    private void FirstTriggerEnter()
    {
        if (ScenarioManager.Instance.GetAchieve(ScenarioAchieve.Chapter1_1_TalkAllEvent)) return;

        //if (outdoorSound.isPlay) outdoorSound.audioSource.Stop();
        var box = GetComponent<BoxCollider>();
        Destroy(box);
        SoundManager.Instance.PlayBGM(SoundManager.Instance.BGM_Source, BGM_Sound.FirstMeet, true, 0.25f, 0, false); // Sound
        UIManager.Instance.DialogueOpen(Dialogue.Main, true, 11); // Story
        UIManager.Instance.dialogueEnd += _ch11Manager.Interact3D;
        //UIManager.Instance.dialogueEnd += outdoorSound.PlaySound;
    }
    public void LightOff2(PlayableDirector director)
    {
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.CutScenePlaying, false); // bool
        lightOffCutScene.gameObject.SetActive(false);
        UIManager.Instance.DialogueOpen(Dialogue.Main, true, 111); // story
        UIManager.Instance.dialogueEnd += SceneClear;
    }
    public override void SceneClear()
    {
        ScenarioManager.Instance.SetAchieve(ScenarioAchieve.Chapter1_1, true);
        GameManager.Instance.fadeManager.MoveScene(SceneEnum.Chapter1_2);
    }
}
