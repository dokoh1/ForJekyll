using DG.Tweening;
using Item;
using UnityEngine;

public class NoiseFirePlug : NoiseObject
{
    [field: Header("Door")]
    [field: SerializeField] public bool IsOpen { get; set; }
    [SerializeField] private DOTweenAnimation openAnimation;
    [SerializeField] private DOTweenAnimation closeAnimation;

    public override void Interact()
    {
        base.Interact();

        if (!IsOpen)
        {
            // 소화전 열리는 로직
            //Debug.Log("NoiseFirePlug - open");
            IsOpen = true;
            PlayNoise(TapNoiseTimer, TapSoundData, tapAS);
            openAnimation.DOKill();
            openAnimation.CreateTween(true);
        }
        else
        {
            // 소화전 닫히는 로직
            //Debug.Log("NoiseFirePlug - close");
            IsOpen = false;
            PlayNoise(TapNoiseTimer, TapSoundData, tapAS, 1);
            closeAnimation.DOKill();
            closeAnimation.CreateTween(true);
        }
    }
}