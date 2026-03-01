using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard_Obj : InteractableBase
{
    [field: Header("InteractType")]
    [field: SerializeField] public override InteractTypeEnum InteractType { get; set; } = InteractTypeEnum.Tap;

    [field: Header("Interactable")]
    [field: SerializeField] public override bool IsInteractable { get; set; } = true;
    [field: SerializeField] public override float InteractHoldTime { get; set; }


    public override void Interact()
    {
        Debug.Log("경비원 확인");

        UIManager.Instance.DialogueOpen(Dialogue.Interaction, false, 151); 

        IsInteractable = false;
    }
}