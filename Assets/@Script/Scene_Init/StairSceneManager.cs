using System.Collections.Generic;
using System.Linq;
using NPC;
using Sirenix.OdinInspector;
using UnityEngine;

public class StairSceneManager : MonoBehaviour
{
    public static StairSceneManager Instance;
    
    [Title("NPC List")]
    public SerializableDictionary<NpcName, NpcUnit> npcUnits;
    
    [BoxGroup("Chapter List", ShowLabel = true, CenterLabel = true)]
    [TabGroup("Chapter List/Tabs", "Chapter")] public SerializableDictionary<SceneEnum, ChapterBase> chapterList;
    
    [BoxGroup("Chapter List", ShowLabel = true, CenterLabel = true)] 
    [TabGroup("Chapter List/Tabs", "Chapter")] public List<MapData> mapList;
        
    [BoxGroup("Script List", ShowLabel = true, CenterLabel = true)]
    [TabGroup("Script List/Tabs", "Script")] public NPCPostionManager psManager;
    [TabGroup("Script List/Tabs", "Script")] public LightSystem lightSystem;
    //[TabGroup("Script List/Tabs", "Script")] public GSceneEventManager gSceneEventManager;
    
    private void Awake()
    {
        Instance = this;
        
        psManager ??=GetComponent<NPCPostionManager>();
        lightSystem ??=GetComponent<LightSystem>();
        lightSystem.InitializeLightSystem();
        //gSceneEventManager ??=GetComponent<GSceneEventManager>();
        //gSceneEventManager.Initialize();
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
        };

        var sceneMap = new Dictionary<ScenarioAchieve, SceneEnum>
        {
            { ScenarioAchieve.Tutorial, SceneEnum.Tutorial },
        };

        foreach (var achieve in scenarioOrder)
        {
            if (ScenarioManager.Instance.GetAchieve(achieve)) continue;
            currentScene = sceneMap[achieve];
            break;
        }
        
        foreach (var c in chapterList)
        {
            if (currentScene != null && c.Key == currentScene.Value) c.Value.Initialize();
        }

        foreach (var m in mapList.Where(m => currentScene != null && m.currentScene == currentScene.Value))
        {
            DisableObject(m.mapObject);
        }
    }

    private void DisableObject(List<GameObject> objs)
    {
        foreach (var go in objs)
        {
            go?.SetActive(false);
        }
    }
}
