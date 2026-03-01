using DG.Tweening;
using UnityEngine;

public class SafeDoor_Obj : InteractableBase
{
    [field: Header("InteractType")]
    [field: SerializeField] public override InteractTypeEnum InteractType { get; set; } = InteractTypeEnum.Tap;

    [field: Header("Interactable")]
    [field: SerializeField] public override bool IsInteractable { get; set; } = true;
    [field: SerializeField] public override float InteractHoldTime { get; set; }

    public override void Interact()
    {
        
    }

    [SerializeField] private DOTweenAnimation doorOpen;
    
    private void OpenDoor()
    {
        IsInteractable = false;
        
        doorOpen.CreateTween(transform);
        doorOpen.DOPlay();
    }
}
