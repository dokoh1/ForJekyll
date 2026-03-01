using Sirenix.OdinInspector;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public enum LightName
{
    Use_Y_Lights,
    Use_W_Lights,
    Use_Room_Lights,

    Unknown,

    None_Y_Lights,
    None_W_Lights,
    None_Room_Lights,
}
public enum Floor
{
    Under,
    Lobby,
    F2, F3, F4, F5
}
public class LightManager : MonoBehaviour
{
    public class FloorElements
    {
        public List<Light> lights = new List<Light>();
        public List<MeshRenderer> renderers = new List<MeshRenderer>();
    }

    [TitleGroup("LightManager", "MonoBehaviour", alignment: TitleAlignments.Centered, horizontalLine: true, boldTitle: true, indent: false)]

    [Title("Lights & MeshRenderer Dictionary")]
    [ShowInInspector, DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.Foldout)]
    public Dictionary<Floor, FloorElements> elementsForFloors = new Dictionary<Floor, FloorElements>();

    public void AddLightToFloor(Floor floor, Light light)
    {
        if (!elementsForFloors.ContainsKey(floor))
        {
            elementsForFloors[floor] = new FloorElements();
        }
        elementsForFloors[floor].lights.Add(light);
    }

    public void AddRendererToFloor(Floor floor, MeshRenderer renderer)
    {
        if (!elementsForFloors.ContainsKey(floor))
        {
            elementsForFloors[floor] = new FloorElements();
        }
        elementsForFloors[floor].renderers.Add(renderer);
    }
    public List<Light> GetLightsForFloor(Floor floor)
    {
        return elementsForFloors.ContainsKey(floor) ? elementsForFloors[floor].lights : new List<Light>();
    }

    public List<MeshRenderer> GetRenderersForFloor(Floor floor)
    {
        return elementsForFloors.ContainsKey(floor) ? elementsForFloors[floor].renderers : new List<MeshRenderer>();
    }

    [TabGroup("Light", "UseLights", SdfIconType.Palette, TextColor = "yellow")]
    [TabGroup("Light", "UseLights")][SerializeField] Material Use_Y_Lights;
    [TabGroup("Light", "UseLights")][SerializeField] Material Use_W_Lights;
    [TabGroup("Light", "UseLights")][SerializeField] Material Use_Room_Lights;


    [TabGroup("Light", "NoneLights", SdfIconType.Palette, TextColor = "white")]
    [TabGroup("Light", "NoneLights")][SerializeField] Material None_Y_Lights;
    [TabGroup("Light", "NoneLights")][SerializeField] Material None_W_Lights;
    [TabGroup("Light", "NoneLights")][SerializeField] Material None_Room_Lights;

    private Dictionary<string, LightName> materailLightName;

    private void Start()
    {
        materailLightName = new Dictionary<string, LightName>()
        {
              { Use_W_Lights.name, LightName.Use_W_Lights},
              { Use_Y_Lights.name, LightName.Use_Y_Lights},
              { Use_Room_Lights.name, LightName.Use_Room_Lights},

              { "None_Y_Lights", LightName.None_Y_Lights},
              { "None_W_Lights", LightName.None_W_Lights},
              { "None_Room_Lights", LightName.None_Room_Lights}
        };
    }
    public void OffListLight(List<Light> lights)
    {
        foreach (var light in lights)
        {
            light.enabled = false;
        }
    } // 빛 끄기

    public void OnListLight(List<Light> lights)
    {
        foreach (var light in lights)
        {
            light.enabled = true;
        }
    } // 빛 켜기

    public void OffChangeMaterial(List<MeshRenderer> meshRenderers)
    {
        foreach (var renderer in meshRenderers)
        {
            if (renderer == null)
            {
                continue;
            }
            Material[] newMaterial = renderer.materials;
            for (int i = 0; i < newMaterial.Length; i++)
            {
                string materialName = newMaterial[i].name.Replace(" (Instance)", "");
                LightName newName = GetMaterialType(materialName);
                switch (newName)
                {
                    case LightName.Use_Y_Lights:
                        newMaterial[i] = None_Y_Lights;
                        break;
                    case LightName.Use_W_Lights:
                        newMaterial[i] = None_W_Lights;
                        break;
                    case LightName.Use_Room_Lights:
                        newMaterial[i] = None_Room_Lights;
                        break;
                    default: break;
                }
                renderer.materials = newMaterial;
            }
        }
    } // Material 끄기

    public void OnChangeMaterial(List<MeshRenderer> meshRenderers)
    {
        foreach (var renderer in meshRenderers)
        {
            if (renderer == null)
            {
                continue;
            }
            Material[] newMaterial = renderer.materials;
            for (int i = 0; i < newMaterial.Length; i++)
            {
                string materialName = newMaterial[i].name.Replace(" (Instance)", "");
                LightName newName = GetMaterialType(materialName);
                switch (newName)
                {
                    case LightName.None_Y_Lights:
                        newMaterial[i] = Use_Y_Lights;
                        break;
                    case LightName.None_W_Lights:
                        newMaterial[i] = Use_W_Lights;
                        break;
                    case LightName.None_Room_Lights:
                        newMaterial[i] = Use_Room_Lights;
                        break;
                    default: break;
                }
                renderer.materials = newMaterial;
            }
        }
    } // Material 켜기

    LightName GetMaterialType(string materialName)
    {
        if (materailLightName.TryGetValue(materialName, out LightName materialType))
        {
            return materialType;
        }
        return LightName.Unknown;
    } // Material 이름 가져오기


    [Title("Floor")]
    public List<Transform> floorTopTransforms;
    public List<Transform> floorBottomTransforms;

    public void SortFloorTransform()
    {
        if (floorTopTransforms.Count > 3)
        {
            floorTopTransforms.Sort((a, b) => a.position.y.CompareTo(b.position.y));
        }

        if (floorBottomTransforms.Count > 3)
        {
            floorBottomTransforms.Sort((a, b) => a.position.y.CompareTo(b.position.y));
        }
    }
    public void DeleteFloorTransform()
    {
        if (floorTopTransforms != null)
        {
            floorTopTransforms.Clear();
        }

        if (floorBottomTransforms != null)
        {
            floorBottomTransforms.Clear();
        }
    }

    public Floor ReturnFloorPosition(Vector3 targetPos)
    {
        int floor = 0;

        for (int i = 0; i < floorTopTransforms.Count; i++)
        {
            if (targetPos.y >= floorBottomTransforms[i].position.y && targetPos.y <= floorTopTransforms[i].position.y)
            {
                floor = i;
                break;
            }
        }
        return (Floor)floor;
    }

    public void OffAllLight()
    {
        var floors = new[] { Floor.Under, Floor.Lobby, Floor.F2, Floor.F3, Floor.F4, Floor.F5 };
        foreach (var floor in floors)
        {
            var lightManager = GameManager.Instance.lightManager;
            lightManager.OffListLight(lightManager.GetLightsForFloor(floor));
            lightManager.OffChangeMaterial(lightManager.GetRenderersForFloor(floor));
        }
    }

    public void OnAllLight()
    {
        var floors = new[] { Floor.Under, Floor.Lobby, Floor.F2, Floor.F3, Floor.F4, Floor.F5 };
        foreach (var floor in floors)
        {
            var lightManager = GameManager.Instance.lightManager;
            lightManager.OnListLight(lightManager.GetLightsForFloor(floor));
            lightManager.OnChangeMaterial(lightManager.GetRenderersForFloor(floor));
        }
    }

    public void FloorLightReset()
    {
        elementsForFloors.Clear();
        DeleteFloorTransform();
    }
}


    
