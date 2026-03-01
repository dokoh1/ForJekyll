using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using Zenject;

public class Chapter4_2_SceneInitializer : ChapterBase
{
    public override void SceneClear() { ScenarioManager.Instance.SetAchieve(ScenarioAchieve.Chapter4_2, true); }
    public override void Initialize()
    {
    }


    [SerializeField] private ColliderHandler playerStart;
    [SerializeField] private ColliderHandler playerEnd;
    [SerializeField] private ColliderHandler puzzleCollider;
    
    [Header("Audio")]
    [SerializeField] private AudioSource DoorAudio;
    [SerializeField] private AudioSource MonsterAudio;
    [SerializeField] private AudioClip DoorCrashClip;

    [Header("CutScene")]
    [SerializeField] private PlayableDirector fanCutScene;
    [SerializeField] private FanLever fanLever;

    private bool isDeadTriggered = false;
    private bool deadTime = false;
    private bool playerIn = false;
    private bool doorCrashed = false;

    private void Start()
    {
        DataManager.Instance.sceneDataManager.AutoDataSave(10);
        SoundManager.Instance.PlayBGM(SoundManager.Instance.BGM_Source, BGM_Sound.Day1Follow, true, 0.5f, 0, false);

        StartCoroutine(DoorCrash());

        //UIManager.Instance.dialogueEnd += SetFlash;
        //UIManager.Instance.DialogueOpen(Dialogue.Interaction, false, 45);

        playerStart.OnTriggerEntered += MonsterMove;
        playerEnd.OnTriggerEntered += MoveScene;
        //puzzleCollider.OnTriggerEntered += MonsterDisable;

        fanLever.OnLeverAction += PlayFanCutScene;
        fanCutScene.stopped += FanCutSceneEnd;

        var player = GameManager.Instance.Player;
        player.SetCrouchSpeedModifier(0.5f);

        GameManager.Instance.Player.PlayerAngle(-90.0f);
    }

    private void SetFlash()
    {
        //GameManager.Instance.Player.PlayerUIBlinker.StartBlink();
        Debug.Log("후레시");
    }

    private void OnTriggerStay(Collider other)
    {
        if (!deadTime || isDeadTriggered) return;

        if (other.CompareTag("Player"))
        {
            isDeadTriggered = true;
            GameManager.Instance.Player
                .JumpScareManager
                .PlayerDead(JumpScareType.EarTypeMonster);
        }
    }

    private IEnumerator DoorCrash()
    {
        float time = 0;

        while (time < 8 && !playerIn)
        {
            time += Time.deltaTime;
            yield return null;
        }
        
        TriggerDoorCrash();
    }

    private void TriggerDoorCrash()
    {
        if (doorCrashed) return;
        doorCrashed = true;

        BoxCollider box = GetComponent<BoxCollider>();
        box.enabled = false;
        box.enabled = true;

        DoorAudio.Stop();
        MonsterAudio.Stop();

        DoorAudio.clip = DoorCrashClip;
        DoorAudio.volume = 1f;
        DoorAudio.loop = false;
        DoorAudio.Play();

        deadTime = true;
    }

    public void PlayFanCutScene()
    {
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.CutScenePlaying, true);
        
        fanCutScene.Play();
    }

    public void FanCutSceneEnd(PlayableDirector director)
    {        

        GameManager.Instance.fadeManager.FadeStart(FadeState.FadeIn);
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.CutScenePlaying, false);
    }

    private void MonsterMove() { playerIn = true; }
    private void MoveScene()
    {
        var player = GameManager.Instance.Player;
        player.CurNoiseAmount = 0;
        player.SumNoiseAmount = 0;
        player.RestoreCrouchSpeed();

        GameManager.Instance.fadeManager.MoveScene(SceneEnum.Chapter4_2);
    }

    private void OnEnable()
    {
        WayPoint.OnDoorCrashRequest += TriggerDoorCrash;
    }

    private void OnDisable()
    {
        WayPoint.OnDoorCrashRequest -= TriggerDoorCrash;
    }
}