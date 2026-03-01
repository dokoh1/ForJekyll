using System;
using UnityEngine;
public class Pan_Obj : InteractableBase
{
    [field: Header("InteractType")]
    [field: SerializeField] public override InteractTypeEnum InteractType { get; set; } = InteractTypeEnum.Tap;

    [field: Header("Interactable")]
    [field: SerializeField] public override bool IsInteractable { get; set; } = true;
    [field: SerializeField] public override float InteractHoldTime { get; set; }
    
    [SerializeField] private GameObject pen;
    [SerializeField] private GameObject ebtn;
    
    public override void Interact()
    {
        IsInteractable = false;
        UIManager.Instance.DialogueOpen(Dialogue.Main, false, 4);
        OnEvent(InteractEventType.Off);
        Destroy(ebtn);
        Destroy(pen);
    }
}
