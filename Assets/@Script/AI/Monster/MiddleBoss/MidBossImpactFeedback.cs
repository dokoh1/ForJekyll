using System.Collections;
using Beautify.Universal;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class MidBossImpactFeedback : MonoBehaviour
{
    // ── Camera Shake ──
    [Header("Camera Shake")]
    [SerializeField] private bool enableCameraShake = true;
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private float shakeForce = 3f;

    // ── Hit Stop ──
    [Header("Hit Stop")]
    [SerializeField] private bool enableHitStop = true;
    [SerializeField] private float hitStopDuration = 0.1f;

    // ── Screen Effects (Beautify3) ──
    [Header("Screen Effects (Beautify3)")]
    [SerializeField] private bool enableScreenEffects = true;
    [SerializeField] private float chromaticPeak = 0.1f;
    [SerializeField] private float vignettePeak = 1f;
    [SerializeField] private float screenEffectDuration = 0.5f;

    // ── Flashlight Flicker ──
    [Header("Flashlight Flicker")]
    [SerializeField] private bool enableFlashlightFlicker = true;
    [SerializeField] private float flickerDuration = 1.2f;
    [SerializeField] private int flickerCount = 9;

    private Tweener _chromaticTween;
    private Tweener _vignetteTween;

    public void Play(Vector3 hitPoint, float intensity)
    {
        Debug.Log($"[ImpactFeedback] Play() called - hitPoint:{hitPoint}, intensity:{intensity}");

        if (enableCameraShake)       ShakeCamera(intensity);
        if (enableHitStop)           StartCoroutine(HitStopRoutine(intensity));
        if (enableScreenEffects)     ScreenImpactEffect(intensity);
        if (enableFlashlightFlicker) FlashlightFlicker(intensity);
    }

    #region Camera Shake

    private void ShakeCamera(float intensity)
    {
        if (impulseSource == null) return;
        impulseSource.GenerateImpulse(shakeForce * intensity);
    }

    #endregion

    #region Hit Stop

    private IEnumerator HitStopRoutine(float intensity)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(hitStopDuration * intensity);
        Time.timeScale = 1f;
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }

    #endregion

    #region Screen Effects (Beautify3)

    private void ScreenImpactEffect(float intensity)
    {
        var b = BeautifySettings.settings;
        if (b == null) return;

        _chromaticTween?.Kill();
        _vignetteTween?.Kill();

        float chromTarget = chromaticPeak * intensity;
        float vigTarget = vignettePeak * intensity;

        b.chromaticAberrationIntensity.value = chromTarget;
        b.vignettingOuterRing.value = vigTarget;

        _chromaticTween = DOTween.To(
            () => b.chromaticAberrationIntensity.value,
            v => b.chromaticAberrationIntensity.value = v,
            0f, screenEffectDuration
        ).SetUpdate(true).SetEase(Ease.OutQuad);

        _vignetteTween = DOTween.To(
            () => b.vignettingOuterRing.value,
            v => b.vignettingOuterRing.value = v,
            0f, screenEffectDuration
        ).SetUpdate(true).SetEase(Ease.OutQuad);
    }

    #endregion

    #region Flashlight Flicker

    private void FlashlightFlicker(float intensity)
    {
        var flash = GameManager.Instance?.Player?.Flash;
        if (flash == null) return;

        int count = Mathf.Max(1, Mathf.RoundToInt(flickerCount * intensity));
        flash.Flicker(flickerDuration * intensity, count);
    }

    #endregion
}
