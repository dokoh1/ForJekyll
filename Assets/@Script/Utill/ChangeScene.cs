using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

public class ChangeScene : EditorWindow
{
    private FadeManager fadeManager => GameManager.Instance.fadeManager;
    private static string previousScenePath;
    private SceneEnum selectedSceneEnum;
    private int selectedChapter;
    
    [MenuItem("Tools/RuntimeSceneChange")]
    public static void ShowWindow()
    {
        GetWindow<ChangeScene>("ChangeScene");
    }

    private void StartFromMainMenu()
    {
        if (EditorBuildSettings.scenes.Length <= 0)
        {
            Debug.LogError("BuildSetting에 씬이 없습니다.");
            return;
        }

        string firstScenePath = EditorBuildSettings.scenes[0].path;

        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            // 현재 씬 경로 저장
            previousScenePath = SceneManager.GetActiveScene().path;

            // 플레이 모드 종료 시 원래 씬으로 복구하는 콜백 등록
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            // 0번째 씬 열고 플레이 시작
            EditorSceneManager.OpenScene(firstScenePath);
            EditorApplication.isPlaying = true;
        }
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode && !string.IsNullOrEmpty(previousScenePath))
        {
            // update 루프에 등록 (delayCall보다 더 안전)
            EditorApplication.update += RestoreSceneOnNextUpdate;

            // 이벤트 해제
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }
    }
    
    private static void RestoreSceneOnNextUpdate()
    {
        // 반드시 update 한 번만 실행되도록 제거
        EditorApplication.update -= RestoreSceneOnNextUpdate;

        if (!EditorApplication.isPlaying && !string.IsNullOrEmpty(previousScenePath))
        {
            Debug.Log($"[FastStart] 이전 씬 복구: {previousScenePath}");
            EditorSceneManager.OpenScene(previousScenePath);
            previousScenePath = null;
        }
    }
    
    private void OnGUI()
    {
        if (!Application.isPlaying)
        {
            if (GUILayout.Button("빠른 시작", GUILayout.Width(100), GUILayout.Height(50)))
            {
                StartFromMainMenu();
            }
            return;
        }

        GUILayout.Label("챕터 이동", EditorStyles.boldLabel);

        selectedSceneEnum = (SceneEnum)EditorGUILayout.EnumPopup("챕터 선택", selectedSceneEnum);
        selectedChapter = (int)EditorGUILayout.IntField("챕터 입력", selectedChapter);

        if (GUILayout.Button("선택 완료"))
        {
            MoveSceneWithAchieves(selectedSceneEnum);
        }
    }
    
    private void MoveSceneWithAchieves(SceneEnum sceneEnum)
    {
        ScenarioManager.Instance.SetAchieve(ScenarioAchieve.PlayerFlashLight, true);

        var target = sceneEnum;

        var belowEnums = Enum.GetValues(typeof(SceneEnum))
            .Cast<SceneEnum>()
            .Where(e => (int)e < (int)target)
            .ToList();

        foreach (var e in belowEnums)
        {
            if (Enum.TryParse<ScenarioAchieve>(e.ToString(), out var achieveEnum))
            {
                ScenarioManager.Instance.SetAchieve(achieveEnum, true);
            }
        }

        fadeManager.MoveScene(sceneEnum);
        DataManager.Instance.Chapter = selectedChapter;
    }
}
#endif