using NPC;
using UnityEngine;

public class NPC_Chapter1_1Manager : SceneServiceLocator
{
    [Header("NPC")]
    private NpcUnit[] npcUnits;
    private NpcUnit PJH;
    private NpcUnit KJM;
    
    private Chapter1_1_Manager chapter1_1_Manager;
    public void Initialize(Chapter1_1_Manager chapter1_1_Manager)
    {
        this.chapter1_1_Manager = chapter1_1_Manager;
    }
    public void NpcInitialize(NpcUnit[] npcUnits, NpcUnit PJH, NpcUnit KJM)
    {
        this.npcUnits = npcUnits;
        this.PJH = PJH;
        this.KJM = KJM;
    }
    
    

    public void StopInteract()
    {
        foreach(var npc in npcUnits)
        {
            npc.IsInteractable = false;
        }
    }
}
