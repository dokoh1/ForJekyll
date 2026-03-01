using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public enum FadeState
{
    FadeIn,
    FadeOut,
}
public class FadeEffect : MonoBehaviour
{
    [SerializeField]
    [Range(0.01f, 10f)]
    private float fadeTime;
    public bool slowFade { get; set; }
    public float slowFadeTime = 0.5f;

    [SerializeField]
    private AnimationCurve fadeCurve;
    public Image image;
    private FadeState fadeState;
    private Sequence sequence;
    
    void Awake()
    {
        image = GetComponent<Image>();
        image.gameObject.SetActive(false);
    }

    public async Awaitable UseFadeEffect(FadeState state, Action onComplete = null)
    {
        image.gameObject.SetActive(true);
        
        fadeState = state;
        DOTween.Kill(image);
        sequence?.Kill();
        
        sequence = DOTween.Sequence();
        
        sequence.SetUpdate(true); // UI 쪽 버그로 인해 시퀸스가 TimeScale의 영향을 받지 않게 수정 #쿠크냐#
        
        sequence.AppendCallback(() => image.enabled = true);

        float time = slowFade ? slowFadeTime : fadeTime;

        switch (state)
        {
            case FadeState.FadeIn:
                sequence.Append(image.DOFade(1f, 0f));
                sequence.Append(image.DOFade(0f, time));
                break;
            case FadeState.FadeOut:
                sequence.Append(image.DOFade(0f, 0f));
                sequence.Append(image.DOFade(1f, time));
                break;
        }
        
        if (onComplete != null)
            sequence.AppendCallback(onComplete.Invoke);

        await sequence.AsyncWaitForCompletion();
    }
}