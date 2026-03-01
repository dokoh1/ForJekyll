using UnityEngine;

[CreateAssetMenu(fileName = "BossAnimationData", menuName = "SO/Unit/MidBoss/AnimationData")]
public class BossAnimationData : ScriptableObject
{
    private string _idleParameterName = "Idle";
    private string _detectParameterName = "Detect";
    private string _rageParameterName = "Rage";
    private string _chargeParameterName = "Charge";
    private string _stunParameterName = "Stun";

    public int IdleParameterHash { get; private set; }
    public int ChargeParameterHash { get; private set; }
    public int DetectParameterHash { get; private set; }
    public int RageParameterHash { get; private set; }
    public int StunParameterHash { get; private set; }

    public void Initialize()
    {
        IdleParameterHash = Animator.StringToHash(_idleParameterName);
        ChargeParameterHash = Animator.StringToHash(_chargeParameterName);
        DetectParameterHash = Animator.StringToHash(_detectParameterName);
        RageParameterHash = Animator.StringToHash(_rageParameterName);
        StunParameterHash = Animator.StringToHash(_stunParameterName);
    }
}
