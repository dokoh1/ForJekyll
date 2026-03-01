using MBT;
using System;
using UnityEngine;

[AddComponentMenu("")]
[MBTNode("Example/Monster Set Lost Target Value")]
public class MonsterSetLostTargetValue : Leaf
{
    public BoolReference isLost;
    public BoolReference isDetect;
    public TransformReference target;

    public override void OnEnter()
    {
        Debug.Log($"MonsterSetLostTargetValue OnEnter - isDetect: {isDetect.Value}, isLost: {isLost.Value}, target: {target.Value}");
        isLost.Value = true;
        isDetect.Value = false;
        target.Value = null;
    }

    public override NodeResult Execute()
    {
        return NodeResult.success;
    }
}