using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diary_Obj : InteractableBase
{
    [field: Header("InteractType")]
    [field: SerializeField] public override InteractTypeEnum InteractType { get; set; } = InteractTypeEnum.Tap;

    [field: Header("Interactable")]
    [field: SerializeField] public override bool IsInteractable { get; set; } = true;
    [field: SerializeField] public override float InteractHoldTime { get; set; }
    
    public event Action OnInteract;
    public override void Interact()
    {
        IsInteractable = false;
        Debug.Log("��ø Ȯ��");
        //�Թμ� ��� 
        //UIManager.Instance.DialogueOpen(Dialogue.Interaction, false, dialogueID);
        OnInteract?.Invoke();
    }
}