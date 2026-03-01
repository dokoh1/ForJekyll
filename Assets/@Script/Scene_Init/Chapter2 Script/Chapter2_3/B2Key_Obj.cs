using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B2Key_Obj : InteractableBase
{
    [field: Header("InteractType")]
    [field: SerializeField] public override InteractTypeEnum InteractType { get; set; } = InteractTypeEnum.Tap;

    [field: Header("Interactable")]
    [field: SerializeField] public override bool IsInteractable { get; set; } = true;
    [field: SerializeField] public override float InteractHoldTime { get; set; }

    public event Action PowerRoomOpen;

    public AudioClip keyPickUpSound;

    public override void Interact()
    {
        Debug.Log("B2 ¹ßÀü½Ç ¿­¼è È¹µæ");

        if(GeneratorDoor_Obj.blockedTried)
            UIManager.Instance.DialogueOpen(Dialogue.Interaction, false, 136); //¸ÕÀú ¹ßÀü½Ç¿¡ °¬´Ù°¡ µ¹¾Æ¿Í¼­ ¿­¼è È¹µæ ½Ã
        else
            UIManager.Instance.DialogueOpen(Dialogue.Interaction, false, 122); //¿­¼è¸¦ ¸ÕÀú È¹µæÇÑ °æ¿ì

        PowerRoomOpen?.Invoke();

        PManagers.Sound.Play(ESound.SFX, keyPickUpSound);

        Destroy(gameObject);
    }
}