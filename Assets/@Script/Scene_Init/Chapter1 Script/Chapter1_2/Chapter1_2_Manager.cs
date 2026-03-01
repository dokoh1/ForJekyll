using DG.Tweening;
using UnityEngine.Playables;

public class Chapter1_2_Manager
{
    private readonly Chapter1_2_SceneInitializer _ch12;
    public Chapter1_2_Manager(Chapter1_2_SceneInitializer ch12) { _ch12 = ch12; }
    public void Monster_NpcSpawn()
    {
        UIManager.Instance.objective.ClearAll();
        GameManager.Instance.MovePlayerTransform(_ch12.playerTeleportPos);
        DataManager.Instance.sceneDataManager.AutoDataSave(10);
        GameManager.Instance.playerSettingManager.PlayerSkillValue(false);

        _ch12.InitializeInteractData(_ch12.yhjInteract);
        GameSceneManager.Instance.npcUnits[NpcName.YHJ].gameObject.SetActive(true);
        GameSceneManager.Instance.npcUnits[NpcName.YHJ].FollowStart();

        foreach (var mon in _ch12.monsters) { mon.SetActive(true); }
        foreach (var ele in _ch12.elevator)
        {
            ele.interactTalk = true;
            ele.interactionIndex = 91;
            ele.IsInteractable = true;
            ele.sceneEnum = SceneEnum.Chapter1_3;
        }

        MonsterSpawnAfter();
    }
    
    public void MonsterSpawnAfter()
    {
        var seq = DOTween.Sequence();

        _ch12.elevatorColliderHandler.OnTriggerEntered += OnTriggerEnter;

        seq.AppendInterval(2f);
        seq.AppendCallback(() => UIManager.Instance.dialogueEnd += MoveOn);
        seq.AppendCallback(() => UIManager.Instance.DialogueOpen(Dialogue.Main, false, 184)); // story
        seq.AppendCallback(() => GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.OnlyLook, true)); // bool
        seq.AppendCallback(() => _ch12.YHJ_Door.IsInteractable = true);
        seq.Play();
    }

    private void MoveOn()
    {
        GameManager.Instance.playerSettingManager.PlayerSkillValue(true);
        _ch12.flashLight.GetFlashLight -= _ch12.MoveOppsiteAction;
        UIManager.Instance.objective.ClearAll();
        UIManager.Instance.questUI.AddQuest("MoveWithYHJ");
        _ch12.centerSecondCollider.OnTriggerEntered += CenterSecondActive;
        NotebookManager.Instance.NotebookMission.UpdateMission("MoveWithYHJ");
        UIManager.Instance.objective.SetMarkerByKey("MoveWithYHJ");
        UIManager.Instance.objective.Follow(_ch12.centerSecondTarget, ObjectiveAction.Follow, 1f);
        
        
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.OnlyLook, false);
    }

    private void CenterSecondActive()
    {
        UIManager.Instance.objective.ClearAll();
        UIManager.Instance.objective.SetMarkerByKey("MoveWithYHJ");
        _ch12.elevatorColliderHandler.OnTriggerEntered += ElevatorWayActive;
        UIManager.Instance.objective.Follow(_ch12.elevatorWayTarget, ObjectiveAction.Follow, 1f);
        
    }

    private void ElevatorWayActive()
    {
        UIManager.Instance.objective.ClearAll();
        UIManager.Instance.objective.SetMarkerByKeyAt(0, "MoveWithYHJ");
        UIManager.Instance.objective.SetMarkerByKeyAt(1, "MoveWithYHJ");
        UIManager.Instance.objective.FollowAt(0, _ch12.elevatorButtonLeft, ObjectiveAction.Push, 1f);
        UIManager.Instance.objective.FollowAt(1, _ch12.elevatorButtonRight, ObjectiveAction.Push, 1f);
    }
    private void OnTriggerEnter()
    {
        foreach (var ele in _ch12.elevator)
        {
            ele.interactTalk = false;
            ele.OffInteract += _ch12.SceneClear;
            UIManager.Instance.objective.ClearAll();
            ele.MaterialChange();
        }
    }
}
