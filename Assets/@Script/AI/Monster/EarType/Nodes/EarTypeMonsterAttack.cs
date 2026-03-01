using MBT;
using UnityEngine;

[AddComponentMenu("")]
[MBTNode("Example/EarType Monster Attack")]
public class EarTypeMonsterAttack : Leaf
{
    public IntReference curState;
    public BoolReference isWork;
    public BoolReference isNoiseDetect;
    public BoolReference isTargetPlayer;
    public BoolReference isCanAttack;
    public TransformReference player;
    public TransformReference npc;
    public Monster monster;

    public override void OnEnter()
    {
        if (!isWork.Value || !isCanAttack.Value) return;
        monster.agent.isStopped = true;
        curState.Value = (int)MonsterState.Attack;
        monster.SetAnimation(true, false, false, false);
        isWork.Value = false;
        isNoiseDetect.Value = false;
        isCanAttack.Value = false;

        if (isTargetPlayer.Value)
        {
            Debug.Log($"EarTypeMonsterAttack - OnEnter() - play - gameOver");
            GameManager.Instance.Player.OnHitSuccess(UnitEnum.UnitType.EarTypeMonster);
        }
        else
        {
            Debug.Log($"EarTypeMonsterAttack - OnEnter() - npc - gameOver");
            if (npc.Value.gameObject.TryGetComponent<IAttackable>(out IAttackable attackable)) attackable.OnHitSuccess(UnitEnum.UnitType.EarTypeMonster);
        }
    }

    public override NodeResult Execute()
    {
        return NodeResult.success;
    }
}
