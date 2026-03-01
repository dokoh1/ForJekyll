using UnityEngine;

public class TutorialKey_Obj : InteractableBase
{
    public override InteractTypeEnum InteractType { get; set; } = InteractTypeEnum.Tap;
    public override bool IsInteractable { get; set; } = true;
    public override float InteractHoldTime { get; set; }

    public AudioClip keyPickUpSound;

    [SerializeField] private Tutorial_SceneInitializer tuto;
    public override void Interact()
    {
        IsInteractable = false;
        UIManager.Instance.objective.Follow(tuto.buttonTarget, ObjectiveAction.Push);
        tuto.barrierKey = true;
        
        PManagers.Sound.Play(ESound.SFX, keyPickUpSound);
        
        Destroy(gameObject);
    }
}