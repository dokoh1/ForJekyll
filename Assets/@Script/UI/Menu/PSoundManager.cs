using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using static Define;

public enum ESound
{
    Music,
    SFX,
    Voice,
    End,
}

public class PSoundManager
{
    private readonly AudioSource[] _audioSources = new AudioSource[(int)ESound.End];
    private readonly Dictionary<string, AudioClip> _audioClips = new();

    private GameObject _soundRoot;

    public void Init(AudioMixer mixer)
    {
        if (_soundRoot == null)
        {
            _soundRoot = GameObject.Find("@SoundRoot");
            if (_soundRoot == null)
            {
                _soundRoot = new GameObject { name = "@SoundRoot" };
                UnityEngine.Object.DontDestroyOnLoad(_soundRoot);

                string[] soundTypeNames = Enum.GetNames(typeof(ESound));
                for (int count = 0; count < soundTypeNames.Length - 1; count++)
                {
                    GameObject go = new GameObject { name = soundTypeNames[count] };
                    _audioSources[count] = go.AddComponent<AudioSource>();
                    go.transform.parent = _soundRoot.transform;

                    _audioSources[count].ignoreListenerPause = true;

                    switch ((ESound)count)
                    {
                        case ESound.Music:
                            _audioSources[count].outputAudioMixerGroup = mixer.FindMatchingGroups("Music")[0];
                            _audioSources[count].loop = true;
                            break;
                        case ESound.SFX:
                            _audioSources[count].outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
                            break;
                        case ESound.Voice:
                            _audioSources[count].outputAudioMixerGroup = mixer.FindMatchingGroups("Voice")[0];
                            break;
                    }
                }

                _audioSources[(int)ESound.Music].loop = true;
            }
        }
    }

    public void Clear()
    {
        foreach (AudioSource audioSource in _audioSources)
            audioSource.Stop();
        _audioClips.Clear();
    }

    public void Play(ESound type)
    {
        AudioSource audioSource = _audioSources[(int)type];
        audioSource.Play();
    }

    public void Play(ESound type, string key, float pitch = 1.0f, float volume = 1.0f)
    {
        AudioSource audioSource = _audioSources[(int)type];

        if (type == ESound.Music)
        {
            LoadAudioClip(key, (audioClip) =>
            {
                if (audioSource.isPlaying)
                    audioSource.Stop();

                audioSource.clip = audioClip;
                //if (Managers.Game.BGMOn)
                audioSource.Play();
            });
        }
        else
        {
            LoadAudioClip(key, (audioClip) =>
            {
                audioSource.pitch = pitch;
                audioSource.volume = volume;
                //if (Managers.Game.EffectSoundOn)
                audioSource.PlayOneShot(audioClip);
            });
        }
    }

    public void Play(ESound type, AudioClip audioClip, float pitch = 1.0f)
    {
        AudioSource audioSource = _audioSources[(int)type];

        if (type == ESound.Music)
        {
            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.clip = audioClip;
            //if (Managers.Game.BGMOn)
            audioSource.Play();
        }
        else
        {
            audioSource.pitch = pitch;
            //if (Managers.Game.EffectSoundOn)
            audioSource.PlayOneShot(audioClip);
        }
    }

    public void Stop(ESound type)
    {
        AudioSource audioSource = _audioSources[(int)type];
        audioSource.Stop();
    }

    private void LoadAudioClip(string key, Action<AudioClip> callback)
    {
        if (_audioClips.TryGetValue(key, out AudioClip audioClip))
        {
            callback?.Invoke(audioClip);
            return;
        }

        //audioClip = AssetManager.Instance.Load<AudioClip>(key);

        _audioClips.TryAdd(key, audioClip);

        callback?.Invoke(audioClip);
    }
}
