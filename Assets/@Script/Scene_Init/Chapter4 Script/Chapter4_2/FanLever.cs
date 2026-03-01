using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanLever : InteractableBase
{
    [field: Header("InteractType")]
    [field: SerializeField] public override InteractTypeEnum InteractType { get; set; } = InteractTypeEnum.Tap;

    [field: Header("Interactable")]
    [field: SerializeField] public override bool IsInteractable { get; set; } = true;
    [field: SerializeField] public override float InteractHoldTime { get; set; }

    [SerializeField] FanSystemManager[] fanSystemManagers;
    [SerializeField] DOTweenAnimation leverUp;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip leverSound;

    public bool CutScenePlayed;
    public event Action OnLeverAction;

    public override void Interact()
    {
        IsInteractable = false;

        if (CutScenePlayed)
        {
            OnLeverAction?.Invoke();
            Debug.Log("CutScene Played");
            return;
        }

        LeverAction();
    }

    public void LeverAction()
    {
        foreach (var fan in fanSystemManagers)
        {
            if (fan != null)
                fan.ActivateFan();
        }

        if (leverUp != null)
            leverUp.DOPlayForward();

        if (audioSource != null || leverSound != null)
        {
            audioSource.clip = leverSound;
            audioSource.Play();
        }
    }
}