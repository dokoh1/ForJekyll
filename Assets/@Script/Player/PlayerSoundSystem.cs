using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundSystem : SerializedMonoBehaviour
{
    public AudioSource[] audioSources;

    private PlayerEnum.Grounds CurGround { get; set; }
    private Collider _lastCollider;
    private readonly Dictionary<Collider, PlayerEnum.Grounds> _colliderToGround = new();

    [OdinSerialize, ShowInInspector]
    public Dictionary<PlayerEnum.Grounds, Dictionary<PlayerEnum.PlayerState, SoundData>> StepGroundDataMap = new();

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        var go = hit.collider.gameObject;

        if (hit.collider == _lastCollider) return;
        _lastCollider = hit.collider;

        if (_colliderToGround.TryGetValue(hit.collider, out var cached))
        {
            SetGroundIfChanged(cached);
            return;
        }

        PlayerEnum.Grounds g = PlayerEnum.Grounds.Untagged;
        if (go.CompareTag("Concrete")) g = PlayerEnum.Grounds.Concrete;
        else if (go.CompareTag("Shaft")) g = PlayerEnum.Grounds.Shaft;
        else if (go.CompareTag("Wet")) g = PlayerEnum.Grounds.Wet;

        if (g != PlayerEnum.Grounds.Untagged)
        {
            _colliderToGround[hit.collider] = g;
            SetGroundIfChanged(g);
        }
        else SetGroundIfChanged(PlayerEnum.Grounds.Concrete);
    }

    private void SetGroundIfChanged(PlayerEnum.Grounds next)
    {
        if (CurGround == next) return;
        CurGround = next;
    }

    public void PlayStepSound(PlayerEnum.PlayerState curState)
    {
        var state = CheckPlayerState(curState);
        if (TryGetSoundData(CurGround, state, out var data) ||
            TryGetSoundData(PlayerEnum.Grounds.Concrete, state, out data))
        {
            PlayStepData(data);
        }
    }

    private bool TryGetSoundData(PlayerEnum.Grounds g, PlayerEnum.PlayerState st, out SoundData data)
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
        if (src == null || src.isPlaying) return;
        var clips = data.noises;
        if (clips == null || clips.Length == 0) return;

        int i = Random.Range(0, clips.Length);
        float jit = Random.Range(-0.03f, 0.03f);

        src.volume = data.volume + jit;
        src.pitch = data.pitch + jit;
        src.clip = clips[i];
        src.Play();
    }

    private PlayerEnum.PlayerState CheckPlayerState(PlayerEnum.PlayerState st)
    {
        if (((int)st & (int)PlayerEnum.PlayerState.Run) != 0) return PlayerEnum.PlayerState.Run;
        if (((int)st & (int)PlayerEnum.PlayerState.Walk) != 0) return PlayerEnum.PlayerState.Walk;
        if (((int)st & (int)PlayerEnum.PlayerState.Crouch) != 0) return PlayerEnum.PlayerState.Crouch;
        return PlayerEnum.PlayerState.Walk;
    }

    public void StopAllSound()
    {
        if (audioSources == null) return;
        foreach (var src in audioSources)
        {
            if (src != null && src.isPlaying) src.Stop();
        }
    }

    public void SetIgnorePause(bool ignore)
    {
        if (audioSources == null) return;
        foreach (var src in audioSources)
        {
            if (src != null) src.ignoreListenerPause = ignore;
        }
    }
}