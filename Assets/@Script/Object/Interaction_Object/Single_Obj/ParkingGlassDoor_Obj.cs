using DG.Tweening;
using UnityEngine;

public class ParkingGlassDoor_Obj : InteractableBase
{
    [field: Header("InteractType")]
    [field: SerializeField] public override InteractTypeEnum InteractType { get; set; } = InteractTypeEnum.Tap;

    [field: Header("Interactable")]
    [field: SerializeField] public override bool IsInteractable { get; set; } = true;
    [field: SerializeField] public override float InteractHoldTime { get; set; }

    [SerializeField] private DOTweenAnimation[] doorOpenAni;
    public override void Interact()
    {
        IsInteractable = false;

        BoxCollider box = gameObject.GetComponent<BoxCollider>();
        box.enabled = false;

        foreach (var door in doorOpenAni)
        {
            door.DOKill();
            door.CreateTween();
        }
    }
}
