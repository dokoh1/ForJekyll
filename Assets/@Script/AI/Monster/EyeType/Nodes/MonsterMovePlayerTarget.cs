using MBT;
using UnityEngine;

[AddComponentMenu("")]
[MBTNode("Example/Monster Move Player Target")]
public class MonsterMovePlayerTarget : MoveToTransform
{
    public TransformReference self;
    public TransformReference player;
    //public TransformReference target;
    public IntReference curState;
    public IntReference monsterType;
    public FloatReference distanceToTarget;
    public FloatReference chaseRange;
    public FloatReference baseSpeed;
    public FloatReference runSpeedModifier;
    public FloatReference attackRange;
    public BoolReference skipValueLostToTarget;
    public BoolReference isOriginPosition;
    public BoolReference isLost;
    public BoolReference isDetect;

    public Monster monster;
    //private bool _canAttack;
    private bool _isMoveFail;
    private bool _isLost;
    private bool _isChasing;

    public override void OnEnter()
    {
        //Debug.Log($"MonsterMovePlayerTarget - OnEnter(), destination : {destination.Value.name}, layer : {destination.Value.gameObject.layer}");

        if (destination.Value.gameObject.layer == 0)
        {
            //Debug.LogWarning($"MonsterMovePlayerTarget - OnEnter() - {destination.Value.gameObject.name}");
            destination.Value = null;
            return;
        }

        if (destination.Value.gameObject.layer == 18 && !GameManager.Instance.Player.CurState.HasFlag(PlayerEnum.PlayerState.Flash))
        {
            //Debug.LogWarning($"MonsterMovePlayerTarget - OnEnter() - lost flash");
            destination.Value = null;
            return;
        }


        _isChasing = true;
        _isLost = false;
        _isMoveFail = false;
        MonsterState state = (MonsterState)curState.Value;
        if (!state.HasFlag(MonsterState.Run))
        {
            // monster.Sound.StopStepAudio();
            curState.Value = (int)MonsterState.Run;
        }
        // monster.Sound.PlayStepSound((MonsterState)curState.Value);
        agent.speed = baseSpeed.Value * runSpeedModifier.Value;
        monster.SetAnimation(false, false, true, false, false);

        time = 0;
        agent.isStopped = false;
        if (destination.Value == null) return;
        //agent.SetDestination(destination.Value.position);
        _isMoveFail = monster.MoveToDestination(destination.Value.position);

    }

    public override NodeResult Execute()
    {
        // if (monster.Sound.GroundChange) monster.Sound.PlayStepSound((MonsterState)curState.Value);

        if (destination.Value == null)
        {
            //Debug.Log($"MonsterMovePlayerTarget - Execute() - destination.Value == null");
            return NodeResult.success;
        }

        if (distanceToTarget.Value < attackRange.Value)
        {
            if (Vector3.Distance(destination.Value.position, transform.position) < attackRange.Value)
            {
                //_canAttack = true;
                _isChasing = false;
                //Debug.Log($"MonsterMovePlayerTarget - Execute() - distanceToTarget.Value < attackRange.Value, destination : {destination.Value.name}, {destination.Value.gameObject.layer}");


                if (destination.Value.gameObject.layer == 18)
                {
                    //Debug.Log($"MonsterMovePlayerTarget - Execute() - destination.Value.gameObject.layer == 18");
                    //destination.Value = player.Value;
                    //_isMoveFail = monster.MoveToDestination(destination.Value.position);

                    if (!GameManager.Instance.Player.CurState.HasFlag(PlayerEnum.PlayerState.Flash))
                    {
                        //Debug.Log("monster move - flash off");
                        _isLost = true;
                        return NodeResult.success;
                    }


                    return NodeResult.running;
                }
                else
                {
                    //Debug.Log($"MonsterMovePlayerTarget - Execute() - destination.Value.gameObject.layer != 18");
                    return NodeResult.success;
                }

            }
            //else
            //{
            //    Debug.Log($"MonsterMovePlayerTarget - Execute() - distanceToTarget.Value < attackRange.Value - else");
            //}

        }

        time += Time.deltaTime;
        if (time > updateInterval)
        {
            time = 0;
            //if (destination.Value == null)
            //{
            //    Debug.Log($"MonsterMovePlayerTarget - Execute() - destination.Value == null");
            //    return NodeResult.running;
            //}

            if (destination.Value.gameObject.layer == 18 && !GameManager.Instance.Player.CurState.HasFlag(PlayerEnum.PlayerState.Flash))
            {
                //Debug.LogWarning($"MonsterMovePlayerTarget - {destination.Value.gameObject.name}");
                destination.Value = null;
                _isLost = true;
                _isChasing = false;
                return NodeResult.success;
            }

            _isMoveFail = monster.MoveToDestination(destination.Value.position);
        }

        self.Value.LookAt(destination.Value);

        if (_isMoveFail)
        {
            _isChasing = false;
            //Debug.Log($"MonsterMovePlayerTarget - Execute() - _isMoveFail");
            return NodeResult.success;
        }

        if (skipValueLostToTarget.Value)
        {
            _isLost = true;
            _isChasing = false;
            //Debug.Log($"MonsterMovePlayerTarget - Execute() - skipValueLostToTarget.Value");
            return NodeResult.success;
        }

        if (distanceToTarget.Value >= chaseRange.Value)
        {
            _isLost = true;
            _isChasing = false;
            //Debug.Log($"MonsterMovePlayerTarget - Execute() - distanceToTarget.Value >= chaseRange.Value");
            return NodeResult.success;
        }

        if (agent.pathPending)
        {
            //Debug.Log($"MonsterMovePlayerTarget - Execute() - agent.pathPending");
            return NodeResult.running;
        }

        if (agent.hasPath)
        {
            //Debug.Log($"MonsterMovePlayerTarget - Execute() - agent.hasPath");
            return NodeResult.running;
        }

        if (agent.remainingDistance < stopDistance)
        {
            _isChasing = false;
            //Debug.Log($"MonsterMovePlayerTarget - Execute() - agent.remainingDistance < stopDistance");
            return NodeResult.success;
        }

        if (isDetect.Value)
        {
            //Debug.Log($"MonsterMovePlayerTarget - Execute() - isDetect.Value");
            return NodeResult.running;
        }

        if (!isDetect.Value)
        {
            _isLost = true;
            _isChasing = false;
            //Debug.Log($"MonsterMovePlayerTarget - Execute() - !isDetect.Value");
            return NodeResult.success;  
        }

        //Debug.Log($"MonsterMovePlayerTarget - Execute() - default");
        return NodeResult.success;
    }

    public override void OnExit()
    {
        //Debug.Log($"MonsterMovePlayerTarget - OnExit() - isDetect : {isDetect.Value}, skipValueLostToTarget : {skipValueLostToTarget.Value}, _isLost : {_isLost}");

        base.OnExit();
        isOriginPosition.Value = false;
        if (_isLost)
        {
            isLost.Value = true;
            isDetect.Value = false;
            destination.Value = null;
            return;
        }

        if (_isChasing)
        {
            if (!isDetect.Value)
            {
                isLost.Value = true;
            }
        }
    }
}
