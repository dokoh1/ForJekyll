using MBT;
using NPC;
using UnityEngine;
using static UnitEnum;

[AddComponentMenu("")]
[MBTNode("Example/EyeType Attack")]
public class EyeTypeAttack : Leaf
{
    public BoolReference isDetect;
    public BoolReference isWork;
    public BoolReference isLost;
    public BoolReference canAttack;
    public BoolReference haveTarget;

    public IntReference curState;

    public TransformReference target;
    public TransformReference self;
    public TransformReference targetNpc;

    public MonsterV2 monster;
    public Npc npcCache;

    private AnimatorStateInfo stateInfo;

    private bool _isPlayer;
    private bool _isNPC;
    private bool _isRework;
    private bool _isImportantNpc;


    public override void OnEnter()
    {
        //Debug.Log($"EyeTypeAttack - OnEnter() - target : {target.Value.name}, layer : {target.Value.gameObject.layer}");

        if (target.Value == targetNpc.Value)
        {
            //Debug.LogWarning($"EyeTypeAttack - OnEnter() - target is same as targetNpc, rework");
            targetNpc.Value = null;
            haveTarget.Value = false;
        }

        isWork.Value = false;
        _isPlayer = false;
        _isNPC = false;
        _isRework = false;
        _isImportantNpc = false;

        monster.agent.isStopped = true;
        curState.Value = (int)MonsterState.Attack;


        int layer = target.Value.gameObject.layer;
        if (layer == LayerMask.NameToLayer("Player"))
        {
            //isWork.Value = false;
            _isPlayer = true;
            monster.SetAnimation(true, false, false, false, false, false);
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
            //Debug.Log($"MonsterAttack - hit ImportantNPC");
            _isNPC = true;
            _isImportantNpc = true;

            HandleNPCAttack();
        }
    }

    private void HandleNPCAttack()
    {
        //gameObject.transform.LookAt(target.Value);
        self.Value.transform.LookAt(target.Value);


        monster.SetAnimation(false, false, false, true, false, false);

        if (npcCache == null)
        {
            target.Value.gameObject.TryGetComponent(out npcCache);
        }

        npcCache?.NpcStop();
        npcCache?.OnHitSuccess();
    }

    public override NodeResult Execute()
    {
        if (target.Value = null)
        {
            Debug.LogWarning($"EyeTypeAttack - Execute() - target is null, rework");
            _isRework = true;
            return NodeResult.failure;
        }

        if (_isPlayer) return NodeResult.success;

        if (_isNPC)
        {
            stateInfo = monster.animator.GetCurrentAnimatorStateInfo(0);
            //Debug.Log($"MonsterAttack - Execute() - {stateInfo.IsName("Attack")}, {stateInfo.normalizedTime}");

            if (stateInfo.IsName("Attack") && stateInfo.normalizedTime >= 1.0f)
            {
                //Debug.Log($"MonsterAttack - Execute() - attack end");

                npcCache = null;
                if (targetNpc.Value != null) return NodeResult.success;
                if (_isImportantNpc)
                {
                    //Debug.LogWarning($"EyeTypeAttack - Execute() - ImportantNPC attacked, game over");
                    monster.SetAnimation(true, false, false, false, false, false);
                }
                return NodeResult.failure;
            }
        }

        return NodeResult.running;
    }

    public override void OnExit()
    {
        if (_isRework)
        {
            //Debug.Log($"EyeTypeAttack OnExit - _isRework : {_isRework}");
            
            canAttack.Value = false;
            if (targetNpc.Value == null) isLost.Value = true;
            target.Value = null;
            isDetect.Value = false;
            isWork.Value = true;
        }
    }
}