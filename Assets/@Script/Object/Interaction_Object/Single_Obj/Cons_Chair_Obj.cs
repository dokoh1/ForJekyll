using UnityEngine;

public class Cons_Chair_Obj : InteractableBase
{
    [field: Header("InteractType")]
    [field: SerializeField] public override InteractTypeEnum InteractType { get; set; } = InteractTypeEnum.Tap;

    // IInteractable
    [field: Header("Interactable")]
    [field: SerializeField] public override bool IsInteractable { get; set; } = true;
    [field: SerializeField] public override float InteractHoldTime { get; set; }

    private ConforenceScene_Manager _conforenceSceneManager => ConforenceScene_Manager.Instance;

    public override void Interact()
    {
        if (_conforenceSceneManager.bossBattleScript.playerPickUp) return;
        
        _conforenceSceneManager.playerChair.SetActive(true);
        _conforenceSceneManager.bossBattleScript.playerPickUp = true;
        Destroy(this.gameObject);
    }
}
