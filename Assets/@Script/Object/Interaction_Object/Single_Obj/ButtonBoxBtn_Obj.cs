using DG.Tweening;
using Marker;
using UnityEngine;

public class ButtonBoxBtn_Obj : InteractableBase
{
    public override InteractTypeEnum InteractType { get; set; } = InteractTypeEnum.Tap;
    public override bool IsInteractable { get; set; } = false;
    public override float InteractHoldTime { get; set; }

    public AudioClip buttonSound;

    [SerializeField] private Shutter_Obj shutterObj;
    [SerializeField] private QuestTarget shutterTarget;
    [SerializeField] private DOTweenAnimation shutterSlowDown;
    [SerializeField] private DOTweenAnimation shutterFoldAni;
    
    [SerializeField] private Renderer renderer;
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private Material defaultMaterial;
    public override void Interact()
    {
        IsInteractable = false;

        shutterSlowDown.DOKill();
        shutterSlowDown.CreateTween(true);

        PManagers.Sound.Play(ESound.SFX, buttonSound);
        GameManager.Instance.HighLightMaterialDelete(renderer, defaultMaterial);
        UIManager.Instance.objective.Follow(shutterTarget, ObjectiveAction.Push);
        OnEvent(InteractEventType.Off);
    }

    public void ShutterFold()
    {
        shutterObj.IsInteractable = true;
        shutterFoldAni.DOKill();
        shutterFoldAni.CreateTween(true);
        shutterObj.MaterialChange();
    }

    public void AddHighLightMat()
    {
        var mat = renderer.materials;
        mat[0] = highlightMaterial;
        renderer.materials = mat;
    }
}