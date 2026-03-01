using System.Collections;
using UnityEngine;

/// <summary>
/// 고장형 투척체: 줍는 즉시 소음 시작. noiseDuration 후 자동 파괴.
/// </summary>
public class BrokenTimerLureThrowable : NoiseLureThrowable
{
    [Header("Visual")]
    [SerializeField] private GameObject crackIndicator;

    [Header("Broken Audio")]
    [SerializeField] private AudioClip brokenRevealClip;

    protected override void OnPickedUp()
    {
        StartBrokenNoiseOnce();
    }

    private void StartBrokenNoiseOnce()
    {
        if (_noiseActive)
            return;

        if (crackIndicator != null)
            crackIndicator.SetActive(true);

        if (audioSource != null && brokenRevealClip != null)
            audioSource.PlayOneShot(brokenRevealClip);

        if (_noiseRoutine != null)
            StopCoroutine(_noiseRoutine);

        _noiseRoutine = StartCoroutine(BrokenNoiseRoutine());
    }

    private IEnumerator BrokenNoiseRoutine()
    {
        StartNoise();

        yield return new WaitForSeconds(noiseDuration);

        StopNoise();
        _noiseRoutine = null;

        while (_isHeld)
            yield return null;

        Destroy(gameObject);
    }
}
