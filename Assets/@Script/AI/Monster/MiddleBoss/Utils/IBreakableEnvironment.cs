using UnityEngine;

/// <summary>
/// 환경 오브젝트가 구현할 수 있는 인터페이스
/// 돌진 시 파괴 여부를 내부에서 결정
/// </summary>

public interface IBreakableEnvironment
{
    void OnHitByMidBoss(Vector3 hitPoint, Vector3 hitNormal);
    bool IsBroken { get; }
}

