using System;
using UnityEngine;
public class FlashLight_Obj : InteractableBase
{
    [field: Header("InteractType")]
    [field: SerializeField] public override InteractTypeEnum InteractType { get; set; } = InteractTypeEnum.Tap;

    [field: Header("Interactable")]
    [field: SerializeField] public override bool IsInteractable { get; set; } = true;
    [field: SerializeField] public override float InteractHoldTime { get; set; }

    [SerializeField] private Look_Interaction lookInteraction;
    public event Action GetFlashLight;

    public override void Interact()
    {
        IsInteractable = false;

        if (!IsInteractable)
        {
            UIManager.Instance.objective.ClearAll();
            SoundManager.Instance.PlaySE(Obj_Sound.ItemPickUp);
            UIManager.Instance.questUI.AddQuest("MoveOpposite2F");
            NotebookManager.Instance.NotebookMission.UpdateMission("MoveOpposite2F");
            GetFlashLight?.Invoke();
            UIManager.Instance.DialogueOpen(Dialogue.Interaction, false, 34);
        }

        OnEvent(InteractEventType.Off);
        ScenarioManager.Instance.SetAchieve(ScenarioAchieve.PlayerFlashLight, true);
        GameManager.Instance.Player.PlayerUIBlinker.StartBlink();

        // 임시 수정 - 마스크, 시간정지 비활성화
        //playerRoomDoor.ChangeDoor();

        Destroy(lookInteraction.gameObject);
        Destroy(gameObject);
    }
}
