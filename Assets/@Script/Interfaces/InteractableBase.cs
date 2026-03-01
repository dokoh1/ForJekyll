using System;
using UnityEngine;

public abstract class InteractableBase : MonoBehaviour, IInteractable
{
    public abstract InteractTypeEnum InteractType { get; set; }// 재정의 해야함

    public abstract bool IsInteractable { get; set; }// 재정의 해야함
    public abstract float InteractHoldTime { get; set; }// 재정의 해야함

    public bool IsTabAndHold { get; set; }
    public bool CanTap { get; set; }
    public bool CanHold { get; set; }

    public abstract void Interact();// 재정의 해야함
    public virtual void Hold() { }

    public event Action OnInteract;
    public event Action OffInteract;

    protected void OnEvent(InteractEventType eventType)
    {
        if (eventType == InteractEventType.On)
        {
            OnInteract?.Invoke();
            OnInteract = null;
        }
        else
        {
            OffInteract?.Invoke();
            OffInteract = null;
        }
    }

    protected enum InteractEventType
    {
        On,
        Off
    }

    public enum InteractTypeEnum
    {
        Tap,
        Hold,
        TapAndHold
    }

    protected virtual void Start()
    {
        if (InteractType == InteractTypeEnum.Tap)
        {
            IsTabAndHold = false;
            InteractHoldTime = 0f;
            CanTap = false;
            CanHold = false;
        }
        else if (InteractType == InteractTypeEnum.Hold)
        {
            IsTabAndHold = false;
            if (InteractHoldTime <= 0f) InteractHoldTime = 1f;
            CanTap = false;
            CanHold = false;
        }
        else if (InteractType == InteractTypeEnum.TapAndHold)
        {
            IsTabAndHold = true;
            if (InteractHoldTime <= 0f) InteractHoldTime = 1f;
            CanTap = true;
            CanHold = true;
        }
    }
}