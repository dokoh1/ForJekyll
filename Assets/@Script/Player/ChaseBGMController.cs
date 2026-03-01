using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ChaseBGMController : MonoBehaviour
{
    private AudioSource _chaseBGM;
    private float fadeTime = 2f;
    private float stopDelay = 0.5f;
    private float removalGrace = 0.8f;

    private readonly HashSet<int> _active = new();
    private readonly Dictionary<int, Tween> _pendingRemovals = new();

    private Tween _fadeTween;
    private Tween _stopDelayTween;

    public int CurrentCount => _active.Count;

    private void Awake()
    {
        if (!_chaseBGM) _chaseBGM = GetComponent<AudioSource>();
        _chaseBGM.volume = 0f;
    }

    private void OnDisable()
    {
        _fadeTween?.Kill();
        _stopDelayTween?.Kill();

        foreach (var kv in _pendingRemovals) kv.Value?.Kill();
        _pendingRemovals.Clear();
        _active.Clear();
        if (_chaseBGM && _chaseBGM.isPlaying) _chaseBGM.Stop();
    }

    public void BeginChase(MonoBehaviour chaser)
    {
        if (!chaser) return;
        int id = chaser.GetInstanceID();

        if (_pendingRemovals.TryGetValue(id, out var t))
        {
            t.Kill();
            _pendingRemovals.Remove(id);
        }

        bool wasZero = _active.Count == 0;
        if (_active.Add(id) && wasZero)
            FadeInBGM();
    }

    public void EndChase(MonoBehaviour chaser)
    {
        if (!chaser) return;
        int id = chaser.GetInstanceID();
        if (_pendingRemovals.ContainsKey(id)) return;

        var tw = DOVirtual.DelayedCall(removalGrace, () =>
        {
            _pendingRemovals.Remove(id);
            if (_active.Remove(id) && _active.Count == 0)
                FadeOutBGMWithDelay();
        }, ignoreTimeScale: false)
        .SetUpdate(false)
        .SetRecyclable(true)
        .SetLink(gameObject, LinkBehaviour.KillOnDestroy);

        _pendingRemovals[id] = tw;
    }

    public void ClearAll()
    {
        foreach (var kv in _pendingRemovals) kv.Value?.Kill();
        _pendingRemovals.Clear();
        bool hadAny = _active.Count > 0;
        _active.Clear();
        if (hadAny) FadeOutBGMWithDelay();
    }

    private void FadeInBGM()
    {
        _stopDelayTween?.Kill(); 
        _stopDelayTween = null;
        if (!_chaseBGM.isPlaying) { _chaseBGM.volume = 0f; _chaseBGM.Play(); }

        _fadeTween?.Kill();
        _fadeTween = _chaseBGM.DOFade(1f, fadeTime)
            .SetEase(Ease.InOutSine)
            .SetUpdate(false)
            .SetRecyclable(true)
            .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
    }

    private void FadeOutBGMWithDelay()
    {
        _stopDelayTween?.Kill();
        _stopDelayTween = DOVirtual.DelayedCall(stopDelay, () =>
        {
            _fadeTween?.Kill();
            _fadeTween = _chaseBGM.DOFade(0f, fadeTime)
                .SetEase(Ease.InOutSine)
                .SetUpdate(false)
                .SetRecyclable(true)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                .OnComplete(() =>
                {
                    if (_chaseBGM.isPlaying) _chaseBGM.Stop();
                });
        }, ignoreTimeScale: false)
        .SetUpdate(false)
        .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
    }
}