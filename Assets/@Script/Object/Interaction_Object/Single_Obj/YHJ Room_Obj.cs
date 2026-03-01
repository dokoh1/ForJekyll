public class YHJ_Room_Obj : Door_Obj
{
    public bool IsStoryEnd { get; set; }
    public override void Interact()
    {
        IsInteractable = false;

        if (!IsStoryEnd)
        {
            if (!ScenarioManager.Instance.GetAchieve(ScenarioAchieve.PlayerFlashLight))
            {
                GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.ObjectInteract, true); // bool
                UIManager.Instance.DialogueOpen(Dialogue.Interaction, false, 25); // story
                UIManager.Instance.dialogueEnd += StoryEnd;
            }
            else
            {
                ScenarioManager.Instance.SetAchieve(ScenarioAchieve.Chapter1_2_WithYHJ, true);
                IsStoryEnd = true;
                UIManager.Instance.dialogueEnd += EndEvent;
                //UIManager.Instance.DialogueStart += DoorOpenAni;
                UIManager.Instance.DialogueOpen(Dialogue.Main,true, 155); // story
            } 
        }
        else
        {
            IsInteractable = false;
            DoorOpen();
        }
    }
    private void StoryEnd()
    {
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.ObjectInteract, false); // bool
        IsInteractable = true;
    }

    private void EndEvent() { OnEvent(InteractEventType.Off); }
}
