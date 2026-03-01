using MBT;
using System;
using UnityEngine;

[AddComponentMenu("")]
[MBTNode("Example/EyeType Find Target")]
public class EyeTypeFindTarget : Leaf
{
    public TransformReference self;
    public TransformReference target;

    public IntReference curState;

    public BoolReference isDetect;
    public BoolReference isLost;
    public BoolReference isFind;

    public MonsterV2 monster;

    public float delayTime = 1.8f;
    //public float skipTime = 1.2f;
    private float _timer;

    private float _rotationSpeed = 5f;

    //private Vector3 _direction;

    public override void OnEnter()
    {
        _timer = 0f;
        
        //Debug.Log($"EyeTypeFindTarget - OnEnter() - target : isDetect : {isDetect.Value}, _timer : {_timer}");
        

        if (target.Value == null)
        {
            Debug.LogWarning($"EyeTypeFindTarget - OnEnter() - target is null for {self.Value.name}");
            return;
        }

        //Debug.Log($"EyeTypeFindTarget - OnEnter() - target : {target.Value.name}, isDetect : {isDetect.Value}, _timer : {_timer}");

        if (monster.agent.isOnNavMesh)
        {
            monster.agent.isStopped = true;
        }

        curState.Value = (int)MonsterState.Find;
        //self.Value.LookAt(target.Value.position);
        monster.SetAnimation(false, false, false, false, false, true);
    }

    public override NodeResult Execute()
    {
        if (GameManager.Instance.IsTimeStop) return NodeResult.running;

        if (!monster.isLooking) LookAt();

        if (_timer >= delayTime)
        {
            isFind.Value = false;

            if (target.Value == null && !isDetect.Value)
            {
                isLost.Value = true;
            }
            else
            {
                isLost.Value = false;
            }

            //Debug.Log($"EyeTypeFindTarget - Execute() - Timer exceeded delay time: {_timer} >= {delayTime}, isDetect: {isDetect.Value}, isLost: {isLost.Value}");
            return NodeResult.failure;
        }

        //if (isDetect.Value && _timer >= skipTime)
        //{
        //    isFind.Value = false;
        //    return NodeResult.failure;
        //}

        _timer += this.DeltaTime;
        
        return NodeResult.running;
    }

    private void LookAt()
    {
        if (target.Value == null) return;
        Vector3 direction = target.Value.position - self.Value.position;
        direction.y = 0f;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        self.Value.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            _rotationSpeed * Time.deltaTime
        );
    }

}