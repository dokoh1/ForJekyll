public class PlayerStateMachine : StateMachine
{
    public Player Player { get; }
    
    // Player State
    public PlayerIdleState IdleState { get; private set; }
    public PlayerWalkState WalkState { get; private set; }
    public PlayerRunState RunState { get; private set; }
    public PlayerCrouchState CrouchState { get; private set; }

    public float MovementSpeed { get; private set; }
    public float MovementSpeedModifier { get; set; } = 1f;
    public float OriginHeight { get; private set; }
    public float CrouchHeight { get; private set; }
    public bool IsInteract { get; set; } = false;
    public float InteractHoldTime { get; set; } = 0f;
    public bool IsTimeStop { get; set; } = false;

    public float curHeight;
    public bool isSizeChage;
    public bool isAddSize;

    public float timeStopEndTime;

    public PlayerStateMachine(Player player)
    {
        Player = player;
        IdleState = new PlayerIdleState(this);
        WalkState = new PlayerWalkState(this);
        RunState = new PlayerRunState(this);
        CrouchState = new PlayerCrouchState(this);
        MovementSpeed = player.Data.GroundData.BaseSpeed;
        OriginHeight = player.FPView.localPosition.y;
        CrouchHeight = OriginHeight * player.Data.GroundData.CrouchHeightModifier;
    }

    public override void ChangeState(IState state)
    {
        if (Player == null) return;

        base.ChangeState(state);
    }
}