using DG.Tweening;
using UnityEngine;
using Zenject;

public class Laver_Obj : InteractableBase
{
    [field: Header("InteractType")]
    [field: SerializeField] public override InteractTypeEnum InteractType { get; set; } = InteractTypeEnum.Tap;

    [field: Header("Interactable")]
    [field: SerializeField] public override bool IsInteractable { get; set; } = true;
    [field: SerializeField] public override float InteractHoldTime { get; set; }

    [SerializeField] private DOTweenAnimation laverUp;
    [SerializeField] private AudioSource audioSource;
    
    [Inject] private Chapter2_3_Manager chapter2_3_Manager;

    public bool isLaverUp = false;
    public override void Interact()
    {
        IsInteractable = false;

        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        audioSource.Play();
        laverUp.DOKill();
        laverUp.CreateTween();
    }
}
