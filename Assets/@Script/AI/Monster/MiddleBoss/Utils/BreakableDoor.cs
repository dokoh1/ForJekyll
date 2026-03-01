using UnityEngine;
/// <summary>
/// 돌진으로 부숴야 하는 문.
/// -HP와 RequiredDistance에 따라 데미지 판정.
/// - 2회 유효 충돌 시 완전 파괴.
/// </summary>
public class BreakableDoor : MonoBehaviour
{
    [SerializeField] private int  maxHp          = 2;
    // 파괴 시 gameObject 비활성화 여부
    [SerializeField] private bool autoDestroyOnBroken = true;

    public int CurrentHp { get; private set; } // 현재 Hp
    public bool IsDestroyed { get; private set; } // 파괴 여부

    private void Awake()
    {
        CurrentHp = maxHp;
    }
    /// <summary>
    /// MidBossEarType이 돌진 중 이 문에 충돌했을 때 호출.
    /// </summary>
    /// <param name="chargeDistance">StartPos ~ HitPos 거리</param>
    /// <param name="requiredDistance">파괴 가능한 최소 거리(12m)</param>
    /// <param name="auraDistance">붉은 오라 시작 거리</param>
    /// <param name="midBoss">보스 참조 (광폭화 전환용)</param>
    /// <returns>유효 타격(HP 감소)이었는지 여부</returns>
    public bool OnHitByMidBoss(float chargeDistance, float requiredDistance, MidBossEarType midBoss)
    {
        if (IsDestroyed) 
            return false;

        bool canBreak = chargeDistance >= requiredDistance;

        if (canBreak)
        {
            CurrentHp--;
            PlayStrongHitFX(chargeDistance >= requiredDistance);

            if (CurrentHp <= 0)
            {
                DestroyDoor();
            }

            return true;
        }
        else
        {
            PlayWeakHitFX();
            return false;
        }
    }

    private void PlayStrongHitFX(bool hadAura)
    {
        // TODO: 균열 + 빛샘 + Screen Shake(강)
    }

    private void PlayWeakHitFX()
    {
        // TODO: 흔들리기만 하는 약한 연출
    }

    private void DestroyDoor()
    {
        IsDestroyed = true;

        // TODO: 파편, 먼지, 탈출 경로 오픈, 씬 전환 등
        if (autoDestroyOnBroken)
        {
            gameObject.SetActive(false);
        }
    }

}