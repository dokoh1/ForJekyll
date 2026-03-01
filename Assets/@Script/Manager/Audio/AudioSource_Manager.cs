using UnityEngine;
public enum AudioType
{
    Master,
    SE,
    BackGround,
    Voice
}

public class AudioSource_Manager : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioType audioType;
    

    private void Awake( )
    {
        if (source == null) source = GetComponent<AudioSource>();

        switch (audioType)
        {
            case AudioType.SE:
                source.outputAudioMixerGroup = SoundManager.Instance.SE_Group; break;
            case AudioType.BackGround:
                source.outputAudioMixerGroup = SoundManager.Instance.BGM_Group; break;
            case AudioType.Voice:
                source.outputAudioMixerGroup = SoundManager.Instance.VOICE_Group; break;
            default:
                source.outputAudioMixerGroup = SoundManager.Instance.Master_Group; break;
        }
    }
}
