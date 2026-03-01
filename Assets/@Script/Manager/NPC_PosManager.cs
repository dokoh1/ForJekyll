using UnityEngine;
using UnityEngine.AI;

public class NPC_PosManager : MonoBehaviour
{
    [SerializeField] private Transform[] firstEmptyPos;
    [SerializeField] private Transform[] secondEmptyPos;

    [SerializeField] private Transform[] npc;

    [SerializeField] private Transform secondPlayerPos;
    public void FirstChangeNPC_Pos()
    {
        for (int i = 0; i < firstEmptyPos.Length; i++) 
        {
            NavMeshAgent agent = npc[i].GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.Warp(firstEmptyPos[i].position);
            }
            else
            {
                npc[i].position = firstEmptyPos[i].position;
            }
        }
    }
    public void SecondChangeNPC_Pos() 
    {
        for (int i = 0; i < secondEmptyPos.Length; i++)
        {
            npc[i].position = secondEmptyPos[i].position;
        }
        GameManager.Instance.MovePlayerTransform(secondPlayerPos);
    }
}
