using MBT;
using UnityEngine;

[AddComponentMenu("")]
[MBTNode("Example/EyeType Turn To Stand Direction")]
public class EyeTypeTurnToStandDirection : Leaf
{
    public TransformReference self;

    public QuaternionReference originDirection;

    public BoolReference isOriginPosition;
    public BoolReference isDetect;

    public float duration = 1f;

    private Quaternion _startRotation;
    private float _elapsedTime;
    private bool _isTurning;

    public override void OnEnter()
    {
        _startRotation = self.Value.rotation;
        _elapsedTime = 0f;
        _isTurning = true;
    }

    public override NodeResult Execute()
    {
        if (GameManager.Instance.IsTimeStop) return NodeResult.running;

        if (isDetect.Value)
        {
            return NodeResult.success;
        }

        if (!_isTurning)
        {
            return NodeResult.success;
        }

        _elapsedTime += Time.deltaTime;

        float t = Mathf.Clamp01(_elapsedTime / duration);
        self.Value.rotation = Quaternion.Slerp(_startRotation, originDirection.Value, t);

        if (t >= 1f)
        {
            self.Value.rotation = originDirection.Value;
            isOriginPosition.Value = true;
            _isTurning = false;
            return NodeResult.success;
        }

        return NodeResult.running;
    }

    public override void OnExit()
    {
        _isTurning = false;
    }
}