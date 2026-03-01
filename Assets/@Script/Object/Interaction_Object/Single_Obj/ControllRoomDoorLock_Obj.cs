using DG.Tweening;
using UnityEngine;
using Zenject;

public class ControllRoomDoorLock_Obj : InteractableBase
{
    [field: Header("InteractType")]
    [field: SerializeField] public override InteractTypeEnum InteractType { get; set; } = InteractTypeEnum.Tap;

    [field: Header("Interactable")]
    [field: SerializeField] public override bool IsInteractable { get; set; } = true;
    [field: SerializeField] public override float InteractHoldTime { get; set; }

    [SerializeField] private Door_Obj[] doorObj;
    [SerializeField] private AudioSource audioSource;
    
    public override void Interact()
    {
        IsInteractable = false;

        if (ScenarioManager.Instance.GetAchieve(ScenarioAchieve.haveCTR_RoomCard))
        {
            audioSource.Play();
            foreach (Door_Obj d in doorObj)
            {
                d.IsInteractable = true;
                d.canOpen = true;
            }
        }
        else
        {
            GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.ObjectInteract, true);
            UIManager.Instance.dialogueEnd += InteractAnd;
            UIManager.Instance.DialogueOpen(Dialogue.Interaction, false, 89);
        }
    }

    void InteractAnd()
    {
        IsInteractable = true;
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.ObjectInteract, false);
    }

    public void AniEnd()
    {
        UIManager.Instance.DialogueOpen(Dialogue.Main, true, 20);
    }
}
