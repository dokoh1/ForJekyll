using System;
using UnityEngine;
[Serializable]
public class SoundData
{
    public string tag;
    public AudioClip[] noises;
    public AudioSource audioSource;
    public float volume;
    public float pitch;
    public float noiseAmount;
}

