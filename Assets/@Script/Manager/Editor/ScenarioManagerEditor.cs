using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ScenarioManager))]
public class ScenarioManagerEditor : Editor
{
    // 시나리오 도달값들이 인스펙터에서 수정 가능한지 안한지 체크하는 bool
    private bool readOnly = true;

    public override void OnInspectorGUI()
    {
        ScenarioManager scenarioManager = (ScenarioManager)target;

        readOnly = EditorGUILayout.ToggleLeft("ReadOnly", readOnly);
        EditorGUILayout.Space(10);

        // 스위치 리스트의 크기를 Enum의 크기에 맞추기
        int achieveCount = System.Enum.GetValues(typeof(ScenarioAchieve)).Length;
        if (scenarioManager.scenarioArchieves == null || scenarioManager.scenarioArchieves.Count != achieveCount)
        {
            scenarioManager.scenarioArchieves = new List<bool>(new bool[achieveCount]);
        }

        int playerAchieveCount = System.Enum.GetValues(typeof(PlayerAchieve)).Length;
        if (scenarioManager.playerArchieves == null || scenarioManager.playerArchieves.Count != playerAchieveCount)
        {
            scenarioManager.playerArchieves = new List<bool>(new bool[playerAchieveCount]);
        }

        serializedObject.Update();

        // 모든 ScenarioAchieve enum 값을 순회하면서 인스펙터에 표시
        for (int i = 0; i < achieveCount; i++)
        {
            ScenarioAchieve achieveType = (ScenarioAchieve)i;

            if(readOnly)
                EditorGUILayout.Toggle(achieveType.ToString(), scenarioManager.scenarioArchieves[i]);
            else
                scenarioManager.SetAchieve(achieveType, EditorGUILayout.Toggle(achieveType.ToString(), scenarioManager.scenarioArchieves[i]));
        }

        for (int i = 0; i < playerAchieveCount; i++)
        {
            PlayerAchieve playerType = (PlayerAchieve)i;

            if (readOnly)
                EditorGUILayout.Toggle(playerType.ToString(), scenarioManager.playerArchieves[i]);
            else
                scenarioManager.SetAchieve(playerType, EditorGUILayout.Toggle(playerType.ToString(), scenarioManager.playerArchieves[i]));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
