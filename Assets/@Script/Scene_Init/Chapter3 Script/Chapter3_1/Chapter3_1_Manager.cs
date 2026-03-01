using UnityEngine;
using NPC;

public class Chapter3_1_Manager : MonoBehaviour
{
    private readonly Chapter3_1_SceneInitializer _ch31;
    public Chapter3_1_Manager(Chapter3_1_SceneInitializer ch31) { _ch31 = ch31; }

    private Elevator_Obj[] elevator;
    public void ObjectInitialize(Elevator_Obj[] elevator, NpcUnit YHJ, NpcUnit KJM)
    {
        this.elevator = elevator;
    }
    public void ElevatorInteractOn()
    {
        UIManager.Instance.DialogueOpen(Dialogue.Main, false, 42);
        foreach (var elevator in elevator)
        {
            elevator.IsInteractable = true;
            elevator.sceneEnum = SceneEnum.Chapter3_2;
        }
    }

    public void ControllRoomOpenAfter()
    {
        UIManager.Instance.DialogueOpen(Dialogue.Main, true, 20);
    }
}
