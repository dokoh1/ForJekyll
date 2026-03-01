using System;
using System.Linq;
using DG.Tweening;
public class Chapter1_1_Manager
{
    private readonly Chapter1_1_SceneInitializer _ch11;
    public Chapter1_1_Manager(Chapter1_1_SceneInitializer ch11) { _ch11 = ch11; }
    public void Interact3D()
    {
        SoundManager.Instance.PlayBGM(SoundManager.Instance.BGM_Source, BGM_Sound.Hotel_Talk_Loop, true, 1f, 13f, true); // sound
        _ch11.questIcon.SetActive(false);
        _ch11.pjhQuestIcon.SetActive(true);

        foreach (var n in _ch11.npcFirstTransforms) { GameSceneManager.Instance.psManager.MoveNpc(n.Key, n.Value); }
        var npcDict = GameSceneManager.Instance.npcUnits;
        foreach (var n in npcDict)
        {
            if (!npcDict.ContainsKey(n.Key) || n.Key == NpcName.YHJ) continue;

            var target = n.Key switch
            {
                NpcName.HMS => npcDict[NpcName.KJM].transform,
                NpcName.PJH => npcDict[NpcName.JYS].transform,
                NpcName.KJM => npcDict[NpcName.HMS].transform,
                NpcName.PSY => npcDict[NpcName.HMS].transform,
                NpcName.JYS => npcDict[NpcName.PJH].transform,
            };
            n.Value.transform.LookAt(target);
        }

        _ch11.InitializeInteractData(_ch11.allInteract);
        //HotelSceneManager.Instance.npcUnits[NpcName.PJH].onInteractEnd += LightSystemBoxCol;
        UIManager.Instance.objective.ClearAll();  
        
        foreach (var n in GameSceneManager.Instance.npcUnits)
        {
            UIManager.Instance.objective.FollowAt((int)n.Key, _ch11.targetsNpc[n.Key], ObjectiveAction.Conversation);
            UIManager.Instance.objective.SetMarkerByKeyAt((int)n.Key, "TalkAll");
            n.Value.onInteractEnd += InteractAll;
        }

        ScenarioManager.Instance.SetAchieve(ScenarioAchieve.Chapter1_1_TalkAllEvent, true); // bool
        DataManager.Instance.sceneDataManager.AutoDataSave(10);

        ScenarioManager.Instance.npcInteracts[NpcName.PJH] = true;
        ScenarioManager.Instance.npcInteracts[NpcName.YHJ] = true;

        UIManager.Instance.questUI.AddQuest("TalkAll");
        NotebookManager.Instance.NotebookMission.UpdateMission("TalkAll");
         
    }

    private void LightSystemBoxCol(NpcName npcName)
    {
        foreach (var n in GameSceneManager.Instance.npcUnits) { n.Value.IsInteractable = false; }

        _ch11.pjhQuestIcon.SetActive(false);

        foreach (var n in _ch11.npcSecondTransforms)
        {
            GameSceneManager.Instance.psManager.MoveNpc(n.Key, n.Value);
        }
        GameManager.Instance.MovePlayerTransform(_ch11.playerSecondTransform);
        foreach (var n in GameSceneManager.Instance.npcUnits.Where(n => n.Key != NpcName.YHJ))
        {
            n.Value.transform.LookAt(_ch11.playerSecondTransform);
        }

        SoundManager.Instance.StopAudioSource(SoundManager.Instance.BGM_Source, true); // sound

        _ch11.lightSystemCollider.gameObject.SetActive(true);
        _ch11.lightSystemCollider.OnTriggerEntered += LightSystemOffCutScene;
        UIManager.Instance.questUI.AddQuest("MoveOppositeElevator");
        NotebookManager.Instance.NotebookMission.UpdateMission("MoveOppositeElevator");
        UIManager.Instance.objective.SetMarkerByKey("MoveOppositeElevator");
        UIManager.Instance.objective.Follow(_ch11.moveOppositeElevator, ObjectiveAction.Follow, 1f);
    }

    private void LightSystemOffCutScene()
    {
        UIManager.Instance.objective.ClearAll();
        GameManager.Instance.fadeManager.FadeStart(FadeState.FadeOut);
        GameManager.Instance.fadeManager.fadeComplete += StartCutScene;
    }

    private void InteractAll(NpcName npcName)
    {
        ScenarioManager.Instance.npcInteracts[npcName] = true;
        UIManager.Instance.objective.ClearAt((int)npcName);
        if (!ScenarioManager.Instance.CheckNpcInteract()) 
            return;
        UIManager.Instance.objective.ClearAll();
        _ch11.InitializeInteractData( _ch11.pjhInteract);
        GameSceneManager.Instance.npcUnits[NpcName.PJH].onInteractEnd += LightSystemBoxCol;
    }
    
    private void StartCutScene()
    {
        _ch11.lightOffCutScene.gameObject.SetActive(true);
        _ch11.lightOffCutScene.Play(true);
        _ch11.lightOffCutScene.stopped += _ch11.LightOff2;
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.CutScenePlaying, true); // bool
        GameManager.Instance.fadeManager.FadeStart(FadeState.FadeIn, 0.5f);

        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(1.5f);
        sequence.AppendCallback(() => { UIManager.Instance.DialogueOpen(Dialogue.Main, false, 108); });
        sequence.Play();
    }
}
