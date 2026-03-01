using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class OutdoorSound : MonoBehaviour
{
    public AudioSource audioSource;
    public bool isPlay = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
                audioSource.volume = 1;
                isPlay = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
                audioSource.volume = 0;
                isPlay = false;
            }
        }
    }

    public void PlaySound()
    {
        if (isPlay)
        {
            audioSource.Play();
            audioSource.volume = 1;
        }
    }

}