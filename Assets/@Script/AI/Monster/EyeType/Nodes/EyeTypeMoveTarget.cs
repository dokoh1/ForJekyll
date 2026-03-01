using MBT;
using System;
using UnityEngine;

[AddComponentMenu("")]
[MBTNode("Example/EyeType Move Target")]
public class EyeTypeMoveTarget : MoveToTransform
{
    public TransformReference self;

    public IntReference curState;

    public FloatReference baseSpeed;
    public FloatReference runSpeedModifier;

    public BoolReference isDetect;
    public BoolReference haveTarget;
    public BoolReference isOriginPosition;

    public MonsterV2 monster;

    private bool _isMoveFail;

    private Vector3 _lastPosition;
    private float _stuckTimer = 0f;
    [SerializeField] private float stuckThreshold = 0.2f;
    [SerializeField] private float stuckTimeLimit = 0.35f;

    private float _screamTime = 0f;
    private float _screamInterval = 3f;

    public override void OnEnter()
    {
        base.OnEnter();
        _isMoveFail = false;

        MonsterState state = (MonsterState)curState.Value;
        if (!state.HasFlag(MonsterState.Walk))
        {
            curState.Value = (int)MonsterState.Walk;
        }

        agent.speed = baseSpeed.Value * runSpeedModifier.Value;
        monster.SetAnimation(false, false, true, false, false, false);
        isOriginPosition.Value = false;

        _screamTime = 0f;
        GameManager.Instance.Player.BGMController.BeginChase(monster);
    }

    public override NodeResult Execute()
    {
        if (GameManager.Instance.IsTimeStop) return NodeResult.running;

        if (isDetect.Value) return NodeResult.failure;

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

        if ((self.Value.position - _lastPosition).sqrMagnitude < stuckThreshold * stuckThreshold)
        {
            _stuckTimer += Time.deltaTime;
            if (_stuckTimer >= stuckTimeLimit)
            {
                Debug.LogWarning("몬스터가 제자리에 머물고 있어 목적지를 재설정합니다.");
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

        if (agent.pathPending) return NodeResult.running;
        if (agent.hasPath) return NodeResult.running;
        if (agent.remainingDistance < stopDistance) return NodeResult.success;
        return NodeResult.success;
    }

    public override void OnExit()
    {
        base.OnExit();
        if (isDetect.Value) haveTarget.Value = false;
    }
}