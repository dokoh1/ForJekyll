using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityOffice_Obj : Door_Obj
{
    public override void Interact()
    {
        IsInteractable = false;

        if (ScenarioManager.Instance.GetAchieve(ScenarioAchieve.SecurityOfficeKey))
        {
            DoorOpen();
        }
        else
        {
            DoorLock();
            GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.ObjectInteract, true);
            UIManager.Instance.dialogueEnd += SecurityOfficeRoomEnd;
            UIManager.Instance.DialogueOpen(Dialogue.Interaction, false, 00);
        }
    }

    private void SecurityOfficeRoomEnd()
    {
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.ObjectInteract, false);
    }
}