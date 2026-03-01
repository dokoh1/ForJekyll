using UnityEngine;

public class Caf_Door : InteractableBase
{
    [field: Header("InteractType")]
    [field: SerializeField] public override InteractTypeEnum InteractType { get; set; } = InteractTypeEnum.Tap;

    [field: Header("Interactable")]
    [field: SerializeField] public override bool IsInteractable { get; set; }
    [field: SerializeField] public override float InteractHoldTime { get; set; }
    
    [SerializeField] private Material material;

    public AudioClip openSound;

    public override void Interact()
    {
        UIManager.Instance.DialogueOpen(Dialogue.Interaction, true, 27);
        PManagers.Sound.Play(ESound.SFX, openSound);
    }

    public void AddMaterial()
    {
        Renderer renderer = GetComponent<Renderer>();
        Material[] materials = renderer.materials;
        materials[0] = material;
        renderer.materials = materials;
    }
}
