using System.Collections.Generic;
using DG.Tweening;
using Marker;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Video;

public class Tutorial_SceneInitializer : ChapterBase
{
    [Header("Marker Quest Target")]
    [SerializeField] public QuestTarget keyTarget;
    [SerializeField] public QuestTarget buttonTarget;
    
    [BoxGroup("Tutorial", ShowLabel = true, CenterLabel = true)]
    [BoxGroup("Tutorial")]
    [TabGroup("Tutorial/Tabs", "Trigger")] public bool barrierKey;
    [TabGroup("Tutorial/Tabs", "Trigger")] public ButtonBoxBtn_Obj btnObj;

    [TabGroup("Tutorial/Tabs", "GameObj")] public Shutter_Obj shutterObj;
    [TabGroup("Tutorial/Tabs", "GameObj")] public DOTweenAnimation metalDoorLock;

    [TabGroup("Tutorial/Tabs)", "Audio")] public AudioSource metalImpact;
    [TabGroup("Tutorial/Tabs)", "Audio")] public AudioSource monsterImpact;
    [TabGroup("Tutorial/Tabs)", "Audio")] public AudioSource arlmAudio;

    [TabGroup("Tutorial/Interact", "NpcInteract")] public List<NpcInteractBase> kjmInteract;
    [TabGroup("Tutorial/Interact", "NpcInteract")] public Transform kjmStartpos;

    [TabGroup("Tutorial/Interact", "Arlm")] public Light[] lights;
    [TabGroup("Tutorial/Interact", "Arlm")] public Light arlmLight;

    public Transform playerStartPos;
    [SerializeField] private GameObject stairIntroCutScene;
    [SerializeField] private VideoPlayer stairIntroCutSceneVideo;
    public override void SceneClear() { ScenarioManager.Instance.SetAchieve(ScenarioAchieve.Tutorial, true); }
    public override void Initialize()
    {
        GameSceneManager.Instance.psManager.SetNpcActive(NpcName.KJM, true);
        GameSceneManager.Instance.psManager.MoveNpc(NpcName.KJM, kjmStartpos);
        GameSceneManager.Instance.npcUnits[NpcName.KJM].transform.LookAt(playerStartPos);
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.CutScenePlaying, true);
        NotebookManager.Instance.NotebookMap.UpdateMap("map_sample");
        
        GameManager.Instance.MovePlayerTransform(playerStartPos);

        stairIntroCutScene.SetActive(true);
        stairIntroCutSceneVideo.loopPointReached += InitDialogue;
    }

    private void InitDialogue(VideoPlayer vp)
    {
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.CutScenePlaying, false);
        UIManager.Instance.dialogueEnd += DialogueEnd;
        UIManager.Instance.DialogueOpen(Dialogue.Main, true, 0);

        btnObj.OffInteract += ArlmEnable;
        stairIntroCutScene.SetActive(false);
    }
    
    private void DialogueEnd()
    {
        UIManager.Instance.questUI.AddQuest("ShutterDef");
        NotebookManager.Instance.NotebookMission.UpdateMission("ShutterDef");
        UIManager.Instance.objective.SetMarkerByKey("ShutterDef");
        UIManager.Instance.objective.Follow(keyTarget, ObjectiveAction.PickUp);
        var seq = DOTween.Sequence();

        seq.AppendInterval(3f);
        seq.AppendCallback(ShutterImpact);
        seq.AppendInterval(3f);
        seq.AppendCallback(() => monsterImpact.Play());
        seq.AppendInterval(2f);
        //seq.AppendCallback(()=> StairSceneManager.Instance.npcUnits[NpcName.KJM].transform.LookAt(shutterObj.transform));
        seq.AppendInterval(1f);
        //seq.AppendCallback(() => UIManager.Instance.dialogueEnd += InteractStart);
        seq.AppendCallback(() => UIManager.Instance.DialogueOpen(Dialogue.Main, false, 10));
        seq.Play();
    }

    private void ShutterImpact()
    {
        metalDoorLock.delay = 1.5f;
        metalDoorLock.loops = -1;

        metalDoorLock.DOKill();
        metalDoorLock.CreateTween(true);

        metalImpact.Play();
    }

    private void InteractStart()
    {
        StairSceneManager.Instance.npcUnits[NpcName.KJM].IsInteractable = true;
        InitializeInteractData(kjmInteract);
    }

    private void ArlmEnable()
    {
        foreach (var l in lights)
        {
            if (l)
            {
                l.enabled = false;
            }
        }

        arlmLight.color = new Color(84 / 255f, 0 / 255f, 0 / 255f);
        arlmAudio.Play();
    }
}
