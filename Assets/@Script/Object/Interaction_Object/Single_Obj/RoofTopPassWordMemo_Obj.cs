using UnityEngine;
using Zenject;

public class RoofTopPassWordMemo_Obj : InteractableBase
{
    [field: Header("InteractType")]
    [field: SerializeField] public override InteractTypeEnum InteractType { get; set; } = InteractTypeEnum.Tap;

    [field: Header("Interactable")]
    [field: SerializeField] public override bool IsInteractable { get; set; } = true;
    [field: SerializeField] public override float InteractHoldTime { get; set; }

    [SerializeField] private Material changeManterial;
    
    [Inject]private Chapter3_1_Manager chapter3_1_Manager;

    public override void Interact()
    {
        IsInteractable = false;
        GameManager.Instance.HighLightMaterialDelete(GetComponent<Renderer>(), changeManterial);
        chapter3_1_Manager.ElevatorInteractOn();
    }
}
