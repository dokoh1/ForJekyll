using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerFearEffect : MonoBehaviour
{
    public PlayerDetectSystem detectSystem;
    public AudioSource heartbeatAS;
    public AudioClip audioClip;
    public SphereCollider sphereCollider;
    [field: SerializeField] private Material HorrorGlitch { get; set; }
    [field: SerializeField] private Volume Volume { get; set; }

    private float _maxDistance;
    private Transform _currentTarget;
    private Coroutine _updateEffectCoroutine;

    private readonly WaitForSeconds _waitTime = new WaitForSeconds(0.2f);

    private void Start()
    {
        _maxDistance = sphereCollider.radius * 0.25f;
        heartbeatAS.clip = audioClip;
        detectSystem.OnTargetChanged += OnTargetChanged;
        detectSystem.OnTargetNull += OnTargetNull;
    }

    private void OnTargetChanged(Transform newTarget)
    {
        if (newTarget == null)
        {
            OnTargetNull();
        }
        else if(newTarget != _currentTarget)
        {
            _currentTarget = newTarget;
            StartFearEffect();
        }
    }

    private void OnTargetNull()
    {
        _currentTarget = null;
        StopFearEffect();
    }

    private void StartFearEffect()
    {
        StartUpdateCoroutine();
    }

    private void StopFearEffect()
    {
        StopUpdateCoroutine();
        EffectReset();
    }

    private void StartUpdateCoroutine()
    {
        StopUpdateCoroutine();
        _updateEffectCoroutine = StartCoroutine(UpdateEffectRoutine());
    }

    private void StopUpdateCoroutine()
    {
        if (_updateEffectCoroutine != null)
        {
            StopCoroutine(_updateEffectCoroutine);
            _updateEffectCoroutine = null;
        }
    }

    private IEnumerator UpdateEffectRoutine()
    {
        while (true)
        {
            float distance = Vector3.Distance(transform.position, _currentTarget.position);
            //Debug.Log(distance);
            UpdateAudio(distance);
            //UpdatePostProcessing(distance);
            yield return _waitTime;
        }
    }

    private void UpdateAudio(float value)
    {
        float distance = value;
        float intensity = Mathf.InverseLerp(_maxDistance, 3f, distance);
        if (heartbeatAS != null && audioClip != null)
        {
            if (!heartbeatAS.isPlaying) heartbeatAS.Play();
            heartbeatAS.volume = intensity;
        }
    }

    //private void UpdatePostProcessing(float value)
    //{
    //    float distance = value;
    //    float distanceRtio = Mathf.InverseLerp(_maxDistance, 0f, distance);
    //    float vignetteIntensity = Mathf.Lerp(0f, 0.40f, distanceRtio);
    //    //float noiseAmountIntensity = Mathf.Lerp(0f, 0.45f, distanceRtio);
    //    float strengthIntensity = Mathf.Lerp(0f, 30f, distanceRtio);
    //    float lineStrengthIntensity = Mathf.Lerp(1f, 0.5f, distanceRtio);
    //    _vignette.intensity.value = vignetteIntensity;
    //    //HorrorGlitch.SetFloat("_NoiseAmount", noiseAmountIntensity);
    //    HorrorGlitch.SetFloat("_Strength", strengthIntensity);
    //    HorrorGlitch.SetFloat("_LineStrength", lineStrengthIntensity);
    //}

    public void EffectReset()
    {
        heartbeatAS.Stop();
        //_vignette.intensity.value = 0f;
        //HorrorGlitch.SetFloat("_NoiseAmount", 0f);
        //HorrorGlitch.SetFloat("_Strength", 0f);
        //HorrorGlitch.SetFloat("_LineStrength", 1f);
    }

    private void OnDestroy()
    {
        EffectReset();
    }

}
