using System;
using UnityEngine;

namespace Item
{
    [Serializable]
    public class ObjectSoundData
    {
        public string tag;
        public AudioClip[] noises;
        public float volume;
        public float pitch;
        public float noiseAmount;
    }
}


