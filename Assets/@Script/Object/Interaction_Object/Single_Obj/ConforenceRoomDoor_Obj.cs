using UnityEngine;

public class ConferenceRoomDoor_Obj : InteractableBase
{
    [field: Header("InteractType")]
    [field: SerializeField] public override InteractTypeEnum InteractType { get; set; } = InteractTypeEnum.Tap;

    [field: Header("Interactable")]
    [field: SerializeField] public override bool IsInteractable { get; set; } = false;
    [field: SerializeField] public override float InteractHoldTime { get; set; }

    [SerializeField] private EyeTypeMonster[] monsters;
    [SerializeField] private Material material;
    [SerializeField] private MeshRenderer rend;


    public override void Interact()
    {
        IsInteractable = false;

        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.OnlyInteractLocked, true);
        GameManager.Instance.fadeManager.fadeComplete += InteractEnd;
        GameManager.Instance.fadeManager.MoveScene(SceneEnum.Chapter3_3);

        if (monsters != null)
        {
            foreach (var monster in monsters)
            {
                monster.gameObject.SetActive(false);
            }
        }
    }

    private void InteractEnd()
    {
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.OnlyInteractLocked, false);
    }

    public void AddMaterial()
    {
        Material[] materials = rend.materials;
        materials[0] = material;
        rend.materials = materials;
    }
}
