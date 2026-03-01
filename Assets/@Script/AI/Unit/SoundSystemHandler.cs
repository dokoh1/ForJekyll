using NPC;
using UnityEngine;

public class SoundSystemHandler : MonoBehaviour
{
    public CharacterSoundSystem soundSystem;

    public void PlayStepSound(NPCState curState)
    {
        soundSystem?.PlayStepSound(curState);
    }

    public void PlaySoundByKey(NpcSoundKey key)
    {
        soundSystem?.PlaySoundByKey(key);
    }
}