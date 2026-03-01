using MBT;
using UnityEngine;

[AddComponentMenu("")]
[MBTNode("Example/EyeType Lost Target")]
public class EyeTypeLostTarget : Leaf
{
    public IntReference curState;
    public BoolReference isDetect;
    public BoolReference isLost;

    public MonsterV2 monster;

    public float lostTime = 2.4f;
    public float skipTime = 0.4f;

    private float _timer;

    public override void OnEnter()
    {
        //Debug.Log($"EyeTypeLostTarget - OnEnter() - isDetect : {isDetect.Value}");
        _timer = 0f;

        if (monster.agent.isOnNavMesh)
        {
            monster.agent.isStopped = true;
        }

        curState.Value = (int)MonsterState.Lost;
        monster.SetAnimation(false, false, false, false, true, false);
        GameManager.Instance.Player.BGMController.EndChase(monster);
    }

    public override NodeResult Execute()
    {
        if (GameManager.Instance.IsTimeStop) return NodeResult.running;

        if (_timer >= lostTime)
        {
            isLost.Value = false;
            return NodeResult.failure;
        }

        if (isDetect.Value && _timer >= skipTime)
        {
            isLost.Value = false;
            return NodeResult.failure;
        }

        _timer += this.DeltaTime;
        return NodeResult.running;
    }
}