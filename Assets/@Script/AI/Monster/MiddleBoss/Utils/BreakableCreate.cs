using UnityEngine;

/// <summary>
/// 파괴 가능한 크레이트. MidBoss 돌진 시 파편으로 교체 후 자신을 Destroy.
/// </summary>
public class BreakableCrate : MonoBehaviour, IBreakableEnvironment
{
    [Header("Prefab References")]
    public GameObject brokenPrefab;
    public GameObject hitVfxPrefab;
    public AudioClip breakSfx;
    public bool IsBroken { get; private set; }

    [Header("Tuning")]
    [SerializeField] private float explosionForce = 150f;
    [SerializeField] private float brokenLifeTime = 2f;

    GameObject _brokenInstance;
    Rigidbody[] _brokenRBs;

    void Awake()
    {
        if (brokenPrefab != null)
        {
            _brokenInstance = Instantiate(brokenPrefab, transform);
            _brokenInstance.transform.localPosition = Vector3.zero;
            _brokenInstance.transform.localRotation = Quaternion.identity;
            _brokenInstance.SetActive(false);

            _brokenRBs = _brokenInstance.GetComponentsInChildren<Rigidbody>(true);
        }
    }

    public void OnHitByMidBoss(Vector3 hitPoint, Vector3 hitNormal)
    {
        if (IsBroken) return;

        if (hitVfxPrefab != null)
        {
            var rot = hitNormal.sqrMagnitude > 0.0001f ? Quaternion.LookRotation(hitNormal) : Quaternion.identity;
            Instantiate(hitVfxPrefab, hitPoint, rot);
        }

        Break(hitPoint, hitNormal);
    }

    void Break(Vector3 hitPoint, Vector3 hitNormal)
    {
        IsBroken = true;

        if (_brokenInstance != null)
        {
            _brokenInstance.transform.SetParent(null);
            _brokenInstance.SetActive(true);

            Vector3 pushDir = hitNormal.sqrMagnitude > 0.0001f ? hitNormal.normalized : -transform.forward;

            foreach (var rb in _brokenRBs)
            {
                if (!rb) continue;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.WakeUp();
                rb.AddForce(pushDir * explosionForce, ForceMode.Impulse);
            }

            Destroy(_brokenInstance, brokenLifeTime);
        }

        if (breakSfx != null)
            AudioSource.PlayClipAtPoint(breakSfx, transform.position);

        Destroy(gameObject);
    }
}
