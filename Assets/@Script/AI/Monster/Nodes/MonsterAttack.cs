using MBT;
using NPC;
using System;
using UnityEngine;
using static UnitEnum;

[AddComponentMenu("")]
[MBTNode("Example/Monster Attack")]
public class MonsterAttack : Leaf
{
    public BoolReference isAttacking;
    public BoolReference isDetect;
    public BoolReference isWork;
    public BoolReference isLost;
    public IntReference curState;
    public TransformReference target;
    public TransformReference self;

    public Monster monster;
    public Npc npcCache;

    private AnimatorStateInfo stateInfo;
    private bool _isNPC;
    private bool _isGameover;
    private bool _isRework;
    private bool _isFlash;

    public override void OnEnter()
    {
        Debug.Log($"MonsterAttack OnEnter");
        _isFlash = false;
        _isRework = false;
        _isGameover = false;
        _isNPC = false;
        isWork.Value = false;

        // monster.Sound.StopStepAudio();
        monster.agent.isStopped = true;
        curState.Value = (int)MonsterState.Attack;

        if (target.Value == null)
        {
            Debug.LogWarning("MonsterAttack: target is null!");
            EndAttack(true);
            return;
        }

        int layer = target.Value.gameObject.layer;

        if (layer == LayerMask.NameToLayer("Player"))
        {
            //isWork.Value = false;
            _isGameover = true;
            monster.SetAnimation(true, false, false, false, false);
            GameManager.Instance.Player.OnHitSuccess(UnitType.EyeTypeMonster);
        }
        else if (layer == LayerMask.NameToLayer("NPC"))
        {
            //Debug.Log($"MonsterAttack - hit NPC");
            _isRework = true;
            _isNPC = true;

            HandleNPCAttack();
        }
        else if (target.Value.gameObject.layer == LayerMask.NameToLayer("ImportantNPC"))
        {
            //Debug.Log($"MonsterAttack - hit ImportantNPC");

            HandleNPCAttack();
        }
        else if (target.Value.gameObject.layer == 18)
        {
            //Debug.LogWarning($"MonsterAttack: hit flash collider, layer: {LayerMask.LayerToName(layer)}, {target.Value.name}");
            EndAttack(true);
            _isFlash = true;
        }
        else if (target.Value.name == "FlashCollider")
        {
            //Debug.LogWarning($"MonsterAttack: hit flash collider by name, layer: {LayerMask.LayerToName(layer)}, {target.Value.name}");
            EndAttack(true);
            _isFlash = true;
        }
        else
        {
            Debug.LogWarning($"MonsterAttack: Unknown target layer {LayerMask.LayerToName(layer)}, {layer}, {target.Value.name}");
            EndAttack(true);
        }
    }

    private void EndAttack(bool isRework)
    {
        if (isRework)
        {
            isDetect.Value = false;
            isLost.Value = true;
            isWork.Value = true;
            isAttacking.Value = false;
            target.Value = null;
            _isRework = true;
        }
        else
        {
            isWork.Value = false;
            _isGameover = true;
        }
    }

    private void HandleNPCAttack()
    {
        self.Value.LookAt(target.Value);

        isAttacking.Value = true;
        monster.SetAnimation(false, false, false, true, false);

        if (npcCache == null)
        {
            target.Value.gameObject.TryGetComponent(out npcCache);
        }

        npcCache?.NpcStop();
        npcCache?.OnHitSuccess();        
    }


    public override NodeResult Execute()
    {
        if (_isGameover) return NodeResult.success;

        if (_isNPC)
        {
            stateInfo = monster.animator.GetCurrentAnimatorStateInfo(0);
            //Debug.Log($"MonsterAttack - Execute() - {stateInfo.IsName("Attack")}, {stateInfo.normalizedTime}");

            if (stateInfo.IsName("Attack") && stateInfo.normalizedTime >= 1.0f)
            {
                //Debug.Log($"MonsterAttack - Execute() - attack end");

                EndAttack(true);
                npcCache = null;
                return NodeResult.success;
            }
        }

        return NodeResult.running;
    }

    public override void OnExit()
    {
        base.OnExit();
        //Debug.Log($"MonsterAttack OnExit - _isRework : {_isRework}");

        EndAttack(_isRework);

        if (_isFlash)
        {
            isLost.Value = false;
        }
    }
}
