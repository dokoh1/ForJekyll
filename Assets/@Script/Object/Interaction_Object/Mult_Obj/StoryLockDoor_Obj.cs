public class StoryLockDoor_Obj : Door_Obj
{
    public bool storyLock;
    public int storyIndex;
    public bool isFinishChapter;

    public SceneEnum sceneEnum;

    public override void Interact()
    {
        if (GameManager.Instance.IsTimeStop) return;
        IsInteractable = false;

        if (isFinishChapter)
        {
            FinishChapter();
        }

        if (storyLock)
        {
            DoorStoryLock(storyIndex);
            return;
        }

        if (!canOpen) { DoorLock(); }
        else { DoorOpen(); }
    }

    public void DoorStoryLock(int storyIndex)
    {
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.ObjectInteract, true);
        UIManager.Instance.dialogueEnd += StoryLockDoorEnd;
        UIManager.Instance.DialogueOpen(Dialogue.Interaction, false, storyIndex);
    }

    private void StoryLockDoorEnd()
    {
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.ObjectInteract, false);
    }

    private void FinishChapter()
    {
        GameManager.Instance.fadeManager.MoveScene(sceneEnum);
    }
}