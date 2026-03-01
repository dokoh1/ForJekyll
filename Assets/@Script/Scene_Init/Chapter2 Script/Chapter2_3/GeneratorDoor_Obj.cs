using System;
using System.Linq.Expressions;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public class GeneratorDoor_Obj : StoryLockDoor_Obj
{
    [SerializeField] private GameObject invisibleWall;
    [SerializeField] private bool IsChapter3_2Door;
    public static bool blockedTried = false;

    private static bool storyPlayed = false;
    public event Action OnDoorOpened;
    public event Action OnNotKeyDoor;

    public bool skipStoryLock = false;

    public override void Interact()
    {
        if (GameManager.Instance.IsTimeStop) return;

        blockedTried = true;

        if (invisibleWall != null)
        {
            invisibleWall.SetActive(true);
        }

        if (storyPlayed)
            storyLock = false;

        if (storyLock && !skipStoryLock)
        {
            DoorStoryLock(storyIndex);
            if (IsChapter3_2Door)
                OnNotKeyDoor?.Invoke();
            storyPlayed = true;
            return;
        }

        if (!canOpen)
            DoorLock();
        else 
        { 
            DoorOpen();
            IsInteractable = false;
            OnDoorOpened?.Invoke();
        }
    }

    public void CloseDoor()
    {
        doorOpen.DOPlayBackwards();

        canOpen = false;
        IsInteractable = false;

        if (navMeshObstacle != null)
            navMeshObstacle.enabled = true;

        GetComponent<BoxCollider>().enabled = true;
    }


}
