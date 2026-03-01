using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public enum SE_Sound
{
    KarmaUp,
    KarmaDown,

    EarTypeJumpScare,
    EyeTypeJumpScare,
    
    AMB_Earthquake,
    FlashLightBlink,
    UI_RootSelect
}

public enum Obj_Sound
{
    ItemPickUp,
    BroadCastSound,
    SafeSuccess,
    SafeFailure,
    GeneratorSwitch,
    FireFlag,
}

public enum BGM_Sound
{
    MainMenu,
    FirstMeet,
    GunAfther,
    Day1Follow,
    Hotel_Talk_Loop,
    ALB_Hotel,
    SpeakerSound,
    Chapter1_2_ElevatorAfther,
}
public class SoundManager : SingletonManager<SoundManager>
{
    [TabGroup("Tab", "AudioSource", SdfIconType.Soundwave, TextColor = "red")]
    [TabGroup("Tab", "AudioSource")][SerializeField] public AudioSource BGM_Source;
    [TabGroup("Tab", "AudioSource")][SerializeField] public AudioSource subBGM_Source;
    [TabGroup("Tab", "AudioSource")][SerializeField] public AudioSource subSubBGM_Source;
    [TabGroup("Tab", "AudioSource")][SerializeField] public AudioSource SFX_Source;
    [TabGroup("Tab", "AudioSource")] [field:SerializeField] public AudioSource Voice_Source { get; set; }
    [TabGroup("Tab", "AudioSource")] [field:SerializeField] public AudioSource DialogueSE_Source { get; set; }


    [TabGroup("Tab", "AudioMixer", SdfIconType.Soundwave, TextColor = "orange")]
    [TabGroup("Tab", "AudioMixer")] public AudioMixerGroup Master_Group;
    [TabGroup("Tab", "AudioMixer")] public AudioMixerGroup BGM_Group;
    [TabGroup("Tab", "AudioMixer")] public AudioMixerGroup SE_Group;
    [TabGroup("Tab", "AudioMixer")] public AudioMixerGroup VOICE_Group;

    public SerializableDictionary<SE_Sound, AudioClip> SE_Dictionary;
    public SerializableDictionary<Obj_Sound, AudioClip> Obj_SDictionary;
    public SerializableDictionary<BGM_Sound, AudioClip> BGM_Dictionary;
    public bool Voice_Playing => Voice_Source.isPlaying;
    public float Voice_Length => Voice_Source.clip.length;

    public void PlaySE<T>(T enumVal, float volume = 0f) where T : Enum
    {
        switch (enumVal)
        {
            case SE_Sound se:
                SFX_Source.clip = SE_Dictionary[se];
                break;
            case Obj_Sound obj:
                SFX_Source.clip = Obj_SDictionary[obj];
                break;
            default:
                break;
        }
        
        if (volume != 0) SFX_Source.volume = volume;
        else SFX_Source.volume = 1;
        SFX_Source.Play();
    }
    
    public void PlayBGM<T>(AudioSource audio, T bgm, bool isFade, float vol, float time, bool coroutine)
    {
        AudioClip clip = GetAudioClip(bgm);
        if (clip == null) return;

        if (coroutine)
        {
            StartCoroutine(PlayBGM_LoopCor(audio, clip, isFade, vol, time));
            return;
        }
        audio.clip = clip;
        audio.volume = vol;
        audio.Play();
        if (!isFade) return;
        audio.volume = 0f;
        audio.DOFade(vol, 2f);
    }

    private AudioClip GetAudioClip<T>(T bgm)
    {
        if (bgm is AudioClip clip) return clip;
        if (bgm is BGM_Sound sound && BGM_Dictionary.TryGetValue(sound, out AudioClip foundClip)) return foundClip;
        return null;
    }

    public void StopAudioSource(AudioSource audioSource, bool isFade)
    {
        if (audioSource.isPlaying)
        {
            if (isFade)
            {
                audioSource.DOFade(0, 2f);
            }
            else audioSource.Stop();
        }
    }

    public void PlayDialogueSE()
    {
        AudioClip SE = DataManager.Instance.LoadVoice(DataManager.Instance.Chapter, TimeLiner.Instance.CurrentIndex);
        
        if (SE != null)
        {
            if (SE.length >= 10f)
            {
                switch (SE)
                {
                    case var _ when SE == BGM_Dictionary[BGM_Sound.GunAfther]:
                        PlayBGM(BGM_Source, SE, true, 0.1f, 18.23f, true);
                        break;
                    case var _ when SE == BGM_Dictionary[BGM_Sound.Hotel_Talk_Loop]:
                        PlayBGM(BGM_Source, SE, true, 0.5f, 13f, true);
                        break;
                }
            }
            else
            {
                DialogueSE_Source.PlayOneShot(SE);
            }
        }
        else
        {
            Debug.LogError("SE 불러오기 실패");
        }
    }
    
    public void PlayVoice()
    {
        AudioClip voice = null;
        if (TimeLiner.Instance.CurrentDialogueType == Dialogue.Main)
             voice = DataManager.Instance.LoadVoice(DataManager.Instance.Chapter, TimeLiner.Instance.CurrentIndex);
        
        if (voice != null)
        {
            Voice_Source.clip = voice;
            Voice_Source.Play();
        }
        else
        {
            Debug.Log("보이스 불러오기 실패");
        }
    }

    public void StopVoice()
    {
        if (Voice_Source.isPlaying)
            Voice_Source.Stop();
    }

    private IEnumerator PlayBGM_LoopCor(AudioSource audioSource, AudioClip clip, bool isFade, float vol, float time)
    {
        if (audioSource.isPlaying)
        {
            audioSource.DOFade(0, 2f);
            yield return new WaitForSeconds(2f); 
        }
  
        audioSource.clip = clip;
        audioSource.volume = 0;
        audioSource.loop = false;
        audioSource.Play();

       
        if (isFade)
        {
            audioSource.DOFade(vol, 2f);
        }
        else
        {
            audioSource.volume = vol;
        }

        float loopStartTime = Mathf.Clamp(time, 0, audioSource.clip.length - 0.01f);

        while (audioSource.clip == clip) 
        {
            yield return null; 

            if (!audioSource.isPlaying)
            {
                audioSource.Play();
                audioSource.time = loopStartTime;
            }
        }
    }

    public void StopAllBGM()
    {
        if (BGM_Source.isPlaying)
        {
            BGM_Source.DOFade(0, 2f);
            BGM_Source.clip = null;
        }

        if (subBGM_Source.isPlaying)
        {
            subBGM_Source.DOFade(0, 2f);
            subBGM_Source.clip = null;
        }
        
        if (subSubBGM_Source.isPlaying)
        {
            subSubBGM_Source.DOFade(0, 2f);
            subSubBGM_Source.clip = null;
        }
    }
}
