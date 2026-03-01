using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnitEnum;

public class UnitSoundSystem : MonoBehaviour
{
    [field: SerializeField] public Grounds CurGround { get; private set; }
    
    public List<SoundData> stepSoundList;
    public List<SoundData> otherSoundList;
    public AudioSource stepAS;
    public AudioSource otherAS;

    private Dictionary<string, Grounds> _groundMappings = new Dictionary<string, Grounds>
    {
        { "Concrete", Grounds.Concrete },
        { "Wet", Grounds.Wet },
        { "Shaft", Grounds.Shaft },
    };

    //private Dictionary<NPCState, string> _npcStateMappings = new Dictionary<NPCState, string>
    //{
    //    { NPCState.Run, "Run" },
    //    { NPCState.Crouch, "Crouch" },
    //    { NPCState.Move, "Walk" },
    //};

    private Dictionary<MonsterState, string> _monsterStateMappings = new Dictionary<MonsterState, string>
    {
        { MonsterState.Run, "Run" },
        { MonsterState.Walk, "Walk" },
    };

    private Coroutine _downVolumeCoroutine;

    public bool GroundChange { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"Collider : {other.tag}");

        if (_groundMappings.TryGetValue(other.tag, out var ground))
        {
            if (CurGround != ground) GroundChange = true;
            CurGround = ground;
        }
    }

    public void PlayStepSound(MonsterState curstate)
    {
        if (stepAS.isPlaying && !GroundChange) return;
        if (CurGround == Grounds.Untagged) return;
        GroundChange = false;

        foreach (MonsterState key in _monsterStateMappings.Keys)
        {
            if (curstate.HasFlag(key) && _monsterStateMappings.TryGetValue(key, out string stateValue))
            {
                string soundTag = $"{CurGround}{stateValue}";
                SoundData tempDatas = FindSoundData(soundTag);
                if (tempDatas != null) PlayClip(tempDatas);
                return;
            }
        }
    }

    //public void PlayStepSound(NPCState curstate)
    //{
    //    if (stepAS.isPlaying && !GroundChange) return;
    //    if (CurGround == Grounds.Untagged) return;
    //    GroundChange = false;

    //    foreach (NPCState key in _npcStateMappings.Keys)
    //    {
    //        if (curstate.HasFlag(key) && _npcStateMappings.TryGetValue(key, out string stateValue))
    //        {
    //            string soundTag = $"{CurGround}{stateValue}";
    //            SoundData tempDatas = FindSoundData(soundTag);
    //            if (tempDatas != null) PlayClip(tempDatas);
    //            return;
    //        }
    //    }
    //}

    private void PlayClip(SoundData tempDatas)
    {
        stepAS.loop = true;
        stepAS.clip = tempDatas.noises[0];
        stepAS.volume = tempDatas.volume;
        stepAS.pitch = tempDatas.pitch;
        stepAS.Play();
    }

    public void OtherSoundPlay(string tag, bool isLoop)
    {
        StopDownVolumeRoutine();
        SoundData tempData = null;
        tempData = FindOtherSoundData(tag);
        tempData.audioSource.clip = tempData.noises[0];
        tempData.audioSource.volume = tempData.volume;
        tempData.audioSource.pitch = tempData.pitch + (Random.Range(-0.1f, 0.1f));
        tempData.audioSource.loop = isLoop;
        tempData.audioSource.Play();
    }

    public void OtherSoundPlay(string tag)
    {
        StopDownVolumeRoutine();
        SoundData tempData = null;
        tempData = FindOtherSoundData(tag);
        tempData.audioSource.clip = tempData.noises[0];
        tempData.audioSource.pitch = 1f + (Random.Range(-0.2f, 0.2f));
        tempData.audioSource.Play();
    }

    public void PlaySoundData(SoundData soundData)
    {
        soundData.audioSource.clip = soundData.noises[0];
        soundData.audioSource.pitch = 1f + (Random.Range(-0.03f, 0.03f));
        soundData.audioSource.volume = 1f + (Random.Range(-0.03f, 0.03f));
        soundData.audioSource.Play();
    }

    public void OtherSoundPlay(SoundData soundData)
    {
        StopDownVolumeRoutine();
        soundData.audioSource.clip = soundData.noises[0];
        soundData.audioSource.pitch = 1f + (Random.Range(-0.03f, 0.03f));
        soundData.audioSource.volume = 1f + (Random.Range(-0.03f, 0.03f));
        soundData.audioSource.Play();
    }

    public void PlayAudioSource(AudioSource audio, string tag)
    {
        StopDownVolumeRoutine();
        SoundData tempData = null;
        tempData = FindOtherSoundData(tag);
        audio.clip = tempData.noises[0];
        audio.pitch = 1f + (Random.Range(-0.2f, 0.2f));
        audio.Play();
    }

    public void StopAudioSource(AudioSource audio)
    {
        _downVolumeCoroutine = StartCoroutine(DownVolumetRoutine(audio));
    }

    public void StopAudioSource(SoundData soundData)
    {
        _downVolumeCoroutine = StartCoroutine(DownVolumetRoutine(soundData.audioSource));
    }

    private IEnumerator DownVolumetRoutine(AudioSource audio)
    {
        StopDownVolumeRoutine();

        if (audio.volume > 0f)
        {
            audio.volume -= Time.deltaTime * 70f;
            yield return new WaitForSeconds(0.2f);

            if (audio.volume <= 0f)
            {
                StopDownVolumeRoutine();
                audio.Stop();
            }
        }        
    }

    private void StopDownVolumeRoutine()
    {
        if (_downVolumeCoroutine != null)
        {
            StopCoroutine(_downVolumeCoroutine);
            _downVolumeCoroutine = null;
        }
    }

    private SoundData FindSoundData(string tag)
    {
        SoundData tempData = stepSoundList.Find(sound => sound.tag == tag);
        if (tempData == null) Debug.LogError($"non data :  {tag}");
    
        return tempData;
    }

    public SoundData FindOtherSoundData(string tag)
    {
        SoundData tempData = null;

        for (int i = 0; i < otherSoundList.Count; i++)
        {
            if (otherSoundList[i].tag == tag) tempData = otherSoundList[i];
        }

        if (tempData == null) Debug.LogError("UnitSoundSystem - none data");

        return tempData;
    }

    public void StopStepAudio()
    {
        if (stepAS.isPlaying)
        {
            stepAS.Stop(); 
        }
    }

    public void StopOtherAudio()
    {
        if (otherAS.isPlaying)
        {
            otherAS.Stop();
        }
    }

    //public void PauseAudio()
    //{
    //    if (stepAS.isPlaying) stepAS.Pause();
    //    if (otherAS.isPlaying) otherAS.Pause();
    //}

    //public void ResumeAudio()
    //{
    //    if (!stepAS.isPlaying) stepAS.UnPause();
    //    if (!otherAS.isPlaying) otherAS.UnPause();
    //}

    //private void OnEnable()
    //{
    //    GameManager.Instance.OnTimeStop += PauseAudio;
    //    GameManager.Instance.OffTimeStop += ResumeAudio;
    //}

    //private void OnDisable()
    //{
    //    GameManager.Instance.OnTimeStop -= PauseAudio;
    //    GameManager.Instance.OffTimeStop -= ResumeAudio;
    //}
}
