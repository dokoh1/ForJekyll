using DG.Tweening;
using UnityEngine;

public class ShutterButtonBox_Obj : InteractableBase
{
    public override InteractTypeEnum InteractType { get; set; } = InteractTypeEnum.Tap;
    public override bool IsInteractable { get; set; } = true;
    public override float InteractHoldTime { get; set; }

    public AudioClip keyOpenSound;
    public AudioClip buttonOpenSound;

    [SerializeField] private Tutorial_SceneInitializer tuto;
    [SerializeField] private GameObject keyIn;
    [SerializeField] private DOTweenAnimation buttonBoxGlassAni;
    [SerializeField] private ButtonBoxBtn_Obj buttonBoxBtn;

    public override void Interact()
    {
        if (!tuto.barrierKey)
        {
            if(UIManager.Instance.isInDialogue) return;

            UIManager.Instance.DialogueOpen(Dialogue.Interaction, false, 23);
            
            return;
        }
        
        IsInteractable = false;

        keyIn.SetActive(true);
        PManagers.Sound.Play(ESound.SFX, keyOpenSound);
    }

    public void KeyAniEnd()
    {
        buttonBoxGlassAni.DOKill();
        buttonBoxGlassAni.CreateTween(true);
        DOVirtual.DelayedCall(0.7f, () =>
    {
        PManagers.Sound.Play(ESound.SFX, buttonOpenSound);
    });
    }

    public void ButtonBoxGlassAniEnd()
    {
        buttonBoxBtn.IsInteractable = true;
        buttonBoxBtn.AddHighLightMat();
    }
}
