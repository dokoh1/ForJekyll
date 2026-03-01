using UnityEngine;
using UnityEngine.Animations.Rigging;

public class NPCLook : MonoBehaviour, IPausableLook
{
    [Header("Reference")]
    public Transform playerTransform;
    public MultiAimConstraint headConstraint;

    [Header("View Setting")]
    public bool canLook;
    public float maxViewAngle = 55f;
    public float transitionSpeed = 2f;
    private WeightedTransformArray _sources;

    [Header("ChangeFacial")]
    public bool canFacial;
    public float facialChangeTime = 5f;
    public Animator animator;

    private bool _isLookingAtPlayer;

    private bool _isChangeFacial;
    private float _lookTimeCounter;

    [field: SerializeField] public Transform FreezeAnchor { get; set; }
    public bool IsFrozen { get; set; }
    public float CachedWeight { get; set; }

    private float _currentWeight;
    private float _targetWeight;

    private float _lastAngle = -999f;

    //private bool _isChangeFreezeAnchor;
    private bool _isBackToAnchor;

    private void Awake()
    {
        _sources = headConstraint.data.sourceObjects;
        if (_sources.Count == 0)
        {
            _sources.Add(new WeightedTransform(playerTransform, 0f));
            _sources.Add(new WeightedTransform(FreezeAnchor, 0f));
        }
        else
        {
            _sources.SetTransform(0, playerTransform);
            _sources.SetTransform(1, FreezeAnchor);
            _sources.SetWeight(0, 0f);
            _sources.SetWeight(1, 0f);
        }

        _currentWeight = 0f;
        _targetWeight = 0f;
        headConstraint.data.sourceObjects = _sources;
    }


    private void LateUpdate()
    {
        if (IsFrozen) return;

        if (!canLook)
        {
            _targetWeight = 0f;
            ApplyWeightTowardsTarget();
            return;
        }

        if (_isLookingAtPlayer && canFacial && !_isChangeFacial)
        {
            _lookTimeCounter += Time.deltaTime;

            if (_lookTimeCounter >= facialChangeTime)
            {
                ChangeFacialOn();
            }
        }

        if (_isBackToAnchor)
        {

            ApplyWeightTowardsTarget(1, _sources.GetWeight(1), 0f);
            if (Mathf.Approximately(_sources.GetWeight(1), 0f))
            {
                //Debug.Log("[NPCLook] Back to player");
                _isBackToAnchor = false;
                _sources.SetWeight(1, 0f);
                headConstraint.data.sourceObjects = _sources;
            }
            return;
        }

        float angle = 0f; 
        bool withinAngle = _isLookingAtPlayer && CheckAngle(out angle);
        if (withinAngle)
        {
            if (Mathf.Abs(angle - _lastAngle) > 10f)
            {
                //Debug.Log($"[NPCLook] Player within view angle: {angle:0.0}");
                _lastAngle = angle;
                _targetWeight = 1f;
            }            
        }
        else
        {
            //Debug.Log("[NPCLook] Player out of view angle");
            _targetWeight = 0f;
            _lastAngle = -999f;
        }

        ApplyWeightTowardsTarget();
    }

    private void ApplyWeightTowardsTarget()
    {
        if (Mathf.Approximately(_currentWeight, _targetWeight))
            return;

        _currentWeight = Mathf.MoveTowards(_currentWeight, _targetWeight, Time.deltaTime * transitionSpeed);

        _sources = headConstraint.data.sourceObjects;
        _sources.SetWeight(0, _currentWeight);
        headConstraint.data.sourceObjects = _sources;
    }

    private void ApplyWeightTowardsTarget(int index, float cur, float target)
    {
        if (Mathf.Approximately(cur, target))
            return;

        cur = Mathf.MoveTowards(cur, target, Time.deltaTime * transitionSpeed);

        _sources = headConstraint.data.sourceObjects;
        _sources.SetWeight(index, cur);
        headConstraint.data.sourceObjects = _sources;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canLook) return;
        if (!other.CompareTag("Player")) return;

        _isLookingAtPlayer = true;
        if (IsFrozen) return;

        _targetWeight = 1f;

        //Debug.Log("OnTriggerEnter: " + other.name);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!canLook) return;
        if (!other.CompareTag("Player")) return;

        if (_lookTimeCounter > 0f && canFacial && !IsFrozen) ChangeFacialOff();

        _isLookingAtPlayer = false;
        if (IsFrozen) return;

        _targetWeight = 0f;

    }

    private bool CheckAngle(out float angle)
    {
        angle = 180f;

        Vector3 toPlayer = playerTransform.position - transform.position;
        toPlayer.y = 0f;

        if (toPlayer.sqrMagnitude < 0.0001f)
        {
            return false;
        }

        angle = Vector3.Angle(transform.forward, toPlayer);
        return angle <= maxViewAngle;
    }

    public void StopLook()
    {
        if (canFacial) ChangeFacialOff();

        _isLookingAtPlayer = false;
        canLook = false;
        canFacial = false;

        _targetWeight = 0f;
        _currentWeight = 0f;

        _sources = headConstraint.data.sourceObjects;
        if (_sources.Count == 0)
            _sources.Add(new WeightedTransform(null, 0f));
        else
            _sources.SetWeight(0, 0f);

        headConstraint.data.sourceObjects = _sources;
    }

    public void ChangeFacialOn()
    {
        if (animator == null)
        {
            Debug.LogWarning("애니메이터 없음");
            return;
        }

        _isChangeFacial = true;
        animator.SetBool("Facial", true);
    }

    public void ChangeFacialOff()
    {
        _isChangeFacial = false;
        _lookTimeCounter = 0f;
        animator.SetBool("Facial", false);
    }

    public void PauseLook()
    {
        IsFrozen = true;
        CachedWeight = _currentWeight;

        if (_isLookingAtPlayer && FreezeAnchor != null)
        {
            FreezeAnchor.position = new Vector3(
                playerTransform.position.x,
                playerTransform.position.y,
                playerTransform.position.z
            );

            _sources = headConstraint.data.sourceObjects;
            _sources.SetWeight(0, 0f);
            _sources.SetWeight(1, CachedWeight);
            //_isChangeFreezeAnchor = true;

            headConstraint.data.sourceObjects = _sources;
        }
    }

    public void ResumeLook()
    {
        IsFrozen = false;

        _sources = headConstraint.data.sourceObjects;
        _sources.SetWeight(0, 0f);
        CachedWeight = 0f;
        _currentWeight = 0f;

        _isBackToAnchor = true;
        headConstraint.data.sourceObjects = _sources;
    }

    private void OnEnable()
    {
        GameManager.Instance.OnTimeStop += PauseLook;
        GameManager.Instance.OffTimeStop += ResumeLook;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnTimeStop -= PauseLook;
        GameManager.Instance.OffTimeStop -= ResumeLook;
    }
}