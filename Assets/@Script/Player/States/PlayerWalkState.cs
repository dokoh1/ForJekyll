using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWalkState : PlayerGroundState
{
    public PlayerWalkState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateMachine.Player.CurState |= PlayerEnum.PlayerState.Walk;
        stateMachine.MovementSpeedModifier = stateMachine.Player.Data.GroundData.WalkSpeedModifier;
        if (NoiseDetectionGaugeManager.Instance != null)
        {
            NoiseDetectionGaugeManager.Instance.SetPlayerNoiseState(NoiseDetectionGaugeManager.PlayerNoiseState.Walk);

        }
        stateMachine.Player.SumNoiseAmount = 6f;
        stateMachine.Player.CurNoiseAmount = 4f;
        stateMachine.Player.NoiseCheckAmount = 10f;
    }

    public override void Exit()
    {
        base.Exit();
        stateMachine.Player.CurState &= ~PlayerEnum.PlayerState.Walk;
        if (NoiseDetectionGaugeManager.Instance != null)
        {
            NoiseDetectionGaugeManager.Instance.SetPlayerNoiseState(NoiseDetectionGaugeManager.PlayerNoiseState.IdleOrCrouch);

        }
        stateMachine.Player.NoiseCheckAmount = 0f;
        stateMachine.Player.CurNoiseAmount = 0f;
    }

    public override void Update()
    {
        base.Update();
        if (stateMachine.Player.CurState.HasFlag(PlayerEnum.PlayerState.Move))
        {
            stateMachine.Player.PlayerSoundSystem.PlayStepSound(stateMachine.Player.CurState);
            if (!stateMachine.Player.CurState.HasFlag(PlayerEnum.PlayerState.Flash)) return;
            if (!stateMachine.Player.Flash.Animator.GetBool(stateMachine.Player.Flash.AnimationData.WalkParameterHash)) stateMachine.Player.Flash.FlashAnimationOn(PlayerEnum.PlayerState.Walk);
        }
    }

    protected override void OnMovementCanceled(InputAction.CallbackContext context)
    {
        base.OnMovementCanceled(context);
        stateMachine.ChangeState(stateMachine.IdleState);
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
