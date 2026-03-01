using MBT;
using NPC;
using UnityEngine;
using UnityEngine.AdaptivePerformance.VisualScripting;

[AddComponentMenu("")]
[MBTNode(name = "NPC Rest")]
public class NPCRest : Wait
{
    public BoolReference isRest;
    public IntReference curState;
    public NpcUnit npcUnit;

    public Animator animator;

    public override void OnEnter()
    {
        base.OnEnter();
        curState.Value = (int)NPCState.Idle;
        if (!npcUnit.Agent.isStopped) npcUnit.Agent.isStopped = true;
        npcUnit.CurNoiseAmount = 0f;
        animator.SetBool("Idle", true);
        animator.SetBool("Run", false);
        animator.SetBool("Walk", false);
    }

    public override void OnExit()
    {
        base.OnExit();
        curState.Value = (int)((NPCState)curState.Value & ~NPCState.Idle);
        isRest.Value = false;
    }

    public override NodeResult Execute()
    {
        if (GameManager.Instance.IsTimeStop) return NodeResult.running;

        //Debug.Log($"Wait - Execute() - timer, {timer}, {time.Value}");

        if (timer >= time.Value)
        {
            // Reset timer in case continueOnRestart option is active
            if (continueOnRestart)
            {
                timer = (randomDeviation == 0f) ? 0f : Random.Range(-randomDeviation, randomDeviation);
            }
            //Debug.Log($"Wait - Execute() - 대기완료, {timer}");
            return NodeResult.success;
        }
        timer += this.DeltaTime;
        return NodeResult.running;
    }

}
