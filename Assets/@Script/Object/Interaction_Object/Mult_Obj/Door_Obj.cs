using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public class Door_Obj : InteractableBase
{
    [field: Header("InteractType")]
    [field: SerializeField] public override InteractTypeEnum InteractType { get; set; } = InteractTypeEnum.Tap;

    [field: Header("Interactable")]
    [field: SerializeField] public override bool IsInteractable { get; set; } = true;
    [field: SerializeField] public override float InteractHoldTime { get; set; }

    public bool canOpen;
    public event Action endOpen;
    [Header("Dot")]
    [SerializeField] protected DOTweenAnimation doorOpen;
    [SerializeField] protected DOTweenAnimation doorLock;

    [Header("DoorClip")]
    [SerializeField] private AudioClip doorOpenSound;
    [SerializeField] private AudioClip doorLockSound;

    [Header("DoorAudioSource")]
    [SerializeField] protected AudioSource audioSource;

    [Header("Material")]
    [SerializeField] private Material changeMaterial;
    [SerializeField] private Renderer renderer;

    [Title("NavMeshObstacle")]
    [SerializeField] public NavMeshObstacle navMeshObstacle;

    public float GetLockDuration() => doorLock.duration;

    protected override void Start()
    {
        base.Start();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        if (ScenarioManager.Instance.GetAchieve(ScenarioAchieve.Chapter1_2_WithYHJ))
        {
            if (changeMaterial != null)
            {
                GameManager.Instance.HighLightMaterialDelete(GetComponent<Renderer>(), changeMaterial);
            }
        }
    }
    public override void Interact()
    {
        if (GameManager.Instance.IsTimeStop) return;
        IsInteractable = false;

        if (!canOpen) { DoorLock(); }
        else { DoorOpen(); }
    }

    public void EndOpen()
    {
        endOpen?.Invoke();
        endOpen = null;
    }

    public void AnimationEnd()
    {
        IsInteractable = true;
    }

    public void DoorOpen()
    {
        if (audioSource != null || doorOpenSound != null)
        {
            audioSource.clip = doorOpenSound;
            audioSource.Play();
        }

        doorOpen.DOKill();
        doorOpen.CreateTween();

        if (changeMaterial != null)
        {
            GameManager.Instance.HighLightMaterialDelete(renderer, changeMaterial);
        }

        if (navMeshObstacle != null) { navMeshObstacle.enabled = false; }

        var boxCollider = GetComponent<BoxCollider>();
        boxCollider.enabled = false;

        EndOpen();
    }

    public void DoorLock()
    {
        if (audioSource != null || doorLockSound != null)
        {
            audioSource.clip = doorLockSound;
            audioSource.Play();
        }
        
        doorLock.DOKill();
        doorLock.CreateTween();

        UIManager.Instance.DialogueOpen(Dialogue.Interaction, false, 23);
    }

    public void ReWindAnimation() { doorOpen.DORewind(); }
}
