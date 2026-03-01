using UnityEngine;
public class Interaction_Obj : InteractableBase
{
    [field: Header("InteractType")]
    [field: SerializeField] public override InteractTypeEnum InteractType { get; set; } = InteractTypeEnum.Tap;

    [field: Header("Interactable")]
    [field: SerializeField] public override bool IsInteractable { get; set; } = true;
    [field: SerializeField] public override float InteractHoldTime { get; set; }

    [SerializeField] private string objectID;
    [SerializeField] private Sprite sprite;

    public override void Interact()
    {
        if (UI_InteractionPopup.Instance != null)
        {
            UI_InteractionPopup.Instance.SetInteractionUI(sprite, objectID);
            UI_InteractionPopup.Instance.OpenPopup();
        }
    }
}