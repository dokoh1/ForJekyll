using NUnit.Framework;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSoundSystem : SerializedMonoBehaviour
{
    public AudioSource[] audioSources;

    private UnitEnum.Grounds CurGround { get; set; }
    private Collider _lastCollider;
    private readonly Dictionary<Collider, UnitEnum.Grounds> _colliderToGround = new();

    [OdinSerialize, ShowInInspector]
    public Dictionary<UnitEnum.Grounds, Dictionary<MonsterState, SoundData>> StepGroundDataMap = new();

    [OdinSerialize, ShowInInspector]
    public Dictionary<MonsterSoundKey, SoundData> OtherDataMap = new();

    private void OnTriggerEnter(Collider other)
    {
        var go = other.gameObject;

        if (other == _lastCollider) return;
        _lastCollider = other;

        if (_colliderToGround.TryGetValue(other, out var cached))
        {
            SetGroundIfChanged(cached);
            return;
        }

        UnitEnum.Grounds g = UnitEnum.Grounds.Untagged;
        if (go.CompareTag("Concrete")) g = UnitEnum.Grounds.Concrete;
        else if (go.CompareTag("Shaft")) g = UnitEnum.Grounds.Shaft;
        else if (go.CompareTag("Wet")) g = UnitEnum.Grounds.Wet;

        if (g != UnitEnum.Grounds.Untagged)
        {
            _colliderToGround[other] = g;
            SetGroundIfChanged(g);
        }
        else SetGroundIfChanged(UnitEnum.Grounds.Concrete);
    }

    private void SetGroundIfChanged(UnitEnum.Grounds next)
    {
        if (CurGround == next) return;
        CurGround = next;
    }

    public void PlayStepSound(MonsterState curState)
    {
        var state = CheckState(curState);
        if (TryGetStepSoundData(CurGround, state, out var data) ||
            TryGetStepSoundData(UnitEnum.Grounds.Concrete, state, out data))
        {
            PlayStepData(data);
        }
    }

    private MonsterState CheckState(MonsterState st)
    {
        if (st == MonsterState.Run) return MonsterState.Run;
        if (st == MonsterState.Walk) return MonsterState.Walk;
        return MonsterState.Walk;
    }

    private bool TryGetStepSoundData(UnitEnum.Grounds g, MonsterState st, out SoundData data)
    {
        data = null;
        return StepGroundDataMap != null
            && StepGroundDataMap.TryGetValue(g, out var inner)
            && inner != null
            && inner.TryGetValue(st, out data)
            && data != null;
    }

    private void PlayStepData(SoundData data)
    {
        var src = data.audioSource;
        if (src == null) return;
        var clips = data.noises;
        if (clips == null || clips.Length == 0) return;
        if (src.isPlaying) src.Stop();

        int i = Random.Range(0, clips.Length);
        float jit = Random.Range(-0.005f, 0.005f);
        src.volume = data.volume + jit;
        src.pitch = data.pitch + jit;
        src.clip = clips[i];
        src.Play();
    }

    public void PlaySoundByKey(MonsterSoundKey key, bool isLoop)
    {
        if (OtherDataMap != null && OtherDataMap.TryGetValue(key, out var data) && data != null)
        {
            var src = data.audioSource;
            if (src == null) return;
            var clips = data.noises;
            if (clips == null || clips.Length == 0) return;

            int i = (clips.Length == 1) ? 0 : Random.Range(0, clips.Length);
            var clip = clips[i];
            if (clip == null) return;
            if (src.isPlaying) src.Stop();

            float jit = Random.Range(-0.03f, 0.03f);
            src.volume = data.volume + jit;
            src.pitch = data.pitch + jit;
            src.clip = clip;
            src.loop = isLoop;
            src.Play();
        }
    }

    public void StopAllSound()
    {
        if (audioSources == null) return;
        foreach (var src in audioSources)
        {
            if (src != null && src.isPlaying) src.Stop();
        }
    }
}