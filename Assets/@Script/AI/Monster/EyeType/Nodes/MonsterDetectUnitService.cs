using MBT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("")]
[MBTNode("Example/Monster Detect Unit Service")]
public class MonsterDetectUnitService : Service
{
    public BoolReference isDetect;
    public BoolReference skipValueIsWork;
    public BoolReference isLostTarget;
    public TransformReference self;
    public TransformReference target;
    public TransformReference player;
    public FloatReference viewAngle;
    public FloatReference findRange;
    public FloatReference chaseRange;
    public FloatReference distanceToTarget;
    public IntReference curState;

    public LayerMask obstructionMask;
    public LayerMask targetMask;

    public Transform detectorHigh;
    public Transform detectorLow;
    private float _chaseRange;
    //private float _chaseRangeFlash;
    //private float _chaseRangeOrigin;
    private float _rangeAddValue = 5.2f;
    private float _stepAngle;
    private int _rayCount;
    private List<Transform> _detectList = new List<Transform>();
    //private bool _isObstruction;
    private Coroutine _detectCoroutine;

    public override void OnEnter()
    {
        //Debug.Log($"MonsterDetectUnitService - OnEnter() - @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
        //Debug.Log($"MonsterDetectUnitService - OnEnter() - isDetect : {isDetect.Value}");

        _chaseRange = chaseRange.Value;
        //_chaseRangeOrigin = chaseRange.Value;
        //_chaseRangeFlash = _chaseRangeOrigin + _rangeAddValue;
        _rayCount = Mathf.CeilToInt(viewAngle.Value * 0.6f);//0.52f
        _stepAngle = viewAngle.Value / _rayCount;
        base.OnEnter();        
    }

    public override void Task()
    {
        if (!skipValueIsWork.Value)
        {
            return;
        }

        StartDetectTargets();
    }

    public void StartDetectTargets()
    {
        if (_detectCoroutine != null || !gameObject.activeInHierarchy)
        {
            return;
        }

        _detectCoroutine = StartCoroutine(DetectTargetsCoroutine());
    }

    private IEnumerator DetectTargetsCoroutine()
    {
        _detectList.Clear();
        //_isObstruction = false;
        float range = curState.Value == (int)MonsterState.Run ? _chaseRange : findRange.Value;
        bool foundTarget = false;

        for (int i = 0; i < _rayCount; i++)
        {
            float angle = -viewAngle.Value / 2f + _stepAngle * i;
            Vector3 directionLow = Quaternion.Euler(0, angle, 0) * detectorLow.forward;
            Vector3 directionHigh = Quaternion.Euler(0, angle, 0) * detectorHigh.forward;

            Debug.DrawRay(detectorLow.position, directionLow * range, Color.green);

            if (PerformRaycast(detectorLow.position, directionLow, range) ||
                PerformRaycast(detectorHigh.position, directionHigh, range))
            {
                foundTarget = true;
            }
            yield return null;  
        }

        yield return null;

        if (!foundTarget)
        {
            yield return null;

            if (!DetectVaildCheck())
            {
                ResetTarget();
            }
        }
        else 
        {
            AssignClosestTarget();
            if (isLostTarget.Value) isLostTarget.Value = false;
        }

        _detectCoroutine = null;
    }

    private bool DetectVaildCheck()
    {
        if (target.Value != null && distanceToTarget.Value < _chaseRange && curState.Value == (int)MonsterState.Run)
        {
            self.Value.LookAt(target.Value);
            Vector3 targetVector3 = new Vector3(target.Value.position.x, detectorLow.position.y, target.Value.position.z);
            Vector3 direction = (targetVector3 - detectorLow.position);

            if (Physics.Raycast(detectorLow.position, direction, out RaycastHit hit, _chaseRange * 2f))
            {
                if (hit.transform == target.Value)
                {
                    _detectList.Add(target.Value);
                    return true;
                }
            }
            else
            {
                _detectList.Add(target.Value);
                return true;
            }
        }
        return false;
    }


    private bool PerformRaycast(Vector3 origin, Vector3 direction, float range)
    {
        if (Physics.Raycast(origin, direction, range, obstructionMask))
        {
            //_isObstruction = true;
            return false;
        }

        if (Physics.Raycast(origin, direction, out RaycastHit hit, range, targetMask))
        {
            if (hit.collider != null)
            {
                _detectList.Add(hit.collider.transform);
                return true;
            }
        }
        return false;
    }

    private void AssignClosestTarget()
    {
        if (_detectList.Count == 0)
        {
            ResetTarget();
            return;
        }

        Transform closestTarget = null;
        float closestDistance = float.MaxValue;

        foreach (var detected in _detectList)
        {
            if (detected == null) continue;
            float distance = Vector3.Distance(self.Value.position, detected.position);
            if (distance < closestDistance)
            {
                closestTarget = detected;
                closestDistance = distance;
            }
        }

        if (closestTarget != null)
        {            
            target.Value = closestTarget;
            if (target.Value.gameObject.layer == LayerMask.NameToLayer("Flash"))
            {
                if (Vector3.Distance(self.Value.position, target.Value.position) < _rangeAddValue)
                {
                    target.Value = player.Value;
                    //_chaseRange = _chaseRangeFlash;
                }

                if (!GameManager.Instance.Player.CurState.HasFlag(PlayerEnum.PlayerState.Flash))
                {
                    //Debug.Log("flash off");
                    ResetTarget();
                    return;
                }
            }

            isDetect.Value = true;
            isLostTarget.Value = false;
        }
        else
        {
            ResetTarget();
        }
    }

    private void ResetTarget()
    {
        isLostTarget.Value = true;
        target.Value = null;    
        isDetect.Value = false;
    }

    public override void OnExit()
    {
        //Debug.Log($"MonsterDetectUnitService - OnExit() - @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
        //Debug.Log($"MonsterDetectUnitService - OnExit() - isDetect : {isDetect.Value}");

        if (_detectCoroutine != null)
        {
            StopCoroutine(_detectCoroutine);
            _detectCoroutine = null;
        }

        base.OnExit();
    }
}
