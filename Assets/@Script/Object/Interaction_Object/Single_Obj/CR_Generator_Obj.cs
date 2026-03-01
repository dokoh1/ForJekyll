using UnityEngine;

public class CR_Generator_Obj : InteractableBase
{
    private ConforenceScene_Manager _conforenceSceneManager => ConforenceScene_Manager.Instance;

    [field: Header("InteractType")]
    [field: SerializeField] public override InteractTypeEnum InteractType { get; set; } = InteractTypeEnum.Tap;

    [field: Header("Interactable")]
    [field: SerializeField] public override bool IsInteractable { get; set; } = false;
    [field: SerializeField] public override float InteractHoldTime { get; set; }

    public bool isEnable { get; private set; } = false;
    [SerializeField] private GameObject pointLight;
    
    [SerializeField] private Renderer[] renderers;
    [SerializeField] private Material noneMaterial;
    [SerializeField] private Material pointMaterial;

    public override void Interact()
    {
        IsInteractable = false;
        isEnable = false;
        pointLight.SetActive(false);
        MaterialChange(noneMaterial);
    }

    protected override void Start()
    {
        base.Start();
        _conforenceSceneManager.bossBattleScript.generatorObjs.Add(this);
    }

    public void GeneratorOn()
    {
        IsInteractable = true;
        isEnable = true;
        pointLight.SetActive(true);
        MaterialChange(pointMaterial);
    }

    private void MaterialChange(Material material)
    {
        foreach (var go in renderers)
        {
            Material[] materials = go.materials;
            materials[0] = material;
            go.materials = materials;
        }
    }
}
