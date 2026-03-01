using DG.Tweening;
using UnityEngine;

public class GeneratorSwitch_Obj : InteractableBase
{
    [field: Header("InteractType")]
    [field: SerializeField] public override InteractTypeEnum InteractType { get; set; } = InteractTypeEnum.Tap;

    [field: Header("Interactable")]
    [field: SerializeField] public override bool IsInteractable { get; set; } = false;
    [field: SerializeField] public override float InteractHoldTime { get; set; }
    
    [SerializeField] private GeneratorPuzzle generatorPuzzle;
    [SerializeField] private DOTweenAnimation upAni;
    [SerializeField] private DOTweenAnimation downAni;
    
    [SerializeField] private int switchNum;
    public bool isUp;
    public void Init(GeneratorPuzzle g)
    {
        generatorPuzzle = g;
        IsInteractable = true;
    }
    public override void Interact()
    {
        if (isUp)
        {
            downAni.DOKill();
            downAni.CreateTween();
            isUp = false;
        }
        else
        {
            upAni.DOKill();
            upAni.CreateTween();
            isUp = true;
        }
        
        IsInteractable = false;
        SoundManager.Instance.PlaySE(Obj_Sound.GeneratorSwitch);
    }

    public void AniEnd()
    {
        IsInteractable = true;
        generatorPuzzle.SwitchToggle(switchNum);
    }
}
