using UnityEngine;

/// <summary>
/// TimerLure / BrokenTimerLure 랜덤 스폰 포인트.
/// Start() 시 확률에 따라 Normal 또는 Broken 프리팹 생성.
/// </summary>
public class ThrowableSpawnPoint : MonoBehaviour
{
    [SerializeField] private GameObject normalPrefab;
    [SerializeField] private GameObject brokenPrefab;

    [Range(0f, 1f)]
    [SerializeField] private float brokenChance = 0.3f;

    private void Start()
    {
        if (normalPrefab == null && brokenPrefab == null)
            return;

        GameObject prefab = Random.value < brokenChance ? brokenPrefab : normalPrefab;
        if (prefab == null)
            prefab = normalPrefab != null ? normalPrefab : brokenPrefab;

        Instantiate(prefab, transform.position, transform.rotation, transform);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.Lerp(Color.green, Color.red, brokenChance);
        Gizmos.DrawWireSphere(transform.position, 0.3f);
        Gizmos.DrawIcon(transform.position, "d_Prefab Icon", true);
    }
#endif
}
