using ModestTree;
using System.Collections;
using UnityEngine;

public class PowerRoomAudioTrigger : MonoBehaviour
{
    public AudioSource[] speakers;
    public float insideVolume = 0.2f;  
    public float outsideVolume = 1.0f; 

    private bool _inside = false;

    private Coroutine _fadeCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_inside)
        {
            Debug.Log("Trigger in");
            _inside = true;
            if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = StartCoroutine(FadeVolume(insideVolume));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && _inside)
        {
            Debug.Log("Trigger out");
            _inside = false;
            if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = StartCoroutine(FadeVolume(outsideVolume));
        }
    }

    private IEnumerator FadeVolume(float targetVolume)
    {
        float duration = 0.2f;  
        float time = 0f;

        float[] startVolumes = new float[speakers.Length];
        for (int i = 0; i < speakers.Length; i++)
        {
            if (speakers[i] != null)
                startVolumes[i] = speakers[i].volume;
        }

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            for (int i = 0; i < speakers.Length; i++)
            {
                if (speakers[i] == null) continue;
                speakers[i].volume = Mathf.Lerp(startVolumes[i], targetVolume, t);
            }

            yield return null;
        }

        for (int i = 0; i < speakers.Length; i++)
        {
            if (speakers[i] == null) continue;
            speakers[i].volume = targetVolume;
        }

        Debug.Log("Fade complete");
    }
}