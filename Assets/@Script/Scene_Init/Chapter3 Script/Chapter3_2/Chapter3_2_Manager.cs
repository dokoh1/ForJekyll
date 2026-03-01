using UnityEngine;

public class Chapter3_2_Manager : MonoBehaviour
{
    private readonly Chapter3_2_SceneInitializer _ch32;
    public Chapter3_2_Manager(Chapter3_2_SceneInitializer ch32) { _ch32 = ch32; }

    [Header("Inject obj")]
    private GameObject TimeLineRoofTopDoor;
    private GameObject playerUI;
    private EyeTypeMonster[] eyeTypeMonsters;
    private Transform VRcam;
    private ConferenceRoomDoor_Obj conferenceRoomDoor;

    public void ObjectInitialize(GameObject TimeLineRoofTopDoor, GameObject playerUI, EyeTypeMonster[] eyeTypeMonsters, Transform VRcam, ConferenceRoomDoor_Obj conferenceRoomDoor)
    {
        this.TimeLineRoofTopDoor = TimeLineRoofTopDoor;
        this.playerUI = playerUI;
        this.eyeTypeMonsters = eyeTypeMonsters;
        this.VRcam = VRcam;
        this.conferenceRoomDoor = conferenceRoomDoor;
    }

    public void TimeLineStart()
    {
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.CutScenePlaying, true);
        GameManager.Instance.fadeManager.FadeStart(FadeState.FadeIn);
        playerUI.SetActive(false);
        //gameManager.Player.Flash.FlashLightOff();
        if (GameManager.Instance.Player.CurState.HasFlag(PlayerEnum.PlayerState.Flash)) GameManager.Instance.Player.SetFlash();
        TimeLineRoofTopDoor.SetActive(true);
    }

    public void TimeLineDialogueStart()
    {
        UIManager.Instance.DialogueOpen(Dialogue.Main, false, 56);
    }
    public void TimeLineMonsterSpawn()
    {
        GameManager.Instance.MovePlayerTransform(VRcam);

        foreach (var monster in eyeTypeMonsters)
        {
            monster.gameObject.SetActive(true);
            monster.MonsterWork(true);
        }
        SoundManager.Instance.PlayBGM(SoundManager.Instance.BGM_Source, BGM_Sound.Day1Follow, true, 0.5f, 0, false);
    }

    public void TimeLinePlayerSettingTrue()
    {
        playerUI.SetActive(true);
        conferenceRoomDoor.IsInteractable = true;
        conferenceRoomDoor.AddMaterial();
        if (!GameManager.Instance.Player.CurState.HasFlag(PlayerEnum.PlayerState.Flash)) GameManager.Instance.Player.SetFlash();
        //gameManager.Player.Flash.FlashLighOn();
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.CutScenePlaying, false);
    }
}
