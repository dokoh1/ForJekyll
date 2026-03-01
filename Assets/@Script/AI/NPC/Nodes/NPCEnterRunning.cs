using MBT;
using NPC;
using UnityEngine;
[AddComponentMenu("")]
[MBTNode("Example/NPC Enter Running")]
public class NPCEnterRunning : Leaf
{
    public BoolReference isRunning;
    public NpcUnit npcUnit;

    public override void OnEnter()
    {
        isRunning.Value = true;
    }

    public override NodeResult Execute()
    {
        return NodeResult.failure;
    }
}
