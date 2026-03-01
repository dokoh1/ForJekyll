using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Elevator_Obj : InteractableBase
{
    [field: Header("InteractType")]
    [field: SerializeField] public override InteractTypeEnum InteractType { get; set; } = InteractTypeEnum.Tap;

    [field: Header("Interactable")]
    [field: SerializeField] public override bool IsInteractable { get; set; } = false;
    [field: SerializeField] public override float InteractHoldTime { get; set; }

    [Header("Dot")]
    [SerializeField] private DOTweenAnimation[] doorOpenAni;
    [SerializeField] private DOTweenAnimation[] doorCloseAni;
    
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip elevatorOpen;
    [SerializeField] private AudioClip elevatorBell;
    [SerializeField] private AudioClip elevatorClose;
   
    [Header("GameObject")]
    [SerializeField] private GameObject[] monsters;
    [SerializeField] private Material highlightMat;
    [SerializeField] private Renderer renderer;

    public SceneEnum sceneEnum;
    public int interactionIndex = 39;
    public bool interactTalk = false;
    
    public override void Interact()
    {
        IsInteractable = false;
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.OnlyInteractLocked, true);
        if (interactTalk)
        {
            UIManager.Instance.dialogueEnd += InteractEnd;
            UIManager.Instance.DialogueOpen(Dialogue.Interaction, false, interactionIndex);

            return;
        }
        StartCoroutine(ElevatorOpen());
    }
    public void AniEnd()
    {
        OnEvent(InteractEventType.Off);
        ScenarioManager.Instance.ResetNpcFavorInteract();
        GameManager.Instance.fadeManager.MoveScene(sceneEnum);
        GameManager.Instance.fadeManager.fadeComplete += ToggleBool;
    }

    private void ToggleBool()
    {
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.OnlyInteractLocked, false);
    }

    public IEnumerator ElevatorOpen()
    {
        if (monsters != null)
        {
            foreach (GameObject monster in monsters)
            {
                monster.SetActive(false);
            }
        }

        audioSource.clip = elevatorBell;
        audioSource.Play();
        
        yield return new WaitForSeconds(audioSource.clip.length);

        audioSource.clip = elevatorOpen;
        audioSource.Play();

        foreach (var anim in doorOpenAni)
        {
            anim.DOKill();
            anim.CreateTween();
        }
    }

    private void InteractEnd()
    {
        IsInteractable = true;
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.OnlyInteractLocked, false);
    }

    public void CloseElevator()
    {
        foreach (var anim in doorCloseAni)
        {
            anim.DOKill();
            anim.CreateTween();
        }
        
        audioSource.clip = elevatorClose;
        audioSource.Play();
    }

    public void MaterialChange()
    {
        if (renderer == null) return;
        
        var mat = renderer.materials;

        for (var i = 0; i < mat.Length; i++)
        {
            mat[0] = highlightMat;
        }
        
        renderer.materials = mat;

    }
}
