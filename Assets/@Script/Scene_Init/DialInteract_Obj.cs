using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class DialInteract_Obj : InteractableBase
    {
        public override InteractTypeEnum InteractType { get; set; } = InteractTypeEnum.Tap;
        public override bool IsInteractable { get; set; } = true;
        public override float InteractHoldTime { get; set; }

        public bool repeat;
        public int dialogueNum;
        public override void Interact()
        {
            IsInteractable = false;
            GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.ObjectInteract, true);
            UIManager.Instance.dialogueEnd += InteractEnd;
            UIManager.Instance.DialogueOpen(Dialogue.Interaction, false ,dialogueNum);
        }

        private void InteractEnd()
        {
            GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.ObjectInteract, false);
            if (repeat)
            {
                IsInteractable = true;
            }
        }
    }