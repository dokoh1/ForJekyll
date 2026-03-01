using UnityEditor;
using static InteractableBase;

public interface IInteractable
{
    InteractTypeEnum InteractType { get; set; }
    bool IsInteractable { get; set; } // 상호작용 가능 여부
    bool IsTabAndHold {get;set;}
    float InteractHoldTime { get; set; } // 0초 즉시 상호작용, 1초부터 홀드 상호작용
    bool CanTap { get; set; }
    bool CanHold { get; set; }

    void Interact(); // 상호작용할 오브젝트의 실행 내용
    void Hold();
}

