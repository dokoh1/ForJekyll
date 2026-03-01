using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCrouchState : PlayerGroundState
{
    private bool _isCrouching;
    private bool _isRunning;
    private LayerMask _layerMask = LayerMask.GetMask("Obstacle", "HeadObstruct");
    private Vector3 _height;

    public PlayerCrouchState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        _isCrouching = true;
        _isRunning = false;
        //Debug.Log("PlayerCrouchState - Enter()");
        stateMachine.Player.CurState |= PlayerEnum.PlayerState.Crouch;
        stateMachine.Player.SumNoiseAmount = 6f;
        stateMachine.isAddSize = false;
        stateMachine.isSizeChage = true;
        //stateMachine.MovementSpeedModifier = groundData.CrouchSpeedModifier;
        stateMachine.MovementSpeedModifier = stateMachine.Player.CrouchSpeedModifier; //벤트씬때문에 수정
        stateMachine.Player.Controller.height = stateMachine.Player.Data.GroundData.Height * stateMachine.Player.Data.GroundData.CrouchHeightModifier;
        _height.y = stateMachine.Player.Controller.height * 0.5f;
        stateMachine.Player.Controller.center = _height;
    }

    public override void Exit()
    {
        base.Exit();
        //Debug.Log("PlayerCrouchState - Exit()");
        stateMachine.Player.CurState &= ~PlayerEnum.PlayerState.Crouch;
        stateMachine.isAddSize = true;
        stateMachine.isSizeChage = true;
        stateMachine.Player.Controller.height = stateMachine.Player.Data.GroundData.Height;
        _height.y = stateMachine.Player.Controller.height * 0.5f;
        stateMachine.Player.Controller.center = _height;
    }

    public override void Update()
    {
        base.Update();
        if (stateMachine.Player.CurState.HasFlag(PlayerEnum.PlayerState.Move))
        {
            stateMachine.Player.PlayerSoundSystem.PlayStepSound(stateMachine.Player.CurState);
            if (!stateMachine.Player.CurState.HasFlag(PlayerEnum.PlayerState.Flash)) return;
            if (!stateMachine.Player.Flash.Animator.GetBool(stateMachine.Player.Flash.AnimationData.CrouchParameterHash)) stateMachine.Player.Flash.FlashAnimationOn(PlayerEnum.PlayerState.Crouch);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        if (_isRunning)
        {
            if (!CheckHeadObstruct())
            {
                stateMachine.ChangeState(stateMachine.RunState);
                return;
            }
        }

        if (!_isCrouching)
        {
            if (CheckHeadObstruct()) return;

            if (stateMachine.Player.CurState.HasFlag(PlayerEnum.PlayerState.Move)) stateMachine.ChangeState(stateMachine.WalkState);
            else stateMachine.ChangeState(stateMachine.IdleState);
        }
    }

    protected override void OnCrouchStarted(InputAction.CallbackContext context)
    {
        base.OnCrouchStarted(context);
        _isCrouching = true;
        _isRunning = false;
    }

    protected override void OnRunStarted(InputAction.CallbackContext context)
    {
        _isRunning = true;
        if (CheckHeadObstruct()) return;
        base.OnRunStarted(context);
        stateMachine.ChangeState(stateMachine.RunState);
    }

    private bool CheckHeadObstruct()
    {
        if (!Physics.Raycast(stateMachine.Player.transform.position, Vector3.up, 2.5f, _layerMask)) return false;
        return true;
    }

    protected override void OnCrouchCanceled(InputAction.CallbackContext context)
    {
        base.OnCrouchCanceled(context);
        _isCrouching = false;
        if (CheckHeadObstruct()) return;
        if (!stateMachine.Player.CurState.HasFlag(PlayerEnum.PlayerState.Move)) stateMachine.ChangeState(stateMachine.IdleState);
        else stateMachine.ChangeState(stateMachine.WalkState);
    }

    protected override void OnRunCanceled(InputAction.CallbackContext context)
    {
        base.OnRunCanceled(context);
        _isRunning = false;
    }
}
