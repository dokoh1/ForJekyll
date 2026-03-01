using MBT;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

[AddComponentMenu("")]
[MBTNode("Example/Monster Lost Target")]
public class MonsterLostTarget : Leaf
{
    public IntReference curState;
    public BoolReference isDetect;
    public BoolReference isLost;

    public Monster monster;
    public float lostTime = 2.4f;

    //private bool _isWait;
    private float _timer;

    //private WaitForSeconds _waitTime = new WaitForSeconds(2.4f);
    //private Coroutine _coroutine;

    public override void OnEnter()
    {
        _timer = 0f;
        //Debug.Log($"MonsterLostTarget - OnEnter - @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
        //Debug.Log($"MonsterLostTarget - OnEnter - isDetect : {isDetect.Value}, timer : {_timer}, isLost : {isLost.Value}");

        curState.Value = (int)MonsterState.Lost;

        //_isWait = true;
        if (monster.agent.isOnNavMesh)
        {
            monster.agent.isStopped = true;
        }

        // monster.Sound.StopStepAudio();
        monster.SetAnimation(false, false, false, false, true);
        //_coroutine = StartCoroutine(WaitTime());
    }

    public override NodeResult Execute()
    {
        if (_timer >= 2.4f)
        {
            //Debug.Log($"MonsterLostTarget - Execute() - ���Ϸ�, {_timer}");
            //_isWait = false;
            return NodeResult.success;
        }

        if (isDetect.Value && _timer >= 0.6f)
        {
            //Debug.Log($"MonsterLostTarget - Execute() - ��� ���� �Ϸ�  - isDetect : {isDetect.Value}, timer : {_timer}, lostTime : {lostTime}");
            //_isWait = false;
            return NodeResult.success;
        }

        //if (!_isWait)
        //{
        //    if (isDetect.Value) return NodeResult.success;
        //}

        _timer += this.DeltaTime;
        return NodeResult.running;
    }

    //private IEnumerator WaitTime()
    //{
    //    yield return _waitTime;
    //    _isWait = false;
    //}

    public override void OnExit()
    {
        //Debug.Log($"MonsterLostTarget - OnExit - isDetect : {isDetect.Value}, timer : {_timer}, isLost : {isLost.Value}");

        if (_timer >= lostTime) isLost.Value = false;

        //StopCoroutine(_coroutine);
        //_coroutine = null;

    }
}
