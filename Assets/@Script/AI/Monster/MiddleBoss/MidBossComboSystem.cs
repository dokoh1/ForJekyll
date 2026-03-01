using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// MidBoss 엔레이지 콤보 돌진 시스템.
/// 파괴 가능한 오브젝트를 탐색하여 연속 돌진 타겟을 설정.
/// </summary>
public class MidBossComboSystem : MonoBehaviour
{
    [SerializeField] private MidBossEarType midBoss;
    [SerializeField] private float comboSearchRadius = 1000f;
    [SerializeField] private LayerMask breakableMask;
    [SerializeField] private Transform comboSearchCenter;

    private bool _active;
    private bool _pendingAfterStun;
    private int _index;
    private readonly List<Vector3> _targets = new();

    public bool IsActive => _active;
    public bool IsPending => _pendingAfterStun;
    public int TargetCount => _targets.Count;

    // Editor 읽기용
    public int CurrentIndex => _index;
    public IReadOnlyList<Vector3> Targets => _targets;

    public void SetPending(bool value) => _pendingAfterStun = value;

    /// <summary>
    /// 스턴 종료 시 호출. 대기 중인 콤보가 있으면 활성화.
    /// </summary>
    public bool TryActivatePending()
    {
        if (!_pendingAfterStun || _targets.Count <= 0)
            return false;

        _pendingAfterStun = false;
        _active = true;
        _index = 0;
        return true;
    }

    /// <summary>
    /// 엔레이지 진입 시: 주변 파괴 가능 오브젝트를 콤보 타겟으로 수집.
    /// </summary>
    public void PrepareTargets()
    {
        _targets.Clear();

        int maxCharges = midBoss.MidBossData.Data.comboMaxCharges;
        Vector3 center = comboSearchCenter.position;
        Collider[] cols = Physics.OverlapSphere(center, comboSearchRadius, breakableMask);

        foreach (var col in cols)
        {
            if (col.TryGetComponent<IBreakableEnvironment>(out _))
            {
                _targets.Add(col.transform.position);
                if (_targets.Count >= maxCharges)
                    break;
            }
        }

        if (_targets.Count == 0)
        {
            for (int i = 0; i < maxCharges; i++)
            {
                if (TryGetRandomNavPoint(out var p))
                    _targets.Add(p);
            }
        }
    }

    /// <summary>
    /// 다음 콤보 타겟을 BB에 기록. 모두 소진 시 EndCombo 호출 후 false 반환.
    /// </summary>
    public bool TrySetNextTarget()
    {
        if (!_active)
            return false;

        if (_index >= _targets.Count)
        {
            End();
            return false;
        }

        Vector3 next = _targets[_index++];
        midBoss.SetVariable(BBKeys.MidBoss.ChargeTargetPos, next);
        return true;
    }

    public void End()
    {
        _active = false;
        _pendingAfterStun = false;
        _targets.Clear();

        NoiseDetectionGaugeManager.Instance?.SetIncreaseBlocked(false);
        midBoss.ApplyStun();
    }

    private bool TryGetRandomNavPoint(out Vector3 point)
    {
        point = midBoss.transform.position;
        float radius = midBoss.MidBossData.Data.randomNavRadius;
        if (NavMesh.SamplePosition(
                midBoss.transform.position + Random.insideUnitSphere * radius,
                out var hit,
                radius,
                NavMesh.AllAreas))
        {
            point = hit.position;
            return true;
        }

        return false;
    }
}
