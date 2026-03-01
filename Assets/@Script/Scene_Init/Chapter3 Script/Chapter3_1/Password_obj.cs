using System;
using UnityEngine;
//using VFolders.Libs;
public class Password_obj : InteractableBase
{
    [field: Header("InteractType")]
    [field: SerializeField] public override InteractTypeEnum InteractType { get; set; } = InteractTypeEnum.Tap;

    [field: Header("Interactable")]
    [field: SerializeField] public override bool IsInteractable { get; set; } = true;
    [field: SerializeField] public override float InteractHoldTime { get; set; }

    public event Action OnPasswordUsed;


    public override void Interact()
    {
        IsInteractable = false;
        UIManager.Instance.DialogueOpen(Dialogue.Main, false, 42);
        OnPasswordUsed?.Invoke();
        Destroy(gameObject);
    }
}
