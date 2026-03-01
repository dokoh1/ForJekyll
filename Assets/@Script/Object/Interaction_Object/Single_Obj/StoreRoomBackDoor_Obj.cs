using UnityEngine;
using Zenject;

public class StoreRoomBackDoor_Obj : InteractableBase
{
    [field: Header("InteractType")]
    [field: SerializeField] public override InteractTypeEnum InteractType { get; set; } = InteractTypeEnum.Tap;

    [field: Header("Interactable")]
    [field: SerializeField] public override bool IsInteractable { get; set; } = false;
    [field: SerializeField] public override float InteractHoldTime { get; set; }

    [SerializeField] private Chapter4_1_SceneInitializer _sceneInitializer;
    [SerializeField] private Material material;
    [SerializeField] private MeshRenderer[] rends;
    
    public override void Interact()
    {
        IsInteractable = false;
        GameManager.Instance.fadeManager.MoveScene(SceneEnum.Chapter4_2);
    }

    public void ChangeMaterial()
    {
        foreach (MeshRenderer rend in rends)
        {
            Material[] materials = rend.materials;
            materials[0] = material;
            rend.materials = materials;
        }
    }  
}
