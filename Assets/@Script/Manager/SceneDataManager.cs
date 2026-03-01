using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class DataToSave
{
    public List<bool> ScenarioBoolList;
    public int Chapter;
    public int Karma;
    public int SceneEnum;
    public Dictionary<string, int> npcFavor;
    public string saveTxt;
    public Dictionary<NpcName, bool> npcFavorBool;
    public Vector2 lastDialogueIndex;
    public Dictionary<(int, int), int> MainSelects;
    public DataToSave()
    {
        ScenarioBoolList = new List<bool>(ScenarioManager.Instance.scenarioArchieves);
        Chapter = DataManager.Instance.Chapter;
        Karma = DataManager.Instance.Karma;
        SceneEnum = (int)GameManager.Instance.fadeManager.currentScene;
        saveTxt = DataManager.Instance.sceneDataManager.nowSaveTxt;
        npcFavorBool = new Dictionary<NpcName, bool>(ScenarioManager.Instance.npcFavorInteract);
        lastDialogueIndex = DataManager.Instance.LastMainDialogue;
        MainSelects = DataManager.Instance.MainSelects;
    }
}
public class SceneDataManager : MonoBehaviour
{
    public SerializableDictionary<SceneEnum, Sprite> sceneImages = new();
    
    [SerializeField] private string FileName = "ForJekyll_SaveData";
    [SerializeField] private string Key = "Aya";

    [Header("현재 저장 위치")]
    public int nowSaveDataNum;
    public int nowLoadDataNum;
    public string nowSaveTxt;
    public TextMeshProUGUI nowTextMeshPro;

    public void DataSave()
    {
        string nowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        nowTextMeshPro.text = $"Chapter.{DataManager.Instance.Chapter} / {nowTime}";
        nowSaveTxt = nowTextMeshPro.text;
        
        DataToSave dataToSave = new DataToSave();
        string filePath = $"{FileName}{nowSaveDataNum}.txt";
        ES3.Save(Key, dataToSave, filePath);  
    }

    public void DataLoad()
    {
        DataToSave data = DataLoad(nowLoadDataNum);
        UIManager.Instance.DialogueCloseImmediate();

        if (data != null)
        {
            DataManager.Instance.Chapter = data.Chapter;
            DataManager.Instance.Karma = data.Karma;
            ScenarioManager.Instance.scenarioArchieves = data.ScenarioBoolList;
            SceneEnum scene = (SceneEnum)data.SceneEnum;
            ScenarioManager.Instance.npcFavorInteract = data.npcFavorBool;
            GameManager.Instance.fadeManager.MoveScene(scene);
            DataManager.Instance.LastMainDialogue = data.lastDialogueIndex;
            DataManager.Instance.MainSelects = data.MainSelects;
            TimeLiner.Instance.LoadPrevLogs();
        }
        else
        {
            DataManager.Instance.Chapter = 0;
            DataManager.Instance.Karma = 0;
            ScenarioManager.Instance.scenarioArchieves = new List<bool>();
            GameManager.Instance.fadeManager.MoveScene(SceneEnum.Tutorial);
            DataManager.Instance.LastMainDialogue = Vector2.zero;
            DataManager.Instance.MainSelects = new();
        }

        PlayerAchieve[] statesToDisable =
        {
            PlayerAchieve.PlayerMove,
            PlayerAchieve.PlayerStop,
            PlayerAchieve.Dialogue,
            PlayerAchieve.OnlyInteractLocked,
            PlayerAchieve.CutScenePlaying,
            PlayerAchieve.Saving,
            PlayerAchieve.EscMenu,
            PlayerAchieve.LookAndInteract
        };

        foreach (var state in statesToDisable)
        {
            ScenarioManager.Instance.SetAchieve(state, false);
        }
    }

    public DataToSave DataLoad(int num)
    {
        string filePath = $"{FileName}{num}.txt";

        if (ES3.FileExists(filePath))
        {
            return ES3.Load<DataToSave>(Key, filePath);
        }
        else
        {
            return null;
        }
    }
    public void DataDelete(int num)
    {
        string filePath = $"{FileName}{num}.txt";
        ES3.DeleteFile(filePath);
    }
    #region 오토세이브, 로드
    
    [ContextMenu("Auto Save Data")]
    public void AutoDataSave(int num)
    {
        string nowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        nowSaveTxt = $"Chapter.{DataManager.Instance.Chapter} / {nowTime}";
        
        DataToSave dataToSave = new DataToSave();
        string filePath = $"{FileName}{num}.txt";
        ES3.Save(Key, dataToSave, filePath);
        
        Debug.Log("Auto Save Data");
    }

    [ContextMenu("Auto Load Data")]
    public void AutoDataLoad()
    {
        nowLoadDataNum = 10;
        DataLoad();
    }

    #endregion
}
