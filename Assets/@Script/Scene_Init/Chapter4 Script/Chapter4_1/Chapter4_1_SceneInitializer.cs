using UnityEngine;

public class Chapter4_1_SceneInitializer : ChapterBase
{
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private float flashlightIntensity = 30f;
    public override void SceneClear() { ScenarioManager.Instance.SetAchieve(ScenarioAchieve.Chapter4_1, true); }
    [SerializeField] private SceneEnum nextScene = SceneEnum.Chapter4_2;
    private Chapter4_1_Manager _ch41Manager;
    public override void Initialize()
    {
        Debug.Log("SceneInitializer 실행됨");
        _ch41Manager = new Chapter4_1_Manager();
        GameManager.Instance.MovePlayerTransform(playerSpawnPoint);
        GameManager.Instance.Player.PlayerAngle(playerSpawnPoint.eulerAngles.y);
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.PlayerMove, true);
        GameManager.Instance.Player.Flash.SetIntensityOverride(flashlightIntensity);
    }

    private void OnDisable()
    {
        GameManager.Instance?.Player?.Flash?.ResetIntensity();
    }
    public void OnMidBossBreakDoor()
    {
        // 1) 챕터 클리어 처리
        SceneClear();
        // 2) 페이드 + 다음 씬 이동
        GameManager.Instance.fadeManager.MoveScene(nextScene);
    }
}
