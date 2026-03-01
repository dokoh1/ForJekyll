using UnityEngine;

public class Knife_Obj : InteractableBase
{
    [field: Header("InteractType")]
    [field: SerializeField] public override InteractTypeEnum InteractType { get; set; } = InteractTypeEnum.Tap;

    [field: Header("Interactable")]
    [field: SerializeField] public override bool IsInteractable { get; set; } = true;
    [field: SerializeField] public override float InteractHoldTime { get; set; }

    [SerializeField] private AudioClip _knifePickUpSound;

    
    public override void Interact()
    {
        IsInteractable = false;
        
        DataManager.Instance.ChangeCondition("Knife", true);
        ScenarioManager.Instance.SetAchieve(ScenarioAchieve.Knife, true);
        PManagers.Sound.Play(ESound.SFX, _knifePickUpSound);

        Destroy(gameObject);
    }
}
