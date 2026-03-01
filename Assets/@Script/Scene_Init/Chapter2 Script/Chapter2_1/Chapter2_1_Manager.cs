using DG.Tweening;

public class Chapter2_1_Manager
{
    private readonly Chapter2_1_SceneInitializer _ch21;
    public Chapter2_1_Manager(Chapter2_1_SceneInitializer ch21)
    {
        _ch21 = ch21;
    }
    public void GetSample()
    {
        DataManager.Instance.ChangeCondition("Sample", true);
        ScenarioManager.Instance.SetAchieve(ScenarioAchieve.BloodCapsule, true);
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.ObjectInteract, true); // bool
        UIManager.Instance.dialogueEnd += GetSampleInteractEnd;
        UIManager.Instance.DialogueOpen(Dialogue.Interaction, false, 49); // story

        _ch21.jys.dialogue = Dialogue.Main; // Story
        _ch21.jys.DialogNum = 80;
        _ch21.jys.is2D = true;
        _ch21.jys.onInteractEnd += MonsterMove;
    }
    private void GetSampleInteractEnd()
    {
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.ObjectInteract, false); // bool
    }

    public void MonsterMove(NpcName npcName)
    {
        DataManager.Instance.sceneDataManager.AutoDataSave(10);

        if (!DataManager.Instance.NPC_Alive["제영섭"])
        {
            _ch21.jys.gameObject.SetActive(false);
            foreach(var jj in _ch21.jysDead)
            {
                jj.SetActive(true);
            }
            //eyeTypeNormalMonster.SetActive(true);
            NormalMonstersSetActive();
            ScenarioManager.Instance.SetAchieve(ScenarioAchieve.JYS_Dead, true);
            GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.OnlyInteractLocked, false);
        }
        else
        {
            _ch21.jys.FollowStart();
            _ch21.jys.IsInteractable = false;
            GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.PlayerMove, true); // bool
        }

        //restaurantDoorDel.SetActive(false);
        //restaurantDoorOn.SetActive(true);
        // bgm 비활성화
        //SoundManager.Instance.PlayBGM(SoundManager.Instance.BGM_Source, BGM_Sound.Day1Follow, true, 0.5f, 0, false);
        UIManager.Instance.objective.ClearAll();
        UIManager.Instance.objective.SetMarkerByKeyAt(0,"RunElevator");
        UIManager.Instance.objective.SetMarkerByKeyAt(1,"RunElevator");
        UIManager.Instance.objective.FollowAt(0, _ch21.elevatorTarget1, ObjectiveAction.Push);
        UIManager.Instance.objective.FollowAt(1, _ch21.elevatorTarget2, ObjectiveAction.Push);
        UIManager.Instance.questUI.AddQuest("RunElevator");
        NotebookManager.Instance.NotebookMission.UpdateMission("RunElevator");
        MonsterMoveAfter();
    }

    private void NormalMonstersSetActive()
    {
        foreach (var eye in _ch21.eyeTypeNormalMonsters)
        {
            eye.SetActive(true);
        }
    }

    private void MonsterMoveAfter()
    {
        var sequence = DOTween.Sequence();

        sequence.AppendInterval(1f);
        
        if (!ScenarioManager.Instance.GetAchieve(ScenarioAchieve.JYS_Dead))
        {
            sequence.AppendCallback(() => _ch21.eyeTypeMonster.gameObject.SetActive(true));
            sequence.AppendCallback(() =>_ch21.eyeTypeMonster.MonsterWork(true));
        }
        
        sequence.AppendInterval(4f);

        foreach (var ele in _ch21.elevator)
        {
             ele.IsInteractable = true;
             ele.sceneEnum = SceneEnum.Demo;
             ele.OffInteract += _ch21.SceneClear;
             ele.MaterialChange();
        }
        
        if (DataManager.Instance.NPC_Alive["제영섭"])
        {
            sequence.AppendCallback(() =>UIManager.Instance.DialogueOpen(Dialogue.Main, false, 90)); // story
        }

        sequence.Play();
    }
}
