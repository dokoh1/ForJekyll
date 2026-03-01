using System.Collections.Generic;
using Marker;
using Sirenix.OdinInspector;
using UnityEngine;

public class Chapter1_3_SceneInitializer : ChapterBase
{    
    [BoxGroup("Chapter1_3", ShowLabel = true, CenterLabel = true)]
    [BoxGroup("Chapter1_3")]
    [TabGroup("Chapter1_3/Tabs", "And")][SerializeField] private Caf_Door caf_Door;
    [TabGroup("Chapter1_3/Tabs", "And")][SerializeField] private Door_Obj doorObj;
    [TabGroup("Chapter1_3/Tabs", "And")][SerializeField] private GameObject questICon;
    
    [BoxGroup("Chapter1_3")]
    [TabGroup("Chapter1_3/Tabs", "PlayerPos")] public Transform playerPos;
    
    [BoxGroup("Chapter1_3")]
    [TabGroup("Chapter1_3/Tabs", "Npc")] public SerializableDictionary<NpcName, Transform> npcTransforms;
    [TabGroup("Chapter1_3/Tabs", "Npc")] public List<NpcInteractBase> npcInteractBase;
    [SerializeField] private QuestTarget npcTarget;
    [SerializeField] private QuestTarget doorTarget;
    
    public override void SceneClear(){ ScenarioManager.Instance.SetAchieve(ScenarioAchieve.Chapter1_3, true); }
    public override void Initialize()
    {
        DataManager.Instance.sceneDataManager.AutoDataSave(10);

        foreach (var n in npcTransforms)
        {
            GameSceneManager.Instance.psManager.MoveNpc(n.Key, n.Value);
        }
        InitializeInteractData(npcInteractBase);
        var npcDict = GameSceneManager.Instance.npcUnits;
        foreach (var n in npcDict)
        {
            if (!npcDict.ContainsKey(n.Key)) continue;

            var target = n.Key switch
            {
                NpcName.HMS => npcDict[NpcName.YHJ].transform,
                NpcName.PJH => npcDict[NpcName.YHJ].transform,
                NpcName.KJM => playerPos.transform,
                NpcName.PSY => npcDict[NpcName.YHJ].transform,
                NpcName.JYS => npcDict[NpcName.YHJ].transform,
                NpcName.YHJ => npcDict[NpcName.HMS].transform,
            }; n.Value.transform.LookAt(target);

            if (n.Key is NpcName.KJM or NpcName.YHJ or NpcName.PSY)
            {
                n.Value.onInteractEnd += DataManager.Instance.FavorEvent;
            }
        }

        UIManager.Instance.dialogueEnd += BoolReset;
        UIManager.Instance.DialogueOpen(Dialogue.Main,true, 191);
        GameSceneManager.Instance.npcUnits[NpcName.HMS].onInteractStart += BGM_Start;
        GameSceneManager.Instance.npcUnits[NpcName.HMS].onInteractEnd += StoryEnd;

        SoundManager.Instance.PlayBGM(SoundManager.Instance.BGM_Source ,BGM_Sound.Hotel_Talk_Loop , true, 0.5f, 13f, true);

        if (!GameManager.Instance.Player.CurState.HasFlag(PlayerEnum.PlayerState.Flash))
        {
            GameManager.Instance.Player.SetFlash();
        }

        doorObj.enabled = false;
        doorObj.IsInteractable = false;
    }

    private void BoolReset()
    {
        UIManager.Instance.objective.SetMarkerByKey("Chapter1_3TalkHMS");
        UIManager.Instance.objective.Follow(npcTarget, ObjectiveAction.Conversation);
        UIManager.Instance.questUI.AddQuest("Chapter1_3TalkHMS");
        NotebookManager.Instance.NotebookMission.UpdateMission("Chapter1_3TalkHMS");
        GameManager.Instance.MovePlayerTransform(playerPos);
        GameManager.Instance.playerSettingManager.ResetBool();
    }
    private void StoryEnd(NpcName npcName)
    {
        questICon.SetActive(false);
        GameSceneManager.Instance.npcUnits[NpcName.HMS].IsInteractable = false;
        caf_Door.IsInteractable = true;
        caf_Door.AddMaterial();
        UIManager.Instance.objective.SetMarkerByKey("Chapter1_3Move4F");
        UIManager.Instance.objective.Follow(doorTarget, ObjectiveAction.Push);
        UIManager.Instance.questUI.AddQuest("Chapter1_3Move4F");
        NotebookManager.Instance.NotebookMission.UpdateMission("Chapter1_3Move4F");
        SoundManager.Instance.StopAudioSource(SoundManager.Instance.BGM_Source, true); // sound
    }

    private void BGM_Start(NpcName npcName)
    {
        SoundManager.Instance.PlayBGM(SoundManager.Instance.BGM_Source , BGM_Sound.Hotel_Talk_Loop , true, 0.5f, 13f, true);
    }
}
