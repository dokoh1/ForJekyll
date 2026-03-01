using UnityEngine;
using Zenject;

public class ConVentilation_Obj : InteractableBase
{
    [field: Header("InteractType")]
    [field: SerializeField] public override InteractTypeEnum InteractType { get; set; } = InteractTypeEnum.Tap;

    [field: Header("Interactable")]
    [field: SerializeField] public override bool IsInteractable { get; set; } = true;
    [field: SerializeField] public override float InteractHoldTime { get; set; }

    public override void Interact()
    {
        IsInteractable = false;
        UIManager.Instance.dialogueEnd += StoryEnd;
        UIManager.Instance.DialogueOpen(Dialogue.Main, true, 108);
    }

    void StoryEnd()
    {
        ScenarioManager.Instance.ResetNpcFavorInteract();
        GameManager.Instance.fadeManager.MoveScene(SceneEnum.Chapter4_1);
    }
}
