using NPC;
using System.Collections;
using Marker;
using Sirenix.OdinInspector;
using UnityEngine;

public class Chapter2_1_SceneInitializer : ChapterBase
{
    [BoxGroup("Chapter2_1", ShowLabel = true, CenterLabel = true)]
    [TabGroup("Chapter2_1/Tabs", "Script")] public Elevator_Obj[] elevator;
    
    [BoxGroup("Chapter2_1")]
    [TabGroup("Chapter2_1/Tabs", "Monster")] public EyeTypeMonsterV2 eyeTypeMonster;
    [TabGroup("Chapter2_1/Tabs", "Monster")] public GameObject eyeTypeNormalMonster;
    [TabGroup("Chapter2_1/Tabs", "Monster")] public GameObject[] eyeTypeNormalMonsters;
    
    [BoxGroup("Chapter2_1")]
    [TabGroup("Chapter2_1/Tabs", "Object")] public GameObject restaurantDoorDel;
    [TabGroup("Chapter2_1/Tabs", "Object")] public GameObject restaurantDoorOn; 
    [TabGroup("Chapter2_1/Tabs", "Object")] public GameObject[] jysDead;
    [TabGroup("Chapter2_1/Tabs", "Object")] public GameObject knife;
    [TabGroup("Chapter2_1/Tabs", "Object")] public BloodCapsule bloodSample;
    
    [BoxGroup("Chapter2_1")]
    [TabGroup("Chapter2_1/Tabs", "Npc")] public NpcUnit jys;
    
    [BoxGroup("Chapter2_1")]
    [TabGroup("Chapter2_1/Tabs", "Position")] public Transform playerSpawnPos;
    [TabGroup("Chapter2_1/Tabs", "Position")] public Transform playerSavePos;
    [TabGroup("Chapter2_1/Tabs", "Position")] public Transform jysPos;

    [SerializeField] private QuestTarget capsuleTarget;
    [SerializeField] private QuestTarget jysTarget;
    [SerializeField] public QuestTarget elevatorTarget1;
    [SerializeField] public QuestTarget elevatorTarget2;
    private Chapter2_1_Manager _ch21Manager;

    public override void SceneClear()
    {
        UIManager.Instance.objective.ClearAll();
        ScenarioManager.Instance.SetAchieve(ScenarioAchieve.Chapter2_1, true);
    }
    public override void Initialize()
    {
        _ch21Manager = new Chapter2_1_Manager(this);
        
        jys.IsInteractable = true;
        jys.gameObject.SetActive(true);
        
        var hasBlood = ScenarioManager.Instance.GetAchieve(ScenarioAchieve.BloodCapsule);
        var hasKnife = ScenarioManager.Instance.GetAchieve(ScenarioAchieve.Knife);
        
        GameSceneManager.Instance.psManager.MoveNpc(jys.npcName, jysPos);
        if (!GameManager.Instance.Player.CurState.HasFlag(PlayerEnum.PlayerState.Flash))
        {
            GameManager.Instance.Player.SetFlash();
        }

        if (!hasBlood && !hasKnife)
        {
            DataManager.Instance.Chapter = 2;
            DataManager.Instance.sceneDataManager.AutoDataSave(10);

            UIManager.Instance.dialogueEnd += StartOther;
            UIManager.Instance.DialogueOpen(Dialogue.Main, true, 0); // story

            SoundManager.Instance.PlayBGM(SoundManager.Instance.BGM_Source, BGM_Sound.Hotel_Talk_Loop, true, 0.5f, 13f, true);
            
            jys.dialogue = Dialogue.Interaction;
            jys.DialogNum = 30;
            jys.is2D = false;
            
            GameSceneManager.Instance.psManager.MoveNpc(jys.npcName, jysPos);
            GameManager.Instance.MovePlayerTransform(playerSpawnPos);

            bloodSample.OffInteract += _ch21Manager.GetSample;
        }
        else
        {
            if (hasKnife)
            {
                knife.SetActive(false);
                DataManager.Instance.ChangeCondition("Knife", true);
            }

            if (hasBlood)
            {
                bloodSample.gameObject.SetActive(false);
                DataManager.Instance.ChangeCondition("Sample", true);
            }

            DataManager.Instance.NPC_Alive["제영섭"] = true;
            GameManager.Instance.MovePlayerTransform(playerSavePos);
            GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.PlayerMove, true);

            jys.dialogue = Dialogue.Main; // Story
            jys.DialogNum = 80;
            jys.is2D = true;
            jys.onInteractEnd += _ch21Manager.MonsterMove;

        }
    }

    private void StartOther()
    {
        SoundManager.Instance.StopAudioSource(SoundManager.Instance.BGM_Source, true); // sound
        StartCoroutine(Other());
        UIManager.Instance.questUI.AddQuest("FindCapsule");
        NotebookManager.Instance.NotebookMission.UpdateMission("FindCapsule");
        UIManager.Instance.objective.ClearAll();
        UIManager.Instance.objective.Follow(capsuleTarget, ObjectiveAction.PickUp);
        UIManager.Instance.objective.SetMarkerByKey("FindCapsule");
        bloodSample.GetBloodCapsule += JysConversation;
    }

    public void JysConversation()
    {
        UIManager.Instance.objective.Follow(jysTarget, ObjectiveAction.Conversation);
    }
    
    private IEnumerator Other()
    {
        yield return new WaitForSeconds(5f);
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.OnlyInteractLocked, true); // bool 
        UIManager.Instance.dialogueEnd += PlayerMove;
        UIManager.Instance.DialogueOpen(Dialogue.Main, false, 75); // story
    }
    private void PlayerMove()
    {
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.OnlyInteractLocked, false); // bool
    }
}
