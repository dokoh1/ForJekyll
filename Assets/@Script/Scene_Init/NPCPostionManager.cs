using System.Collections.Generic;
using NPC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public class NPCPostionManager : MonoBehaviour
{
    [BoxGroup("PosManager", ShowLabel = true, CenterLabel = true)]
    [BoxGroup("PosManager")]
    [TabGroup("PosManager/Tabs", "Pos")] public SerializableDictionary<NpcName, NavMeshAgent> navMeshAgents = new();
    public void Initialize(List<NpcUnit> h)
    {
        foreach (var n in h)
        {
            navMeshAgents.Add(n.npcName, n.GetComponent<NavMeshAgent>());
        }
    }
    
    public void MoveNpc(NpcName npc, Transform target)
    {
        navMeshAgents[npc].Warp(target.position);
    }

    public void SetNpcActive(NpcName npc, bool active)
    {
        if (navMeshAgents.ContainsKey(npc))
            navMeshAgents[npc].gameObject.SetActive(active);
    }
}
