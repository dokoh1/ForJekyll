using UnityEngine;

/// <summary>
/// 소음 유인 투척체 공통 베이스.
/// StartNoise/StopNoise, noiseDuration, noiseAmount, OnDisable 공유.
/// </summary>
public abstract class NoiseLureThrowable : ThrowableBase
{
    [Header("Noise Settings")]
    [SerializeField] protected float noiseDuration = 3f;
    [SerializeField] protected float noiseAmount = 80f;

    protected bool _noiseActive;
    protected Coroutine _noiseRoutine;

    protected virtual void OnDisable()
    {
        StopNoise();
    }

    protected void StartNoise()
    {
        if (_noiseActive)
            return;

        _noiseActive = true;
        NoiseCheckAmount = noiseAmount;

        NoiseDetectionGaugeManager.Instance?.RegisterTimerNoise();
    }

    protected virtual void StopNoise()
    {
        if (!_noiseActive)
            return;

        _noiseActive = false;
        NoiseCheckAmount = 0f;

        NoiseDetectionGaugeManager.Instance?.UnregisterTimerNoise();
    }
}
