using MBT;
using UnityEngine;

[AddComponentMenu("")]
[MBTNode("Example/Attack Check With Npc Service")]
public class AttackCheckWithNpcService : Service
{
    public TransformReference player;
    public TransformReference npc;
    public FloatReference attackRange;
    public BoolReference canAttack;
    public BoolReference isNoiseDetect;
    public BoolReference isWork;
    public BoolReference isTargetPlayer;

    private Transform _player;
    private Transform _npc;
    private float _attackRange;

    public override void OnEnter()
    {
        if (_attackRange == 0f) _attackRange = attackRange.Value;
        _player = player.Value;
        _npc = npc.Value;
        base.OnEnter();
    }

    public override void Task()
    {
        if (_attackRange >= Vector3.Distance(transform.position, _player.position))
        {
            canAttack.Value = true;
            isNoiseDetect.Value = false;
            isTargetPlayer.Value = true;
            return;
        }
        else canAttack.Value = false;

        if (_npc == null) return;
        if (_attackRange >= Vector3.Distance(transform.position, _npc.position))
        {
            canAttack.Value = true;
            isNoiseDetect.Value = false;
            isTargetPlayer.Value = false;
        }
        else canAttack.Value = false;
    }
}
