using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EmerganceyDoor_Obj : InteractableBase
{
    [field: Header("InteractType")]
    [field: SerializeField] public override InteractTypeEnum InteractType { get; set; } = InteractTypeEnum.Tap;

    [field: Header("Interactable")]
    [field: SerializeField] public override bool IsInteractable { get; set; } = true;
    [field: SerializeField] public override float InteractHoldTime { get; set; }

    [Header("Door Materials")]
    public Material defaultMaterial;
    public Material highlightMaterial;

    private Renderer renderer;

    public AudioClip stepSound;

    public bool isOpen = false;
    private void Awake()
    {
        renderer = GetComponent<Renderer>();
    }

    public void SetHighlight(bool active)
    {
        if (renderer == null) return;

        renderer.material = active ? highlightMaterial : defaultMaterial;
    }

    public override void Interact()
    {
        if (!isOpen)
        {
            Debug.Log("¾ÆÁ÷ Å»ÃâÇÒ¼ö ¾ø´Ù");
            return;
        }

        Debug.Log("Å»Ãâ");

        GameManager.Instance.fadeManager.FadeStart(FadeState.FadeOut);

        PManagers.Sound.Play(ESound.SFX, stepSound);

        //¾À ÀüÈ¯??

        GameManager.Instance.fadeManager.MoveScene(SceneEnum.Chapter3_1);
    }
}