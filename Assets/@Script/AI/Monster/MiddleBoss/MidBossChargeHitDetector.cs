using UnityEngine;

/// <summary>
/// MidBossEarType의 자식에 붙는 콜라이더용 스크립트.
/// - 보스가 Charge 중일 때 어떤 오브젝트에 부딪혔는지 감지해서 MidBossEarType.OnChargeHit()에게 알려준다.
/// - 실제 충돌 지점(hitPoint)도 Contact 포인트로 넘긴다.
/// </summary>
[RequireComponent(typeof(Collider))]
public class MidBossChargeHitDetector : MonoBehaviour
{
    [SerializeField] private MidBossEarType midBoss;

    private void Reset()
    {
        if (midBoss == null)
            midBoss = GetComponentInParent<MidBossEarType>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (midBoss == null)
        {
            return;
        }

        if (!midBoss.IsCharging)
        {
            return;
        }

        Vector3 attackerPos = transform.position;
        Vector3 hitPoint = other.ClosestPoint((attackerPos));
        Vector3 hitNormal = (hitPoint - attackerPos).normalized;
        
        midBoss.OnChargeHit(other, hitPoint, hitNormal);
    }
}