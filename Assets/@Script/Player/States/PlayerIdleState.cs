using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerIdleState : PlayerGroundState
{
    public PlayerIdleState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateMachine.Player.CurState |= PlayerEnum.PlayerState.Idle;
        //Debug.Log("PlayerIdleState - Enter()");
    }

    public override void Exit()
    {
        base.Exit();
        stateMachine.Player.CurState &= ~PlayerEnum.PlayerState.Idle;
    }

    protected override void OnMovementStarted(InputAction.CallbackContext context)
    {
        base.OnMovementStarted(context);

        if (stateMachine.Player.CurState.HasFlag(PlayerEnum.PlayerState.Move)) stateMachine.ChangeState(stateMachine.WalkState);
    }

    protected override void OnCrouchStarted(InputAction.CallbackContext context)
    {
        base.OnCrouchStarted(context);
        stateMachine.ChangeState(stateMachine.CrouchState);
    }

    protected override void OnRunStarted(InputAction.CallbackContext context)
    {
        base.OnRunStarted(context);
        stateMachine.ChangeState(stateMachine.RunState);
    }
}
