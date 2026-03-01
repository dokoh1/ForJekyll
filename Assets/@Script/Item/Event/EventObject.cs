using UnityEngine;

public class EventObject : InteractableBase
{

    [field: Header("InteractType")]
    [field: SerializeField] public override InteractTypeEnum InteractType { get; set; } = InteractTypeEnum.Tap;

    [field: Header("Interactable")]
    [field: SerializeField] public override bool IsInteractable { get; set; } = true;
    [field: SerializeField] public override float InteractHoldTime { get; set; }


    public override void Interact()
    {
        Debug.Log("Event Object Interact - Start Lavin's Code");
    }
}