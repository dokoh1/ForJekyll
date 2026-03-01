using System.Collections.Generic;
using System.Linq;
using DG.Tweening.Core.Easing;
using NPC;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
#endif

public class ConforenceScene_Manager : MonoBehaviour
{
    
    private static ConforenceScene_Manager _instance;
    public static ConforenceScene_Manager Instance
    {
        get
        {
            _instance ??= FindAnyObjectByType<ConforenceScene_Manager>();
            return _instance;
        }
    }

    [Title("NPC List")]
    public SerializableDictionary<NpcName, NpcUnit> npcUnits;

    [TabGroup("Script List/Tabs", "Script")]
    public NPCPostionManager psManager;

    [TabGroup("Tab", "Manager", SdfIconType.GearFill, TextColor = "orange")]
    [TabGroup("Tab", "Manager")] public BossBattleScript bossBattleScript;
    
    [BoxGroup("Chapter List", ShowLabel = true, CenterLabel = true)]
    [TabGroup("Chapter List/Tabs", "Chapter")] public SerializableDictionary<SceneEnum, ChapterBase> chapterList;
    
    [BoxGroup("HotelPrefabs List", ShowLabel = true, CenterLabel = true)] 
    [TabGroup("HotelPrefabs List/Tabs", "HotelPrefabs")] public SerializableDictionary<SceneEnum, GameObject> hotelPrefabs;
    
    [Header("PlayerObj")]
    [SerializeField] public GameObject playerChair;
    [SerializeField] private GameObject bossBattleTimeLine;

    [Header("Boss And Chapter3_3 Choice")] 
    [SerializeField] private bool bossCutScene;
    [SerializeField] private bool Chapter3_3Scene;
    private void Awake()
    {
        if (bossCutScene)
        {
            if (bossBattleScript == null) 
             bossBattleScript = GetComponent<BossBattleScript>();
        }
        if (Chapter3_3Scene)
            psManager ??= GetComponent<NPCPostionManager>();
    }

    private void Initialize()
    {
        if (!Chapter3_3Scene)
            return;
        var npcArray = npcUnits.Select(npc => npc.Value).ToList();
        psManager.Initialize(npcArray);
        
        GameManager.Instance.playerSettingManager.ResetBool();

        SceneEnum? currentScene = null;
        
        var scenearioOrder = new[]
        {
            ScenarioAchieve.Chapter3_3,
        };

        var sceneMap = new Dictionary<ScenarioAchieve, SceneEnum>
        {
            {ScenarioAchieve.Chapter3_3, SceneEnum.Chapter3_3 }
        };

        foreach (var achieve in scenearioOrder)
        {
            if (ScenarioManager.Instance.GetAchieve(achieve))
            {
                continue;
            }
            currentScene = sceneMap[achieve];
            Debug.Log(currentScene);
            break;
        }
        
        foreach (var c in chapterList)
        {
            if (currentScene != null && c.Key == currentScene.Value)
            {
                c.Value.Initialize();
            }
        }

        foreach (var g in hotelPrefabs)
        {
            g.Value.SetActive(currentScene != null && g.Key == currentScene.Value);
        }

    }

    private void Start()
    {
        Initialize();
        if (bossCutScene)
        {
#if UNITY_EDITOR
        ScenarioManager.Instance.SetAchieve(ScenarioAchieve.PlayerFlashLight, true);
        ScenarioManager.Instance.SetAchieve(ScenarioAchieve.PSU_Crazy, true);
#endif
        bossBattleScript.StartBossBattle();
        }

    }
    public void StartBossBattleTimeLine()
    {
        //GameManager.Instance.Player.Flash.FlashLightOff();
        if (GameManager.Instance.Player.CurState.HasFlag(PlayerEnum.PlayerState.Flash)) GameManager.Instance.Player.SetFlash();
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.CutScenePlaying, true);
        bossBattleTimeLine.SetActive(true);
    }
}
