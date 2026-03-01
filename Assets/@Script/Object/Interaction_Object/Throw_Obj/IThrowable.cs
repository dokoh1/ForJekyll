/// <summary>
/// 투척 가능한 오브젝트가 구현하는 인터페이스.
///
/// 왜 IInteractable.Interact()와 분리했는가:
///   기존에는 E키 하나로 "줍기 + 던지기"를 토글했지만,
///   기획 변경으로 "E키 = 줍기", "좌클릭 = 던지기"로 입력이 분리됨.
///   Interact()는 E키(줍기)에 연결되어 있으므로, 던지기는 별도 계약이 필요.
///
/// 호출 흐름:
///   좌클릭 → PlayerBaseState.OnThrowStarted()
///   → PlayerInteractable.HeldInteractable을 IThrowable로 캐스팅
///   → ThrowObject() 호출
///
/// 왜 인터페이스인가:
///   PlayerBaseState는 구체 타입(GlassBottle/TimerLure/BrokenTimer)을 몰라도 됨.
///   "is IThrowable" 한 번의 체크로 어떤 투척체든 동일하게 던질 수 있음.
/// </summary>
public interface IThrowable
{
    void ThrowObject();
}
