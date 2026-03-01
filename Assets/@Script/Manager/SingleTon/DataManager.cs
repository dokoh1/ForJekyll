using System;
using System.Collections.Generic;
using ModestTree;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class DataManager : SingletonManager<DataManager>
{
    public event Action<Settings> OnSetting;

    [TabGroup("Tab", "Manager", SdfIconType.GearFill, TextColor = "orange")]
    [TabGroup("Tab", "Manager")] public SceneDataManager sceneDataManager;
    [TabGroup("Tab", "Manager")] public UI_SavePanel UISavePanel;

    [Header("다이얼로그 관련")] 
    [SerializeField] private SerializableDictionary<string, string> NPC_Name;
    [field: SerializeField] public SerializableDictionary<string, NPCData> NPCs { get; set; }
    [field: SerializeField] public SerializableDictionary<string, Color> NPC_Colors { get; set; }
    [field: SerializeField] public SerializableDictionary<string, int> NPC_Favor { get; set; }
    [field: SerializeField] public SerializableDictionary<string, bool> NPC_Alive { get; set; }
    [SerializeField] private SerializableDictionary<int, Sprite> backgrounds;
    [SerializeField] private SerializableDictionary<string, bool> scenarioData;
    [SerializeField] private VoiceData voiceData;
    [field: SerializeField] public Vector2 LastMainDialogue { get; set; } = Vector2.zero;
    public Dictionary<(int, int), int> MainSelects = new();

    [Header("설정")]
    [SerializeField] public Settings settings;
    [SerializeField] private LocalizationSettings localizationSettings;

    [Header("카르마 관련")]
    private int _karma;
    public event Action<int> OnKarmaChanged;

    private string localeSetting = String.Empty;



    private Locale locale(string code) =>
        localizationSettings.GetAvailableLocales().Locales.Find(l => l.Identifier.Code == code);

    public string LocaleSetting
    {
        get
        {
            if (localeSetting.IsEmpty())
            {
                localeSetting = GetSystemLocale;
                localizationSettings.SetSelectedLocale(locale(localeSetting));
            }

            return localizationSettings.GetSelectedLocale().Identifier.Code;
        }

        set
        {
            localeSetting = value;
            localizationSettings.SetSelectedLocale(locale(localeSetting));
            UIManager.Instance.view.RefreshLog();
        } 
    }

    public string GetSystemLocale
    {
        get
        {
            return Application.systemLanguage switch
            {
                SystemLanguage.Korean => "ko",
                SystemLanguage.English => "en",
                SystemLanguage.Japanese => "ja",
                SystemLanguage.ChineseSimplified => "zh_Hans",
                SystemLanguage.ChineseTraditional => "zh_Hant",
                SystemLanguage.Russian => "ru",
                _ => "en"
            };
        }
    }


    [Header("스토리 관련")]
    [field: SerializeField] public int Chapter { get; set; }
    
    public void SavePanelOff()
    {
        UISavePanel.ClosePanel();
    }
    public int Karma
    {
        get => _karma;
        set
        {
            if (_karma == value) return;
            _karma = value;
            OnKarmaChanged?.Invoke(_karma);
        }
    }

    public void SavePanelOn()
    {
        UISavePanel.OpenPanel();
    }

    protected override void Awake()
    {
        base.Awake();
        if (sceneDataManager == null) sceneDataManager = GetComponent<SceneDataManager>();
        if (UISavePanel == null) UISavePanel = GetComponentInChildren<UI_SavePanel>();
    }

    private void Start()
    {
        // 현재 추가해둔 NPC Data 기반으로 호감도 저장을 위한 Dictionary 생성
        foreach (var item in NPCs)
        {
            NPC_Favor.Add(item.Key, 0);
            NPC_Alive.Add(item.Key, true);
        }
    }

    public void ChangeCondition(string key, bool value)
    {
        if (scenarioData.ContainsKey(key))
            scenarioData[key] = value;
        else
            Debug.LogError($"ScenarioData에 변수명 : {key}가 존재하지 않습니다.");
    }

    public bool CheckCondition(string key)
    {
        if (key.ToLower() == "normal")
            return true;
        
        if (scenarioData.ContainsKey(key))
            return scenarioData[key];
        else
            return false;
    }
    
    public bool GetNpcName(string nameKey, out string name) => NPC_Name.TryGetValue(nameKey, out name);
    
    public bool GetNpcFace(string nameKey, string faceKey, out Sprite sprite)
    {
        sprite = null;
        
        if (NPCs.ContainsKey(nameKey))
        {
            sprite = NPCs[nameKey].GetFace(faceKey);
            return NPCs.ContainsKey(nameKey);
        }

        return false;
    }

    public Color GetNPCColor(string name)
    {
        if (GetNpcName(name, out string npcName))
            name = npcName;
        return NPC_Colors.TryGetValue(name, out Color color) ? color : Color.white;
    }

    public bool GetBackground(int key, out Sprite sprite) => backgrounds.TryGetValue(key, out sprite);

    public void ChangeFontSize(float fontSize)
    {
        settings.FontSize = fontSize;
        OnSetting?.Invoke(settings);
    }

    public void ChangePlaySpeed(float playSpeed)
    {
        settings.PlaySpeed = playSpeed;
        OnSetting?.Invoke(settings);
    }

    public void LoadSetting() => OnSetting?.Invoke(settings);
    
    public AudioClip LoadVoice(int chapter, int index)
    {
        if (TimeLiner.Instance.CurrentDialogueType != Dialogue.Main || !voiceData.CheckContainKey(new Vector2(chapter, index), out var soundKey)|| soundKey.IsNullOrWhitespace())
            return null;
        
        return voiceData.GetSound(soundKey);
    }

    public void FavorEvent(NpcName npcName) => ScenarioManager.Instance.npcFavorInteract[npcName] = true;

    public void SaveMainIndex()
    {
        LastMainDialogue = new Vector2(Chapter, TimeLiner.Instance.CurrentIndex);
    }
    
    public void AddSelectData(int index)
    {
        if (TimeLiner.Instance.CurrentDialogueType == Dialogue.Main)
        {
            if (MainSelects.ContainsKey((Chapter, TimeLiner.Instance.CurrentIndex)))
                MainSelects[(Chapter, TimeLiner.Instance.CurrentIndex)] = index;
            else 
                MainSelects.Add((Chapter, TimeLiner.Instance.CurrentIndex), index);
        }
    }
    
    public int GetSelectData(int chapter, int index)
    {
        return MainSelects.ContainsKey((chapter, index)) ? MainSelects[(chapter, index)] : 0;
    }
}

[Serializable]
public class Settings
{
    [field:SerializeField] public float PlaySpeed { get; set; }
    [field:SerializeField] public float FontSize { get; set; }
}