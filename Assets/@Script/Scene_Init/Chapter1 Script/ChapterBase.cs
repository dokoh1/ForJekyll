using System.Collections.Generic;
using System.Linq;
using NPC;
using UnityEngine;

public enum MapType
{
    Hotel,
    Parking,
    Stair,
    Conforence,
}
public abstract class ChapterBase : MonoBehaviour, IChapter
{
    public abstract void Initialize();
    public abstract void SceneClear();

    public void InitializeInteractData(List<NpcInteractBase> npcInteracts)
    {
        SerializableDictionary<NpcName, NpcUnit> npcUnits = GameSceneManager.Instance.npcUnits;
        foreach (var n in npcUnits) { InitData(n.Key, n.Value, npcInteracts); }
    }

    private void InitData(NpcName npcName, NpcUnit unit ,List<NpcInteractBase> npcInteracts)
    {
        foreach (var nn in npcInteracts.Where(nn => npcName == nn.npcName))
        {
            unit.IsInteractable = true;
            unit.DialogNum = nn.dialogueIndex;
            unit.dialogue = nn.dialogueType;
            unit.is2D = nn.is2D;
                
            if(!unit.gameObject.activeInHierarchy) 
                unit.gameObject.SetActive(true);
        }
    }
}
