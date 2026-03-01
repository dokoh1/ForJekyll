using DG.Tweening;
using UnityEngine;

public class BarrierConBox_Obj : InteractableBase
{
    public override InteractTypeEnum InteractType { get; set; } = InteractTypeEnum.Tap;
    public override bool IsInteractable { get; set; } = true;
    public override float InteractHoldTime { get; set; }
    
    [SerializeField] private Tutorial_SceneInitializer tuto;
    [SerializeField] private DOTweenAnimation glassCubeAni;
    public override void Interact()
    {
        if (tuto.barrierKey)
        {
            IsInteractable = false;
            glassCubeAni.DOKill();
            glassCubeAni.CreateTween(true);
        }
    }
}
