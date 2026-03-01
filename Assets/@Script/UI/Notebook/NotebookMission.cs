using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class NotebookMission : MonoBehaviour
{
    [SerializeField] private UIScriptData scriptData;

    [Header("Current Mission")]
    [SerializeField] private TextMeshProUGUI currentMission;
    [SerializeField] private TextMeshProUGUI currentMissionDetail;
    [SerializeField] private TextMeshProUGUI currentMissionWhy;


    [Header("Past Mission")]
    [SerializeField] private Transform pastMissionPrefabs;
    [SerializeField] private GameObject pastMissionPrefab;
    private const int maxPastQuests = 5;

    //인게임 언어 변경시 자동으로 언어를 변환하기 위한 변수
    private string lastQuestId;

    private void OnEnable()
    {
        if (!string.IsNullOrEmpty(lastQuestId))
            UpdateMission(lastQuestId);
    }


    public void UpdateMission(string questId)
    {
        lastQuestId = questId;
        currentMission.text = scriptData.GetText(questId, ScriptDataType.Quest);
        UpdateMissionDetail();
        UpdateMissionWhy();
        UpdatePastMission();
    }

    private void UpdateMissionDetail()
    {
        currentMissionDetail.text = scriptData.GetText(lastQuestId, ScriptDataType.QuestDetail);
    }

    private void UpdateMissionWhy()
    {
        currentMissionWhy.text = scriptData.GetText(lastQuestId, ScriptDataType.QuestWhy);
    }


    private void UpdatePastMission()
    {
        foreach (Transform child in pastMissionPrefabs)
        {
            Destroy(child.gameObject);
        }

        List<string> pastIds = scriptData.GetIdsAbove(lastQuestId, ScriptDataType.Quest, maxPastQuests);

        foreach (string questId in pastIds)
        {
            GameObject PMInfoObj = Instantiate(pastMissionPrefab, pastMissionPrefabs);
            PMInfoObj.GetComponentInChildren<TextMeshProUGUI>().text =
                scriptData.GetText(questId, ScriptDataType.PastQuest);
        }
    }
}
