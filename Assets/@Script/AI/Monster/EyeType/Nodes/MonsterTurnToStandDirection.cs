using MBT;
using System.Collections;
using UnityEngine;

[AddComponentMenu("")]
[MBTNode("Example/Monster Turn To Stand Direction")]
public class MonsterTurnToStandDirection : Leaf
{
    public TransformReference self;
    public QuaternionReference originDirection;
    public BoolReference isOriginPosition;
    public BoolReference isDetect;

    public float duration = 1f;

    private Coroutine _coroutine;
    private bool _isStand;
    private bool _isChange;

    public override void OnEnter()
    {
        _isChange = true;
        _coroutine = StartCoroutine(MonsterRotation());
    }

    public override NodeResult Execute()
    {
        if (isDetect.Value) return NodeResult.success;
        if (!_isChange) return NodeResult.success;
        return NodeResult.running;
    }

    IEnumerator MonsterRotation()
    {
        Quaternion startRotation = self.Value.rotation;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            self.Value.rotation = Quaternion.Slerp(startRotation, originDirection.Value, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        self.Value.rotation = originDirection.Value;
        _isChange = false;     
        isOriginPosition.Value = true;
    }

    public override void OnExit()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }        
    }
}
