using System.Collections;
using UnityEngine;

/// <summary>
/// 즉발형 투척체: 유리병.
/// 충돌 시 파괴 + 즉발 소음 + 게이지 즉시 증가.
/// </summary>
public class GlassBottleThrowable : ThrowableBase
{
    [Header("Break Settings")]
    [SerializeField] private float minBreakVelocity = 2.0f;

    [Header("Noise Settings")]
    [SerializeField] private float noiseOnImpact = 80f;
    [SerializeField] private float gaugeInstantAmount = 80f;
    [SerializeField] private float noiseDuration = 1.0f;

    [Header("Visual / Audio")]
    [SerializeField] private GameObject intactMesh;
    [SerializeField] private GameObject brokenMesh;
    [SerializeField] private AudioClip breakClip;

    private const float ThrowCollisionIgnoreTime = 0.15f;

    [SerializeField] private bool _isBroken;
    private float _throwTime;

    protected override void Start()
    {
        base.Start();

        if (intactMesh != null)
            intactMesh.SetActive(true);
        if (brokenMesh != null)
            brokenMesh.SetActive(false);
    }

    private IEnumerator NoiseDecayRoutine()
    {
        yield return new WaitForSeconds(noiseDuration);
        NoiseCheckAmount = 0f;
        Destroy(gameObject);
    }

    public override void Interact()
    {
        if (!IsInteractable || _isBroken)
            return;

        if (!_isHeld)
            PickUp();
    }

    protected override void OnThrown()
    {
        _throwTime = Time.time;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_isHeld || _isBroken)
            return;

        // 던진 직후 플레이어 Collider 즉시 충돌 무시
        if (Time.time - _throwTime < ThrowCollisionIgnoreTime)
            return;

        if (collision.relativeVelocity.magnitude < minBreakVelocity)
            return;

        BreakBottle(collision.GetContact(0).point);
    }

    private void BreakBottle(Vector3 hitPoint)
    {
        _isBroken = true;
        IsInteractable = false;

        _rb.isKinematic = true;
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;

        if (intactMesh != null) intactMesh.SetActive(false);
        if (brokenMesh != null) brokenMesh.SetActive(true);

        if (audioSource != null && breakClip != null)
            audioSource.PlayOneShot(breakClip);

        NoiseCheckAmount = noiseOnImpact;
        StartCoroutine(NoiseDecayRoutine());

        if (NoiseDetectionGaugeManager.Instance != null)
            NoiseDetectionGaugeManager.Instance.AddInstant(gaugeInstantAmount);
    }
}
