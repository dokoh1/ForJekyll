using UnityEngine;

public class GasMask_Obj : InteractableBase
{
    public override InteractTypeEnum InteractType { get; set; } = InteractTypeEnum.Tap;
    public override bool IsInteractable { get; set; } = true;
    public override float InteractHoldTime { get; set; }
    
    [SerializeField] private GasMaskNote_Obj gasMaskObj;
    public override void Interact()
    {
        ScenarioManager.Instance.SetAchieve(ScenarioAchieve.GasMask, true);
        OnEvent(InteractEventType.Off);
        Destroy(gameObject);
    }
}