using UnityEngine;

public class DeadBGMController : MonoBehaviour
{
    public AudioSource _audioSource;

    public void PlayBGM()
    {
        _audioSource.Play();
    }

    void OnDisable()
    {
        if (_audioSource.isPlaying) _audioSource.Stop();
    }
}