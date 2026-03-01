using UnityEngine;

public class Chapter4_2_Manager : SceneServiceLocator
{
    private GameObject followMonster;
    private AudioSource DoorAudio;
    private AudioSource MonsterAudio;
    private AudioClip DoorCrashClip;

    public void ObjectInitialize(GameObject followMonster, AudioSource DoorAudio, AudioSource MonsterAudio,
        AudioClip DoorCrashClip)
    {
        this.followMonster = followMonster;
        this.DoorAudio = DoorAudio;
        this.MonsterAudio = MonsterAudio;
        this.DoorCrashClip = DoorCrashClip;
    }
}
