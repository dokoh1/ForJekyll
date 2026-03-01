using System.Collections.Generic;
using UnityEngine;

public class FootstepsSystem : MonoBehaviour
{
    [field: SerializeField] public PlayerEnum.Grounds CurGround { get; private set; }
    [field: SerializeField] public Player Player { get; private set; }

    public List<SoundData> stepSoundList;
    public AudioSource stepAS;
    public AudioSource otherAS;

    private int _stepSoundsIndex = 0;

    private Dictionary<int, PlayerEnum.Grounds> _groundMappings = new Dictionary<int, PlayerEnum.Grounds>
    {
        { "Concrete".GetHashCode(), PlayerEnum.Grounds.Concrete },
        { "Wet".GetHashCode(), PlayerEnum.Grounds.Wet },
        { "Shaft".GetHashCode(), PlayerEnum.Grounds.Shaft },
    };

    private Dictionary<PlayerEnum.PlayerState, string> _stateMappings = new Dictionary<PlayerEnum.PlayerState, string>
    {
        { PlayerEnum.PlayerState.Walk, "Walk" },
        { PlayerEnum.PlayerState.Run, "Run" },
        { PlayerEnum.PlayerState.Crouch, "Crouch" }
    };

    //private void OnControllerColliderHit(ControllerColliderHit hit)
    //{
    //    int tagHash = hit.collider.tag.GetHashCode();
    //    if (_groundMappings.TryGetValue(tagHash, out var ground)) CurGround = ground;
    //}

    //public void PlayStepSound(PlayerEnum.PlayerState curstate)
    //{
    //    if (stepAS.isPlaying || CurGround == PlayerEnum.Grounds.Untagged) return;

    //    foreach (var key in _stateMappings.Keys)
    //    {
    //        if (curstate.HasFlag(key) && _stateMappings.TryGetValue(key, out string stateValue))
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
        if (_stepSoundsIndex >= tempDatas.noises.Length) _stepSoundsIndex = 0;
        stepAS.clip = tempDatas.noises[_stepSoundsIndex];
        stepAS.volume = tempDatas.volume;
        stepAS.pitch = tempDatas.pitch;
        stepAS.Play();

        Player.CurNoiseAmount = Mathf.Min(Player.CurNoiseAmount + tempDatas.noiseAmount, Player.SumNoiseAmount);
        _stepSoundsIndex = (_stepSoundsIndex + 1) % tempDatas.noises.Length;
    }

    private SoundData FindSoundData(string tag)
    {
        SoundData tempDatas = stepSoundList.Find(sound => sound.tag == tag);

        if (tempDatas == null) Debug.LogError($"사운드 데이터 없음 {tag}");
        return tempDatas;
    }
}
