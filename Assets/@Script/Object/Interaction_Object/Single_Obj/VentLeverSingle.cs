using DG.Tweening;
using UnityEngine;

public class VentLeverSingle : InteractableBase
{
    [field: Header("InteractType")]
    [field: SerializeField] public override InteractTypeEnum InteractType { get; set; } = InteractTypeEnum.Tap;

    [field: Header("Interactable")]
    [field: SerializeField] public override bool IsInteractable { get; set; } = true;
    [field: SerializeField] public override float InteractHoldTime { get; set; }

    
    [SerializeField] private DOTweenAnimation leverAnimation;
    public override void Interact()
    {
        IsInteractable = false;

        leverAnimation.DOKill();
        leverAnimation.CreateTween(true);
    }
}
