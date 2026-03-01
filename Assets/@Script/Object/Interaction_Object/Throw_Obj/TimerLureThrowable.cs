using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// 타이머형 투척체: 던진 후 _adjustedDelay 초 뒤 소음 발생.
/// 마우스 스크롤로 딜레이 조절 가능 (1~10초). 소음 전 재줍기로 취소 가능.
/// </summary>
public class TimerLureThrowable : NoiseLureThrowable, ITimerAdjustable
{
    [Header("Timer Settings")]
    [SerializeField] private float noiseDelay = 3f;

    private const float MinDelay = 1f;
    private const float MaxDelay = 10f;

    private float _adjustedDelay;

    [Header("Timer Display")]
    [SerializeField] private TextMeshPro timerText;

    [Header("Audio")]
    [SerializeField] private AudioClip timerTickClip;

    private bool _isArmed;

    protected override void Start()
    {
        base.Start();

        _adjustedDelay = Mathf.Clamp(Mathf.Round(noiseDelay), MinDelay, MaxDelay);
        SetTimerTextVisible(false);
    }

    // ITimerAdjustable: 마우스 스크롤로 딜레이 조절
    public void AdjustDelay(float amount)
    {
        if (!_isHeld || _isArmed)
            return;

        _adjustedDelay = Mathf.Clamp(_adjustedDelay + amount, MinDelay, MaxDelay);
        UpdateTimerText();
    }

    protected override void OnPickedUp()
    {
        if (_isArmed && !_noiseActive)
            CancelNoiseTimer();

        SetTimerTextVisible(true);
        UpdateTimerText();
    }

    protected override void OnThrown()
    {
        SetTimerTextVisible(false);
        ArmNoiseTimer();
    }

    protected override void StopNoise()
    {
        base.StopNoise();
        _isArmed = false;
    }

    private void UpdateTimerText()
    {
        if (timerText == null)
            return;

        timerText.text = $"{_adjustedDelay:0}s";
    }

    private void SetTimerTextVisible(bool visible)
    {
        if (timerText == null)
            return;

        timerText.gameObject.SetActive(visible);
    }

    private void ArmNoiseTimer()
    {
        if (_isArmed || _noiseActive)
            return;

        _isArmed = true;

        if (_noiseRoutine != null)
            StopCoroutine(_noiseRoutine);

        _noiseRoutine = StartCoroutine(NoiseTimerRoutine());
    }

    private void CancelNoiseTimer()
    {
        _isArmed = false;

        if (_noiseRoutine != null)
        {
            StopCoroutine(_noiseRoutine);
            _noiseRoutine = null;
        }
    }

    private IEnumerator NoiseTimerRoutine()
    {
        yield return new WaitForSeconds(_adjustedDelay);

        if (!_isArmed)
        {
            _noiseRoutine = null;
            yield break;
        }

        StartNoise();
        yield return new WaitForSeconds(noiseDuration);
        StopNoise();
        _noiseRoutine = null;

        Destroy(gameObject);
    }
}
