using DG.Tweening;
using MBT;
using UnityEngine;

[AddComponentMenu("")]
[MBTNode("Example/EyeType Chase Target")]
public class EyeTypeChaseTarget : MoveToTransform
{
    public TransformReference targetNpc;
    public TransformReference player;
    public TransformReference self;

    public IntReference curState;

    //public FloatReference distanceToTarget;
    public FloatReference chaseRange;
    public FloatReference baseSpeed;
    public FloatReference runSpeedModifier;
    public FloatReference attackRange;

    public BoolReference haveTarget;
    public BoolReference isDetect;
    public BoolReference isLost;
    public BoolReference canAttack;
    public BoolReference isOriginPosition;
    public BoolReference isFind;

    public MonsterV2 monster;

    private bool _isMoveFail;

    //private float _distanceToTarget;
    private float _chaseRange;
    [SerializeField] private float _flashRange = 5.2f;

    //private Transform _destination;

    private Vector3 _lastPosition;
    private float _stuckTimer = 0f;
    [SerializeField] private float stuckThreshold = 0.2f;
    [SerializeField] private float stuckTimeLimit = 0.35f;
    private float _screamTime = 0f;
    private float _screamInterval = 3f;

    public override void OnEnter()
    {
        if (!CheckCanChase())
        {
            Debug.LogWarning($"EyeTypeChaseTarget - OnEnter() - destination is null or not detected");
            CheckTargetNpc();
            return;
        }

        //Debug.Log($"EyeTypeChaseTarget - OnEnter(), destination : {destination.Value.name}, layer : {destination.Value.gameObject.layer}");
        if (destination.Value.gameObject.layer == 18 && CheckDistanceToTarget() <= _flashRange)
        {
            Debug.LogWarning($"EyeTypeChaseTarget - OnEnter() - destination is flash, target : {destination.Value.name}");
            destination.Value = player.Value;
        }

        //Debug.Log($"EyeTypeChaseTarget - OnEnter() - destination : {destination.Value.name}, layer : {destination.Value.gameObject.layer}");
        _screamTime = 0f;
        time = 0;
        //_distanceToTarget = CheckDistanceToTarget();
        //_destination = destination.Value;
        _chaseRange = chaseRange.Value;

        _lastPosition = transform.position;
        _stuckTimer = 0f;


        MonsterState state = (MonsterState)curState.Value;
        if (!state.HasFlag(MonsterState.Run))
        {
            curState.Value = (int)MonsterState.Run;
        }

        agent.speed = baseSpeed.Value * runSpeedModifier.Value;
        monster.SetAnimation(false, false, true, false, false, false);
        agent.isStopped = false;
        _isMoveFail = monster.MoveToDestination(destination.Value.position);
        isOriginPosition.Value = false;
        if (!_isMoveFail) GameManager.Instance.Player.BGMController.BeginChase(monster);
    }

    public override NodeResult Execute()
    {
        if (GameManager.Instance.IsTimeStop) return NodeResult.running;

        if (destination.Value == null)
        {
            Debug.LogWarning($"EyeTypeChaseTarget - Execute() - destination is null, stop");

            //Debug.LogWarning($"EyeTypeChaseTarget - Execute() - destination is null, target : {targetNpc.Value.name}");
            //CheckTarget();

            if (targetNpc.Value != null)
            {
                //Debug.Log($"EyeTypeChaseTarget - CheckTarget() - targetNpc : {targetNpc.Value.name}");
                haveTarget.Value = true;
                return NodeResult.success;
            }
            else
            {
                //Debug.LogWarning($"EyeTypeChaseTarget - CheckTarget() - targetNpc is null, checking player");
                isLost.Value = true;
                isDetect.Value = false;
                destination.Value = null;
                return NodeResult.failure;
            }
        }


        if (!CheckCanChase())
        {
            Debug.LogWarning($"EyeTypeChaseTarget - Execute() - destination is null or not detected");
            CheckTarget();
        }

        if (destination.Value.gameObject.layer == 18)
        {
            //Debug.LogWarning($"EyeTypeChaseTarget - Execute() - destination is flash, target : {destination.Value.name}");

            if (CheckDistanceToTarget() <= _flashRange)
            {
                Debug.LogWarning($"EyeTypeChaseTarget - Execute() - _distanceToTarget <= _flashRange, target : {destination.Value.name}");
                //CheckTarget();
                destination.Value = player.Value;
                _isMoveFail = monster.MoveToDestination(destination.Value.position);
                if (!CheckCanChase())
                {
                    CheckTarget();
                }

                return NodeResult.running;

                //isFind.Value = true;
                //return NodeResult.success;
            }
        }
        else
        {
            // player or npc
            if (CheckDistanceToTarget() < attackRange.Value)
            {
                //Debug.Log($"EyeTypeChaseTarget - Execute() - distanceToTarget.Value < attackRange.Value");
                canAttack.Value = true;
                return NodeResult.failure;
            }
        }

        if ((self.Value.position - _lastPosition).sqrMagnitude < stuckThreshold * stuckThreshold)
        {
            _stuckTimer += Time.deltaTime;
            if (_stuckTimer >= stuckTimeLimit)
            {
                agent.ResetPath();
                _isMoveFail = monster.MoveToDestination(destination.Value.position);
                _stuckTimer = 0f;
                _lastPosition = transform.position;
                return NodeResult.success;
            }
        }
        else
        {
            _stuckTimer = 0f;
            _lastPosition = transform.position;
        }

        time += Time.deltaTime;
        _screamTime += Time.deltaTime;

        if (_screamTime > _screamInterval)
        {
            _screamTime = 0f;
            monster.Sound.PlaySoundByKey(MonsterSoundKey.Chase, false);
         }

        if (time > updateInterval)
            {
                time = 0;
                _isMoveFail = monster.MoveToDestination(destination.Value.position);
                self.Value.transform.LookAt(destination.Value.transform);
            }

        

        if (_isMoveFail)
        {
            Debug.LogError($"대상이 이동 불가지역에 있습니다 - 대상 : {destination.Value.name}");
            return NodeResult.success;
        }

        if (CheckDistanceToTarget() >= _chaseRange)
        {
            //Debug.LogWarning($"EyeTypeChaseTarget - Execute() - CheckDistanceToTarget() >= chaseRange, target : {destination.Value.name}");
            CheckTarget();
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
            Debug.Log($"EyeTypeChaseTarget - Execute() - agent.remainingDistance < stopDistance");
            return NodeResult.success;
        }

        return NodeResult.success;
    }

    private bool CheckCanChase()
    {
        if (destination.Value == null || !isDetect.Value)
        {
            Debug.LogWarning($"EyeTypeChaseTarget - CheckTarget() - destination is null or not detected");
            return false;
        }

        if (destination.Value.gameObject.layer == 0)
        {
            Debug.LogWarning($"EyeTypeChaseTarget - CheckTarget() - destination layer is 0");
            return false;
        }

        if (destination.Value.gameObject.layer == 18 && !GameManager.Instance.Player.CurState.HasFlag(PlayerEnum.PlayerState.Flash))
        {
            Debug.LogWarning($"EyeTypeChaseTarget - CheckTarget() - lost flash");
            return false;
        }
        return true;
    }

    private NodeResult CheckTarget()
    {
        if (targetNpc.Value != null)
        {
            //Debug.Log($"EyeTypeChaseTarget - CheckTarget() - targetNpc : {targetNpc.Value.name}");
            haveTarget.Value = true;
            return NodeResult.success;
        }
        else
        {
            //Debug.LogWarning($"EyeTypeChaseTarget - CheckTarget() - targetNpc is null, checking player");
            isLost.Value = true;
            isDetect.Value = false;
            destination.Value = null;
            return NodeResult.failure;
        }
    }

    private void CheckTargetNpc()
    {
        if (targetNpc.Value != null)
        {
            haveTarget.Value = true;
        }
        else
        {
            isLost.Value = true;
        }
    }

    private float CheckDistanceToTarget()
    {
        return Vector3.Distance(gameObject.transform.position, destination.Value.position);
    }

    public override void OnExit()
    {
        base.OnExit();
        
        //Debug.Log($"EyeTypeChaseTarget - OnExit() - target : {destination.Value}, isDetect : {isDetect.Value}");

        if (!isDetect.Value && destination.Value == null)
        {
            isLost.Value = true;
        }
    }
}