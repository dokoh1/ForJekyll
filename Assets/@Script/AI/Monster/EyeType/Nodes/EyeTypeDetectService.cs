using MBT;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[AddComponentMenu("")]
[MBTNode("Example/EyeType Detect Service")]
public class EyeTypeDetectService : Service
{
    public BoolReference isDetect;
    public BoolReference isWork;
    public BoolReference haveTarget;
    public BoolReference isFind;

    public TransformReference self;
    public TransformReference target;
    public TransformReference player;
    public TransformReference targetNpc;

    public FloatReference viewAngle;
    public FloatReference findRange;
    public FloatReference chaseRange;
    public IntReference curState;

    public LayerMask obstructionMask;
    public LayerMask targetMask;

    public Transform detectorHigh;
    public Transform detectorLow;

    private NativeArray<RaycastCommand> _commandArray;
    private NativeArray<RaycastHit> _results;
    private List<Transform> _detected = new();
    private float[] _cachedAngles;
    private QueryParameters _queryParams;

    private float _chaseRange;
    private float _findRange;
    [SerializeField] private float _flashRange = 5.2f;
    private float _viewAngle;
    private float _stepAngle;
    private int _rayCount;

    private float _lastDetectTime;
    [SerializeField] private float _detectInterval = 0f;

    private bool _isFind;
    private bool _isTargetNpc;

    public override void OnEnter()
    {
        //Debug.Log($"EyeTypeDetectService - OnEnter() - @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
        //Debug.Log($"EyeTypeDetectService - OnEnter() - isDetect : {isDetect.Value}");
        base.OnEnter();
        _chaseRange = chaseRange.Value;
        _findRange = findRange.Value;
        _viewAngle = viewAngle.Value;
        _rayCount = Mathf.CeilToInt(_viewAngle * 0.6f); // 0.52f
        _stepAngle = _viewAngle / _rayCount;

        _commandArray = new NativeArray<RaycastCommand>(_rayCount * 2, Allocator.Persistent);
        _results = new NativeArray<RaycastHit>(_commandArray.Length, Allocator.Persistent);

        CacheAngles();

        _queryParams = new QueryParameters(
            layerMask: targetMask, 
            hitMultipleFaces: false,
            hitTriggers: QueryTriggerInteraction.Collide,
            hitBackfaces: false
        );
    }

    private void CacheAngles()
    {
        _cachedAngles = new float[_rayCount];
        for (int i = 0; i < _rayCount; i++)
        {
            _cachedAngles[i] = -_viewAngle / 2f + _stepAngle * i;
        }
    }

    private void OnDestroy()
    {
        if (_commandArray.IsCreated)
        {
            _commandArray.Dispose();
        }
        if (_results.IsCreated)
        {
            _results.Dispose();
        }
    }

    public override void Task()
    {
        //if (GameManager.Instance.IsTimeStop) return;
        if (Time.time - _lastDetectTime >= _detectInterval)
        {
            _lastDetectTime = Time.time;
            ScheduleRaycasts();
        }
    }

    private void ScheduleRaycasts()
    {
        _detected.Clear();

        float detectRange;

        if (curState.Value == (int)MonsterState.Run || curState.Value == (int)MonsterState.Find)
        {
            detectRange = _chaseRange;
        }
        else detectRange = _findRange;

        if (!GameManager.Instance.Player.CurState.HasFlag(PlayerEnum.PlayerState.Flash) && targetNpc.Value == null) detectRange = 4f;

        int cmdIndex = 0;

        for (int i = 0; i < _rayCount; i++)
        {
            float angle = _cachedAngles[i];

            Vector3 dirLow = Quaternion.Euler(0, angle, 0) * detectorLow.forward;
            Vector3 dirHigh = Quaternion.Euler(0, angle, 0) * detectorHigh.forward;

            _commandArray[cmdIndex++] = new RaycastCommand(detectorLow.position, dirLow, _queryParams, detectRange);
            _commandArray[cmdIndex++] = new RaycastCommand(detectorHigh.position, dirHigh, _queryParams, detectRange);
        }

        JobHandle handle = RaycastCommand.ScheduleBatch(_commandArray, _results, 1, default);
        handle.Complete();
        ProcessResults();
    }

    private void ProcessResults()
    {
        _isFind = false;
        _isTargetNpc = false;

        for (int i = 0; i < _results.Length; i++)
        {
            var hit = _results[i];
            if (hit.collider == null) continue;

            if (((1 << hit.collider.gameObject.layer) & obstructionMask) != 0)
            {
                //Debug.Log($"EyeTypeDetectService - ProcessResults() - Obstruction detected: {hit.collider.gameObject.name}");
                continue;
            }

            if (hit.collider.gameObject.layer == 18)
            {
                float dist = hit.distance;
                if (dist < _flashRange)
                {
                    //Debug.Log($"EyeTypeDetectService - ProcessResults() - Flash detected at distance: {dist}");
                    _detected.Add(player.Value);
                    continue;
                }
            }

            _detected.Add(hit.collider.transform);
        }

        if (_detected.Count > 0)
        {
            //Debug.Log($"target : {target.Value}, targetNpc : {targetNpc.Value}");
            
            if (target.Value == null && targetNpc.Value == null)
            {
                _isFind = true;
            }

            target.Value = GetClosest(_detected);

            if (target.Value == targetNpc.Value)
            {
                _isTargetNpc = true;
            }

            if (_isFind && !isFind.Value && !_isTargetNpc)
            {
                //Debug.Log($"EyeTypeDetectService - ProcessResults() - 타겟 발견!");
                isFind.Value = true;
            }

            isDetect.Value = true;
        }
        else
        {
            if (targetNpc.Value != null)
            {
                haveTarget.Value = true;
                //Debug.Log($"EyeTypeDetectService - ProcessResults() - Using targetNpc: {targetNpc.Value.name}");

            }
            else
            {
                target.Value = null;
                isDetect.Value = false;
                //Debug.Log("EyeTypeDetectService - ProcessResults() - No targets detected.");
            }

        }
    }

    Transform GetClosest(List<Transform> targets)
    {
        Transform closest = null;
        float closestDist = float.MaxValue;

        foreach (var t in targets)
        {
            if (t == null) continue;
            float dist = Vector3.Distance(self.Value.position, t.position);
            if (dist < closestDist)
            {
                closest = t;
                closestDist = dist;
            }
        }

        return closest;
    }

    private void OnDisable()
    {
        if (_commandArray.IsCreated)
            _commandArray.Dispose();
        if (_results.IsCreated)
            _results.Dispose();
    }
}