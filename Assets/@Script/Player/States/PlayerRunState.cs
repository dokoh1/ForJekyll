using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRunState : PlayerGroundState
{
    public PlayerRunState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        //Debug.Log("PlayerRunState - Enter()");
        stateMachine.Player.CurState |= PlayerEnum.PlayerState.Run;
        stateMachine.MovementSpeedModifier = stateMachine.Player.Data.GroundData.RunSpeedModifier;
        //Debug.Log($"PlayerRunState - Enter() - MovementSpeedModifier : {stateMachine.MovementSpeedModifier}");
        if (NoiseDetectionGaugeManager.Instance != null)
        {
            NoiseDetectionGaugeManager.Instance.SetPlayerNoiseState(NoiseDetectionGaugeManager.PlayerNoiseState.Run);
        }
        stateMachine.Player.SumNoiseAmount = 12f;
        stateMachine.Player.CurNoiseAmount = 9.5f;
        stateMachine.Player.NoiseCheckAmount = 20f;
    }

    public override void Exit()
    {
        base.Exit();
        stateMachine.Player.CurState &= ~PlayerEnum.PlayerState.Run;
        if (NoiseDetectionGaugeManager.Instance != null)
        {
            NoiseDetectionGaugeManager.Instance.SetPlayerNoiseState(NoiseDetectionGaugeManager.PlayerNoiseState.IdleOrCrouch);
        }
        stateMachine.Player.CurNoiseAmount = 0f;
        stateMachine.Player.NoiseCheckAmount = 0f;
        //Debug.Log("PlayerRunState - Exit()");
    }

    public override void Update()
    {
        base.Update();
        if (stateMachine.Player.CurState.HasFlag(PlayerEnum.PlayerState.Move))
        {
            stateMachine.Player.PlayerSoundSystem.PlayStepSound(stateMachine.Player.CurState);
            if (!stateMachine.Player.CurState.HasFlag(PlayerEnum.PlayerState.Flash)) return;
            if (!stateMachine.Player.Flash.Animator.GetBool(stateMachine.Player.Flash.AnimationData.RunParameterHash)) stateMachine.Player.Flash.FlashAnimationOn(PlayerEnum.PlayerState.Run);
        }
    }

    protected override void OnRunCanceled(InputAction.CallbackContext context)
    {
        base.OnRunCanceled(context);
        //Debug.Log("PlayerRunState - OnRunCanceled()");
        if (!stateMachine.Player.CurState.HasFlag(PlayerEnum.PlayerState.Move)) stateMachine.ChangeState(stateMachine.IdleState);
        else stateMachine.ChangeState(stateMachine.WalkState);
    }

    protected override void OnCrouchStarted(InputAction.CallbackContext context)
    {
        base.OnCrouchStarted(context);
        stateMachine.ChangeState(stateMachine.CrouchState);
    }
}
