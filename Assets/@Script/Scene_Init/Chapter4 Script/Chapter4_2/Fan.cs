using DG.Tweening;
using UnityEngine;

public class Fan : MonoBehaviour
{
    [SerializeField] private DOTweenAnimation fanOn;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip fanSound;

    public void Play()
    {
        fanOn?.DOPlayForward();

        if (audioSource && fanSound)
        {
            audioSource.clip = fanSound;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    public void Stop()
    {
        fanOn?.DORewind();
        audioSource?.Stop();
    }
}
