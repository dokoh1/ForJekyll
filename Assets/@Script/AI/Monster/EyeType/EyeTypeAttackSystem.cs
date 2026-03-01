using MBT;
using System.Collections;
using UnityEngine;

public class EyeTypeAttackSystem : MonoBehaviour
{
    public Blackboard bb;
    public IAttackable cachedPlayer;
    public IAttackable cachedNPC;

    private IAttackable _target;
    private Coroutine _npcAttackCorutine;
    public bool reWork = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 0)
        {
            //Debug.Log($"EyeTypeAttackSystem - OnTriggerEnter() - layer : {other.gameObject.layer}, {other.name}");
            return;
        }

        if (bb.GetVariable<Variable<int>>("curState").Value != (int)MonsterState.Attack) return;
        reWork = true;

        if (other.gameObject.layer == LayerMask.NameToLayer("NPC")) 
        {
            //Debug.Log($"hit npc");
            reWork = true;
            StartNpcAttack(other);
            return;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("ImportantNPC"))
        {
            //Debug.Log($"hit i npc");
            reWork = false;
            StartNpcAttack(other);
            return;
        }
    }

    private IEnumerator NpcAttack(Collider other)
    {
        //Debug.Log($"EyeTypeAttackSystem - NpcAttack() - other : {other.name}, layer : {other.gameObject.layer}, reWork : {reWork}");

        if (cachedNPC != null)
        {
            Attack(cachedNPC);
        }

        if (cachedNPC == null && other.TryGetComponent<IAttackable>(out var attackableTarget))
        {
            _target = attackableTarget;
            Attack(_target);
        }

        yield return null;
        //Debug.Log($"EyeTypeAttackSystem - yeild return 후, other : {other.name}, layer : {other.gameObject.layer}, reWork : {reWork}");
        if (other.gameObject.layer == 0) reWork = true;

        //Debug.Log("EyeTypeAttackSystem - OnTriggerEnter");
        if (reWork)
        {
            //Debug.Log($"EyeTypeAttackSystem - OnTriggerEnter() - reWork : {reWork}, 공격후 타겟 초기화");
            bb.GetVariable<Variable<bool>>("isWork").Value = true;
            bb.GetVariable<Variable<bool>>("isAttacking").Value = false;
            bb.GetVariable<Variable<Transform>>("target").Value = null;
            bb.GetVariable<Variable<bool>>("isDetect").Value = false;
            bb.GetVariable<Variable<bool>>("isLost").Value = true;
        }
    }

    private void StartNpcAttack(Collider other)
    {
        StopNpcAttack();
        _npcAttackCorutine = StartCoroutine(NpcAttack(other));
    }

    private void StopNpcAttack()
    {
        if (_npcAttackCorutine != null) StopCoroutine(_npcAttackCorutine);
    }

    public void Attack()
    {
        _target?.OnHitSuccess();
    }

    public void Attack(IAttackable target)
    {
        target?.OnHitSuccess();
    }
}
