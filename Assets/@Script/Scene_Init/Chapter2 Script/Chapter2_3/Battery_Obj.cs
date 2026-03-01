using UnityEngine;

public class Battery_Obj : InteractableBase
{
    [field: Header("InteractType")]
    [field: SerializeField] public override InteractTypeEnum InteractType { get; set; } = InteractTypeEnum.Tap;

    [field: Header("Interactable")]
    [field: SerializeField] public override bool IsInteractable { get; set; } = true;
    [field: SerializeField] public override float InteractHoldTime { get; set; }

    public override void Interact()
    {
        SoundManager.Instance.PlaySE(Obj_Sound.ItemPickUp);

        //ParkingSceneManager.Instance.PlayerBatteryCount++;
        
        Destroy(gameObject);
    }
}
