using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBaseState : IState
{
    protected PlayerStateMachine stateMachine;
    protected readonly PlayerGroundData groundData;

    private float _curHeight;
    private float verticalVelocity;
    private Vector3 Movement => Vector3.up * verticalVelocity;

    public PlayerBaseState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        groundData = stateMachine.Player.Data.GroundData;
    }

    public virtual void Enter()
    {
        AddInputActionsCallbacks();
        _curHeight = stateMachine.Player.FPView.localPosition.y;
    }

    /// <summary>
    /// 모든 Player 입력 액션에 콜백을 등록.
    /// Enter() 시 호출되고, Exit() 시 RemoveInputActionsCallbacks()로 해제.
    ///
    /// ── 투척 관련 입력 3종 ──
    ///   Interaction (.started)  → E키        → OnInteractionStarted → 줍기
    ///   Throw       (.started)  → 좌클릭      → OnThrowStarted      → 던지기
    ///   TimerAdjust (.performed) → 마우스 스크롤 → OnTimerAdjust       → 딜레이 조절
    ///
    /// 왜 TimerAdjust만 .performed인가:
    ///   Button 타입(.started): 키를 "누르는 순간" 1회 발생 → E키, 좌클릭에 적합.
    ///   Value 타입(.performed): 값이 "변경될 때마다" 발생 → 스크롤 휠에 적합.
    ///   스크롤 휠은 연속적인 Value 변화이므로 .started는 최초 1회만 발생하고,
    ///   .performed는 매 스크롤 틱마다 발생해서 매번 ±1초 조절이 가능.
    /// </summary>
    protected virtual void AddInputActionsCallbacks()
    {
        stateMachine.Player.Input.Player.Movement.started += OnMovementStarted;
        stateMachine.Player.Input.Player.Movement.canceled += OnMovementCanceled;
        stateMachine.Player.Input.Player.Run.started += OnRunStarted;
        stateMachine.Player.Input.Player.Run.canceled += OnRunCanceled;
        stateMachine.Player.Input.Player.Crouch.started += OnCrouchStarted;
        stateMachine.Player.Input.Player.Crouch.canceled += OnCrouchCanceled;
        stateMachine.Player.Input.Player.Interaction.started += OnInteractionStarted;
        stateMachine.Player.Input.Player.Interaction.canceled += OnInteractionCanceled;
        stateMachine.Player.Input.Player.Flash.started += OnFlashStarted;

        // 좌클릭(Mouse/leftButton) → Throw 액션 → OnThrowStarted
        // Button 타입이므로 .started (클릭 순간 1회 발생)
        stateMachine.Player.Input.Player.Throw.started += OnThrowStarted;

        // 마우스 스크롤(Mouse/scroll/y) → TimerAdjust 액션 → OnTimerAdjust
        // Value(Axis) 타입이므로 .performed (스크롤 값이 변경될 때마다 발생)
        // 스크롤 업: +값, 스크롤 다운: -값 → OnTimerAdjust에서 부호만 판별해서 ±1초 조절
        stateMachine.Player.Input.Player.TimerAdjust.performed += OnTimerAdjust;
        //stateMachine.Player.Input.Player.TimeStop.started += OnTimeStopStarted;
    }

    //private void OnTimeStopStarted(InputAction.CallbackContext context)
    //{
    //    if (stateMachine.Player.PlayerCooldownUI.IsCooling 
    //        || GameManager.Instance.IsGameover 
    //        || !ScenarioManager.Instance.GetAchieve(ScenarioAchieve.GasMask)) return;
    //    //Debug.Log("PlayerBaseState - OnTimeStopStarted()");
    //    CheckTimeOut();
    //}


    protected virtual void OnFlashStarted(InputAction.CallbackContext context)
    {
        if (!ScenarioManager.Instance.GetAchieve(ScenarioAchieve.PlayerFlashLight))
        {
            return;
        }

        if (stateMachine.Player.PlayerUIBlinker.IsBlinking)
        {
            stateMachine.Player.PlayerUIBlinker.StopBlink();
        }

        stateMachine.Player.SetFlash();
    }

    /// <summary>
    /// 좌클릭 시 호출됨 (Player.Throw 액션 → Mouse/leftButton).
    ///
    /// 동작:
    ///   1) PlayerInteractable에서 현재 들고 있는 오브젝트(HeldInteractable)를 가져옴
    ///   2) IThrowable 인터페이스로 캐스팅 시도
    ///   3) 성공하면 ThrowObject() 호출 → ThrowableBase.ThrowObject()가 실행됨
    ///
    /// 왜 IThrowable로 캐스팅하는가:
    ///   HeldInteractable의 타입은 InteractableBase (GlassBottle/TimerLure/BrokenTimer 등).
    ///   구체 타입을 몰라도 IThrowable만 확인하면 던지기 가능 여부를 판단할 수 있음.
    ///   향후 투척 불가 오브젝트(예: 열쇠)를 들고 있을 수도 있으므로 is 체크 필수.
    ///
    /// UI 맵과 충돌 안 하는 이유:
    ///   UI 맵의 Click도 좌클릭이지만, Player 맵과 UI 맵은
    ///   InputController.PlayerInputSwitch/UIInputSwitch로 상호배타적으로 활성화됨.
    /// </summary>
    protected virtual void OnThrowStarted(InputAction.CallbackContext context)
    {
        var held = stateMachine.Player.PlayerInteractable.HeldInteractable;
        if (held is IThrowable throwable)
            throwable.ThrowObject();
    }

    /// <summary>
    /// 마우스 스크롤 시 호출됨 (Player.TimerAdjust 액션 → Mouse/scroll/y).
    ///
    /// ── 동작 흐름 ──
    ///   1) PlayerInteractable.HeldInteractable로 현재 들고 있는 오브젝트 가져옴
    ///   2) ITimerAdjustable로 캐스팅 시도 (is 패턴)
    ///   3) 성공하면 스크롤 방향에 따라 AdjustDelay(+1 or -1) 호출
    ///
    /// ── IThrowable과 같은 패턴 ──
    ///   OnThrowStarted:  held is IThrowable     → ThrowObject()   (모든 투척체)
    ///   OnTimerAdjust:   held is ITimerAdjustable → AdjustDelay()  (TimerLure만)
    ///   둘 다 인터페이스 캐스팅으로 구체 타입을 몰라도 동작함.
    ///
    /// ── 3종 투척체별 반응 ──
    ///   TimerLure:       ITimerAdjustable 구현 O → 스크롤로 딜레이 조절 가능
    ///   GlassBottle:     ITimerAdjustable 구현 X → 캐스팅 실패 → 아무 일 없음
    ///   BrokenTimerLure: ITimerAdjustable 구현 X → 캐스팅 실패 → 아무 일 없음
    ///   기타(열쇠 등):   ITimerAdjustable 구현 X → 캐스팅 실패 → 아무 일 없음
    ///
    /// ── 스크롤 값 처리 ──
    ///   Windows에서 scroll/y는 보통 ±120 단위로 전달됨.
    ///   하지만 크기는 무시하고 부호(>0 / <0)만 사용 → 항상 정확히 1초씩 변경.
    ///   이유: 마우스마다 스크롤 감도가 다를 수 있으므로, 절대값 사용은 위험.
    ///         부호만 보면 어떤 마우스든 "한 칸 = 1초" 보장.
    /// </summary>
    protected virtual void OnTimerAdjust(InputAction.CallbackContext context)
    {
        // 현재 들고 있는 오브젝트 가져오기
        var held = stateMachine.Player.PlayerInteractable.HeldInteractable;

        // ITimerAdjustable 캐스팅: TimerLure만 성공, 나머지는 무시됨
        if (held is ITimerAdjustable adjustable)
        {
            // 스크롤 방향 읽기: 양수=위, 음수=아래
            float scrollValue = context.ReadValue<float>();

            // 부호만 판별 → ±1초씩 조절 (스크롤 감도 무관)
            if (scrollValue > 0f)
                adjustable.AdjustDelay(1f);   // 스크롤 업 → 딜레이 +1초
            else if (scrollValue < 0f)
                adjustable.AdjustDelay(-1f);  // 스크롤 다운 → 딜레이 -1초
        }
    }

    /// <summary>
    /// E키 시 호출됨 (Player.Interaction 액션).
    ///
    /// 변경 전: HeldInteractable이 있으면 Interact() 호출 → E키로 던지기도 가능했음
    /// 변경 후: HeldInteractable이 있으면 early return → E키로는 던지기 불가
    ///   이미 오브젝트를 들고 있는 상태에서 E키를 누르면 아무 일도 안 함.
    ///   던지기는 오직 좌클릭(OnThrowStarted)으로만 가능.
    ///
    /// 이유: E키 토글(줍기/던지기 반복)은 실수로 던지기 쉬움.
    ///   좌클릭 분리로 "줍기"와 "던지기"를 명확히 구분.
    /// </summary>
    protected virtual void OnInteractionStarted(InputAction.CallbackContext context)
    {
        // 이미 들고 있으면 E키 무시 — 던지기는 좌클릭으로만
        if (stateMachine.Player.PlayerInteractable.HeldInteractable != null)
            return;
        if (stateMachine.Player.PlayerInteractable.CurInteractable == null || stateMachine.Player.PlayerInteractable.isCarry) return;

        if (!stateMachine.Player.PlayerInteractable.CurInteractable.IsTabAndHold && stateMachine.Player.PlayerInteractable.CurInteractable.IsInteractable)
        {
            stateMachine.Player.PlayerInteractable.OnInteracted();
        }
        else if (stateMachine.Player.PlayerInteractable.CurInteractable.IsTabAndHold)
        {
            stateMachine.InteractHoldTime = 0f;
            stateMachine.InteractHoldTime = Time.time;
            stateMachine.IsInteract = true;
        }
    }

    protected virtual void OnInteractionCanceled(InputAction.CallbackContext context)
    {
        if (stateMachine.IsInteract)
        {
            float holdDuration = Time.time - stateMachine.InteractHoldTime;
            stateMachine.IsInteract = false;

            if (holdDuration < 0.2f)
            {
                stateMachine.Player.PlayerInteractable.OnInteracted();   
            }

            stateMachine.InteractHoldTime = 0f;
        }

        stateMachine.Player.PlayerInteractable.OffInteracted();
    }

    protected virtual void OnMovementStarted(InputAction.CallbackContext context)
    {
        stateMachine.Player.CurState |= PlayerEnum.PlayerState.Move;
    }

    protected virtual void OnMovementCanceled(InputAction.CallbackContext context)
    {
        stateMachine.Player.CurState &= ~PlayerEnum.PlayerState.Move;
        if (stateMachine.Player.CurState.HasFlag(PlayerEnum.PlayerState.Flash)) stateMachine.Player.Flash.FlashAnimationOff();
    }

    protected virtual void OnRunStarted(InputAction.CallbackContext context)
    {
    }

    protected virtual void OnRunCanceled(InputAction.CallbackContext context)
    {
    }

    protected virtual void OnCrouchStarted(InputAction.CallbackContext context)
    {
    }

    protected virtual void OnCrouchCanceled(InputAction.CallbackContext context)
    {
    }

    public virtual void Exit()
    {
        RemoveInputActionsCallbacks();
    }

    /// <summary>
    /// AddInputActionsCallbacks()에서 등록한 콜백을 전부 해제.
    /// Exit() 시 호출됨.
    ///
    /// 해제하지 않으면: 상태 전환 후에도 이전 상태의 콜백이 남아서
    /// 동일 입력에 두 번 반응하는 버그가 발생함.
    ///
    /// 반드시 Add에서 등록한 것과 동일한 이벤트+핸들러 조합으로 -= 해야 함.
    /// TimerAdjust도 .performed로 등록했으므로 .performed로 해제.
    /// </summary>
    protected virtual void RemoveInputActionsCallbacks()
    {
        stateMachine.Player.Input.Player.Movement.started -= OnMovementStarted;
        stateMachine.Player.Input.Player.Movement.canceled -= OnMovementCanceled;
        stateMachine.Player.Input.Player.Run.started -= OnRunStarted;
        stateMachine.Player.Input.Player.Run.canceled -= OnRunCanceled;
        stateMachine.Player.Input.Player.Crouch.started -= OnCrouchStarted;
        stateMachine.Player.Input.Player.Crouch.canceled -= OnCrouchCanceled;
        stateMachine.Player.Input.Player.Interaction.started -= OnInteractionStarted;
        stateMachine.Player.Input.Player.Interaction.canceled -= OnInteractionCanceled;
        stateMachine.Player.Input.Player.Flash.started -= OnFlashStarted;
        stateMachine.Player.Input.Player.Throw.started -= OnThrowStarted;
        stateMachine.Player.Input.Player.TimerAdjust.performed -= OnTimerAdjust;
        //stateMachine.Player.Input.Player.TimeStop.started -= OnTimeStopStarted;
    }

    public virtual void HandleInput()
    {
    }

    public virtual void Update()
    {
        //if (stateMachine.IsTimeStop)
        //{

        //    float remainingTime = stateMachine.timeStopEndTime - Time.time;
        //    if (remainingTime <= 0f)
        //    {
        //        AudioListener.pause = false;
        //        //DOTween.PlayAll();
        //        //DOTweenAnimeManager.ResumeCapturedTweens();
        //        stateMachine.IsTimeStop = false;
        //        remainingTime = 0f;
        //        GameManager.Instance.IsTimeStop = false;
        //        //Debug.Log("PlayerBaseState - PhysicsUpdate() - TimeStop End");                
        //        stateMachine.Player.PlayerSkillEffect.EffectEnd();
        //        stateMachine.Player.PlayerSkillEffect.VolumeEffectToggle();
        //    }
        //}

        if (!stateMachine.Player.Input.Player.Movement.enabled) return;
        Move();
        ChangeSize(stateMachine.isAddSize);

        if (stateMachine.IsInteract)
        {
            if (stateMachine.Player.PlayerInteractable.CurInteractable == null || !stateMachine.Player.PlayerInteractable.CurInteractable.IsTabAndHold) return;

            float holdDuration = Time.time - stateMachine.InteractHoldTime;
            if (holdDuration >= 0.2f)
            {
                stateMachine.Player.PlayerInteractable.OnHoldInteracted();
            }
        }

        if (stateMachine.Player.CurState.HasFlag(PlayerEnum.PlayerState.Flash)) FlashLight();
        CheckStuck();
    }

    public virtual void PhysicsUpdate()
    {
        if (verticalVelocity < 0f && stateMachine.Player.Controller.isGrounded)
        {
            verticalVelocity = Physics.gravity.y * Time.deltaTime;
        }
        else
        {
            verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }
    }

    private void CheckStuck()
    {
        if (stateMachine.Player.WithNpc == null) return;
        Vector2 inputDir = stateMachine.Player.Input.Player.Movement.ReadValue<Vector2>();
        stateMachine.Player.StuckDetector.Update(inputDir, stateMachine.Player.WithNpc.transform);
    }

    private void ChangeSize(bool add)
    {
        if (!stateMachine.isSizeChage) return;

        if (add)
        {
            _curHeight += Time.deltaTime * 20f;
            if (_curHeight >= stateMachine.OriginHeight)
            {
                stateMachine.isSizeChage = false;
                _curHeight = stateMachine.OriginHeight;
            }
            stateMachine.Player.FPView.localPosition = new Vector3(stateMachine.Player.FPView.localPosition.x, _curHeight, stateMachine.Player.FPView.localPosition.z);

        }
        else
        {
            _curHeight -= Time.deltaTime * 20f;
            if (_curHeight <= stateMachine.CrouchHeight)
            {
                stateMachine.isSizeChage = false;
                _curHeight = stateMachine.CrouchHeight;
            }
            stateMachine.Player.FPView.localPosition = new Vector3(stateMachine.Player.FPView.localPosition.x, _curHeight, stateMachine.Player.FPView.localPosition.z);
        }
    }

    private void Move()
    {
        Vector3 movementDirection = GetMovementDirection();
        Move(movementDirection);
    }

    private Vector3 GetMovementDirection()
    {
        Vector2 movementInput = stateMachine.Player.Input.Player.Movement.ReadValue<Vector2>();
        //Debug.Log($"PlayerBaseState - GetMovementDirection() - movementInput : {movementInput}");
        Vector3 forward = stateMachine.Player.transform.forward;
        Vector3 right = stateMachine.Player.transform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        return forward * movementInput.y + right * movementInput.x;
    }

    private void Move(Vector3 direction)
    {
        float movementSpeed = GetMovementSpeed();

        //stateMachine.Player.Controller.Move
        //    (
        //        ((direction * movementSpeed) + (Movement * stateMachine.Player.Data.GroundData.GravityModifier)) * Time.deltaTime
        //    );

        Vector3 inputMove = direction * movementSpeed;
        Vector3 gravityMove = Movement * stateMachine.Player.Data.GroundData.GravityModifier;
        Vector3 fanMove = stateMachine.Player.FanForce;  //FanTarget

        Vector3 finalVelocity = inputMove + gravityMove + fanMove;

        stateMachine.Player.Controller.Move(finalVelocity * Time.deltaTime);
    }

    private float GetMovementSpeed()
    {
        float movementSpeed = stateMachine.MovementSpeed * stateMachine.MovementSpeedModifier;
        return movementSpeed;
    }

    private void FlashLight()
    {
        //if (!stateMachine.Player.IsDetectFlash) return;
        if (!stateMachine.Player.CurState.HasFlag(PlayerEnum.PlayerState.Flash)) return;
        //Debug.DrawRay(stateMachine.Player.FlashTransform.position, stateMachine.Player.FlashTransform.forward * 10f, Color.green);
        // 3.5 - 2.8 = 0.7
        float range = stateMachine.Player.FlashRange + 0.7f;

        if (Physics.Raycast(stateMachine.Player.FlashTransform.position, stateMachine.Player.FlashTransform.forward, out RaycastHit hit, range, stateMachine.Player.flashObstacle))
        {
            //Debug.Log($"{hit.distance}");

            if (hit.collider.gameObject.layer == 8)
            {
                //stateMachine.Player.FlashCollider.SetActive(false);
                EyeTypeMonsterV2 monster = null;
                if (hit.collider.gameObject.TryGetComponent<EyeTypeMonsterV2>(out monster))
                {
                    monster.LookAtPlayer();             
                    return;
                }
            }

            //// min
            //if (hit.distance <= 1.6f)
            //{
            //    //stateMachine.Player.FlashCollider.layer = 0;
            //    stateMachine.Player.FlashCollider.SetActive(false);
            //    return;
            //}
            //else
            //{
            //    //stateMachine.Player.FlashCollider.layer = 18;
            //    stateMachine.Player.FlashCollider.SetActive(true);
            //}
            //float positionZ = Mathf.Lerp(1f, stateMachine.Player.FlashRange, Mathf.InverseLerp(1.6f, range, hit.distance));
            //Vector3 pos = stateMachine.Player.FlashCollider.transform.localPosition;
            //pos.z = positionZ;
            //stateMachine.Player.FlashCollider.transform.localPosition = pos;
        }
        //else
        //{
        //    stateMachine.Player.FlashCollider.layer = 18;
        //    stateMachine.Player.FlashCollider.SetActive(true);
        //    Vector3 pos = stateMachine.Player.FlashCollider.transform.localPosition;
        //    pos.z = stateMachine.Player.FlashRange;
        //    stateMachine.Player.FlashCollider.transform.localPosition = pos;
        //}
    }

    //private void TimeStop()
    //{        
    //    AudioListener.pause = true;
    //    //DOTween.PauseAll();
    //    //DOTweenAnimeManager.PauseRunningTweens();
    //    stateMachine.Player.PlayerCooldownUI.StartCooldown(stateMachine.Player.Data.GroundData.StopCoolTime);
    //    stateMachine.IsTimeStop = true;
    //    stateMachine.timeStopEndTime = Time.time + stateMachine.Player.Data.GroundData.StopDuration;
    //    GameManager.Instance.IsTimeStop = true;
    //}

    //private Sequence _timeOut;
    //private void CheckTimeOut()
    //{
    //    if (stateMachine.Player.PlayerSkillEffect.IsPlaying) return;
        
    //    _timeOut?.Kill();
    //    _timeOut = DOTween.Sequence();
    //    _timeOut.AppendCallback(() => stateMachine.Player.PlayerSkillEffect.EffectStart());
    //    _timeOut.AppendInterval(0f);
    //    _timeOut.AppendCallback(TimeStop)
    //        .SetAutoKill(true).SetTarget(this)            
    //        //.SetUpdate(true)          
    //        .OnKill(() => _timeOut = null);
    //    _timeOut.Play();
    //}
}
    