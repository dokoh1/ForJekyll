using UnityEngine;

public class MonsterSoundHandler : MonoBehaviour
{
    public MonsterSoundSystem soundSystem;

    public void PlayStepSound(MonsterState curState)
    {
        soundSystem?.PlayStepSound(curState);
    }

    public void PlaySoundByKey(MonsterSoundKey key)
    {
        soundSystem?.PlaySoundByKey(key, false);
    }

    public void PlaySoundByKeySetLoop(MonsterSoundKey key)
    {
        soundSystem?.PlaySoundByKey(key, true);
    }
}