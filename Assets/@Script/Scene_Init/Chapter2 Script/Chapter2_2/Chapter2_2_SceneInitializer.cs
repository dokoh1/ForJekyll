using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class Chapter2_2_SceneInitializer : ChapterBase
{
    [BoxGroup("Chapter2_2", ShowLabel = true, CenterLabel = true)]
    [BoxGroup("Chapter2_2")]
    [TabGroup("Chapter2_2/Tabs", "Script")][SerializeField] private Elevator_Obj elevator;
    [TabGroup("Chapter2_2/Tabs", "Script")][SerializeField] private Caf_Door cafDoor;
    [TabGroup("Chapter2_2/Tabs", "Script")][SerializeField] private Door_Obj[] doors;
    [TabGroup("Chapter2_2/Tabs", "Script")][SerializeField] private ColliderHandler elevatorFrontCol;
    
    [BoxGroup("Chapter2_2")]
    [TabGroup("Chapter2_2/Tabs", "Script")][SerializeField] private GameObject qusetIcon;
    [TabGroup("Chapter2_2/Tabs", "Script")][SerializeField] private GameObject subQuestIcon;
    [TabGroup("Chapter2_2/Tabs", "Script")][SerializeField] private GameObject candle;
    
    [BoxGroup("Chapter2_2")]
    [TabGroup("Chapter2_2/Tabs", "Pos")] public Transform playerPos;
    [TabGroup("Chapter2_2/Tabs", "Pos")] public SerializableDictionary<NpcName, Transform> zeroNpcTransforms;
    [TabGroup("Chapter2_2/Tabs", "Pos")] public SerializableDictionary<NpcName, Transform> firstNpcTransforms;
    
    [FormerlySerializedAs("npcInteracts")]
    [BoxGroup("Chapter2_2")]
    [TabGroup("Chapter2_2/Tabs", "Npc")] public List<NpcInteractBase> firstNpcInteracts;
    [TabGroup("Chapter2_2/Tabs", "Npc")] public List<NpcInteractBase> secondNpcInteracts;
    [TabGroup("Chapter2_2/Tabs", "Npc")] public List<NpcInteractBase> thirdNpcInteracts;
    public override void SceneClear() { ScenarioManager.Instance.SetAchieve(ScenarioAchieve.Chapter2_2, true);}
    public override void Initialize()
    {
        GameManager.Instance.Player.PlayerAngle(-90);
        
        DataManager.Instance.sceneDataManager.AutoDataSave(10);
        GameManager.Instance.MovePlayerTransform(playerPos);
        UIManager.Instance.dialogueEnd += ElevatorOpen;
        UIManager.Instance.DialogueOpen(Dialogue.Main, true, ScenarioManager.Instance.GetAchieve(ScenarioAchieve.JYS_Dead) ? 113 : 125); // story
        foreach (var n in zeroNpcTransforms) { GameSceneManager.Instance.psManager.MoveNpc(n.Key, n.Value); }
        foreach (var n in GameSceneManager.Instance.npcUnits.Where
                     (n => n.Key != NpcName.JYS)) { n.Value.gameObject.SetActive(true); }
        
        var npcDict = GameSceneManager.Instance.npcUnits;
        foreach (var n in GameSceneManager.Instance.npcUnits)
        {
            if (!npcDict.ContainsKey(n.Key) || n.Key == NpcName.JYS || n.Key == NpcName.KJM) continue;

            var target = n.Key switch
            {
                NpcName.HMS => npcDict[NpcName.PSY].transform,
                NpcName.PJH => npcDict[NpcName.PSY].transform,
                NpcName.PSY => npcDict[NpcName.PJH].transform,
                NpcName.YHJ => npcDict[NpcName.PSY].transform,
            }; n.Value.transform.LookAt(target);
        }

        Destroy(cafDoor);
        foreach (var d in doors) { d.canOpen = true; }
        elevatorFrontCol.OnTriggerEntered += ElevatorNextStory;
    }

    private void ElevatorOpen() { StartCoroutine(elevator.ElevatorOpen()); }

    private void ElevatorNextStory()
    {
        UIManager.Instance.dialogueEnd += InteractOn;
        UIManager.Instance.DialogueOpen(Dialogue.Main, true, 132); // story
    }

    private void InteractOn()
    {
        candle.SetActive(false);
        subQuestIcon.SetActive(true);
        //GameManager.Instance.Player.PlayerUIBlinker.StartBlink();
        Debug.Log("후레시");
        
        InitializeInteractData(firstNpcInteracts);
        GameSceneManager.Instance.npcUnits[NpcName.KJM].FollowStart();
        GameSceneManager.Instance.npcUnits[NpcName.PSY].onInteractEnd += InteractHms;
        foreach (var n in firstNpcTransforms) { GameSceneManager.Instance.psManager.MoveNpc(n.Key, n.Value); }
        var npcDict = GameSceneManager.Instance.npcUnits;
        foreach (var n in GameSceneManager.Instance.npcUnits)
        {
            if (!npcDict.ContainsKey(n.Key) || n.Key == NpcName.JYS) continue;

            var target = n.Key switch
            {
                NpcName.HMS => playerPos,
                NpcName.PJH => playerPos,
                NpcName.KJM => playerPos,
                NpcName.PSY => playerPos,
                NpcName.YHJ => playerPos,
            }; n.Value.transform.LookAt(target);
        }
    }

    private void InteractHms(NpcName npcName)
    {
        InitializeInteractData(secondNpcInteracts);
        GameSceneManager.Instance.npcUnits[NpcName.PSY].onInteractEnd -= InteractHms;
        GameSceneManager.Instance.npcUnits[NpcName.HMS].onInteractEnd += MoveChapter;
        GameSceneManager.Instance.npcUnits[NpcName.HMS].IsInteractable = true;
 
        foreach (var n in GameSceneManager.Instance.npcUnits.Where(n 
                     => n.Key is not (NpcName.HMS or NpcName.PJH or NpcName.JYS))) { n.Value.onInteractEnd += DataManager.Instance.FavorEvent; }
        
        subQuestIcon.SetActive(false);
        qusetIcon.SetActive(true);
    }

    private void MoveChapter(NpcName npcName)
    {
        InitializeInteractData(thirdNpcInteracts);
    }
}
