/// <summary>
/// 타이머 딜레이를 플레이어가 조절할 수 있는 투척체 인터페이스.
///
/// ── 왜 만들었는가 ──
///   투척체 3종 중 TimerLure만 "던진 후 n초 뒤 소음" 메커닉이 있으므로
///   딜레이 조절 기능도 TimerLure에만 해당됨.
///   PlayerBaseState에서 구체 타입(TimerLureThrowable)을 직접 참조하면
///   결합도가 높아지므로, 인터페이스로 분리해서 캐스팅으로 판별.
///
/// ── 왜 IThrowable과 별도 인터페이스인가 ──
///   IThrowable: "던질 수 있는가?" → 3종 모두 구현
///   ITimerAdjustable: "딜레이를 조절할 수 있는가?" → TimerLure만 구현
///
///   만약 IThrowable에 AdjustDelay()를 넣으면:
///     - GlassBottle, BrokenTimer는 빈 구현(throw NotImplementedException 또는 무시)을 강제당함
///     - 인터페이스 분리 원칙(ISP) 위반
///
///   별도 인터페이스이므로:
///     - TimerLure만 구현, 나머지는 아예 구현하지 않음
///     - PlayerBaseState에서 "is ITimerAdjustable" 체크 → 실패하면 그냥 무시
///
/// ── 3종 투척체별 대응 ──
///   GlassBottle:     IThrowable O, ITimerAdjustable X → 스크롤 무반응
///   TimerLure:       IThrowable O, ITimerAdjustable O → 스크롤로 ±1초 조절
///   BrokenTimerLure: IThrowable O, ITimerAdjustable X → 스크롤 무반응
///
/// ── 호출 흐름 ──
///   [입력] 마우스 스크롤 (Mouse/scroll/y)
///   → [Input System] Player 맵의 "TimerAdjust" 액션 (Value, Axis)
///   → [콜백] PlayerBaseState.OnTimerAdjust(context)
///     → context.ReadValue&lt;float&gt;()로 스크롤 방향 읽기
///     → HeldInteractable을 ITimerAdjustable로 캐스팅
///     → 성공: AdjustDelay(+1f) 또는 AdjustDelay(-1f) 호출
///     → 실패(GlassBottle/BrokenTimer): 아무 일 없음
///   → [TimerLure] AdjustDelay()에서 _adjustedDelay를 Clamp(1~10)하고 텍스트 갱신
/// </summary>
public interface ITimerAdjustable
{
    /// <summary>
    /// 타이머 딜레이를 amount만큼 변경.
    ///
    /// amount > 0: 딜레이 증가 (스크롤 업 → 소음까지 더 오래 걸림)
    /// amount &lt; 0: 딜레이 감소 (스크롤 다운 → 소음이 더 빨리 시작됨)
    ///
    /// 구현체(TimerLureThrowable)에서 내부적으로 min/max 클램프 처리.
    /// 현재 범위: 1초 ~ 10초.
    ///
    /// 안전 조건 (구현체 내부):
    ///   - _isHeld == true (들고 있을 때만 조절 가능)
    ///   - _isArmed == false (이미 던져서 타이머 작동 중이면 조절 불가)
    /// </summary>
    void AdjustDelay(float amount);
}
