using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class GeneratorRoomKey : InteractableBase
    {
        public override InteractTypeEnum InteractType { get; set; } = InteractTypeEnum.Tap;
        public override bool IsInteractable { get; set; } = true;
        public override float InteractHoldTime { get; set; }
        public override void Interact()
        {
            ScenarioManager.Instance.SetAchieve(ScenarioAchieve.GeneratorRoomKey, true);
            
            Destroy(gameObject);
        }
    }