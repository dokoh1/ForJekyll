using System.Collections.Generic;
using System.Linq;
using NPC;
using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public struct NpcInteractBase
{
    public NpcName npcName;
    public Dialogue dialogueType;
    public int dialogueIndex;
    public bool is2D;
}

[System.Serializable]
public class MapData
{
    public SceneEnum currentScene;
    public List<GameObject> mapObject;
}
public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance;
    [Title("NPC List")]
    public SerializableDictionary<NpcName, NpcUnit> npcUnits;

    [BoxGroup("Chapter List", ShowLabel = true, CenterLabel = true)]
    [TabGroup("Chapter List/Tabs", "Chapter")] public SerializableDictionary<SceneEnum, ChapterBase> chapterList;

    [BoxGroup("Chapter List", ShowLabel = true, CenterLabel = true)]
    [TabGroup("Chapter List/Tabs", "Chapter")] public List<MapData> mapList;

    [BoxGroup("Script List", ShowLabel = true, CenterLabel = true)]
    [TabGroup("Script List/Tabs", "Script")] public NPCPostionManager psManager;
    [TabGroup("Script List/Tabs", "Script")] public LightSystem lightSystem;
    [TabGroup("Script List/Tabs", "Script")] public GSceneEventManager gSceneEventManager;

    private void Awake()
    {
        Instance = this;

        psManager ??= GetComponent<NPCPostionManager>();
        lightSystem ??= GetComponent<LightSystem>();
        lightSystem.InitializeLightSystem();
        gSceneEventManager ??= GetComponent<GSceneEventManager>();
        gSceneEventManager.Initialize();
    }
    private void Start() { Initialize(); }
    private void Initialize()
    {
        var npcArray = npcUnits.Select(npc => npc.Value).ToList();

        psManager.Initialize(npcArray);

        GameManager.Instance.playerSettingManager.ResetBool();

        SceneEnum? currentScene = null;

        var scenarioOrder = new[]
        {
            ScenarioAchieve.Tutorial,
            ScenarioAchieve.Chapter1_1,
            ScenarioAchieve.Chapter1_2,
            ScenarioAchieve.Chapter1_3,
            ScenarioAchieve.Chapter2_1,
            ScenarioAchieve.Chapter2_2,
            ScenarioAchieve.Chapter2_3,
            ScenarioAchieve.Chapter3_1,
            ScenarioAchieve.Chapter3_2,
            ScenarioAchieve.Chapter3_3,
            ScenarioAchieve.Chapter4_1,
            ScenarioAchieve.Chapter4_2,
            ScenarioAchieve.Chapter4_3,
            ScenarioAchieve.Chapter5_1,
            ScenarioAchieve.Chapter5_2,
        };

        var sceneMap = new Dictionary<ScenarioAchieve, SceneEnum>
        {
            { ScenarioAchieve.Tutorial, SceneEnum.Tutorial },
            { ScenarioAchieve.Chapter1_1, SceneEnum.Chapter1_1 },
            { ScenarioAchieve.Chapter1_2, SceneEnum.Chapter1_2 },
            { ScenarioAchieve.Chapter1_3, SceneEnum.Chapter1_3 },
            { ScenarioAchieve.Chapter2_1, SceneEnum.Chapter2_1 },
            { ScenarioAchieve.Chapter2_2, SceneEnum.Chapter2_2 },
            { ScenarioAchieve.Chapter2_3, SceneEnum.Chapter2_3 },
            { ScenarioAchieve.Chapter3_1, SceneEnum.Chapter3_1 },
            { ScenarioAchieve.Chapter3_2, SceneEnum.Chapter3_2 },
            { ScenarioAchieve.Chapter3_3, SceneEnum.Chapter3_3 },
            { ScenarioAchieve.Chapter4_1, SceneEnum.Chapter4_1 },
            { ScenarioAchieve.Chapter4_2, SceneEnum.Chapter4_2 },
            { ScenarioAchieve.Chapter4_3, SceneEnum.Chapter4_3 },
            { ScenarioAchieve.Chapter5_1, SceneEnum.Chapter5_1 },
            { ScenarioAchieve.Chapter5_2, SceneEnum.Chapter5_2 },
        };

        foreach (var achieve in scenarioOrder)
        {
            if (ScenarioManager.Instance.GetAchieve(achieve)) continue;
            currentScene = sceneMap[achieve];
            break;
        }

        foreach (var m in mapList.Where(m => m.currentScene == currentScene))
        {
            EnableObject(m.mapObject);
        }
        
        foreach (var c in chapterList)
        {
            if (currentScene != null && c.Key == currentScene)
            {
                Debug.Log(currentScene);
                Debug.Log(c.Key);
                c.Value.gameObject.SetActive(true);
                c.Value.Initialize();
                break;
            }
        }
    }

    private void EnableObject(List<GameObject> objs)
    {
        foreach (var go in objs)
        {
            go?.SetActive(true);
        }
    }
}
