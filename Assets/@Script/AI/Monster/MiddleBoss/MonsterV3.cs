using MBT;
using UnityEngine;
using UnityEngine.AI;

public class MonsterV3 : MonoBehaviour
{
    [field: Header("References")]
    [field: SerializeField] protected Blackboard BB { get; set; }
    [field: SerializeField] public GameObject PatrolPosition { get; set; }
    
    [field: Header("Animations")]
    [field: SerializeField] public BossAnimationData BossAnimationData { get; set; }
    [field: SerializeField] public Animator animator;
    
    [field: Header("Nav")] 
    public NavMeshAgent agent;

    [field: Header("Setting")] 
    public MonsterSetting monsterSetting;
    
    public MonsterSoundSystem Sound { get; private set; }

    private float  lookRotationSpeed = 8f;
    private float  lookFinishAngle   = 1f;
    private bool   _hasReachedLookTarget;
    private Vector3 _lookTargetPos;
    private bool   _isLooking;
        
    private void Awake()
    {
        if (Sound == null)
            Sound = GetComponent<MonsterSoundSystem>();
        
        if (BossAnimationData != null)
            BossAnimationData.Initialize();
    }

    protected virtual void Start()
    {
    }

    protected virtual void Update()
    {
        if (_isLooking && !_hasReachedLookTarget)
        {
            // 필요하면 아래 함수 호출
            UpdateLookAt();
        }
    }

    #region LookAt
    /// <summary>
    /// 특정 월드 좌표를 향해 몸을 돌리기 시작한다.
    /// - y축은 고정(수평 회전).
    /// - 각도 차이가 lookFinishAngle 이하가 되면 자동 종료.
    /// </summary>
    public void LookAtPosition(Vector3 worldPos)
    {
        _lookTargetPos = worldPos;
        _lookTargetPos.y = transform.position.y;

        _hasReachedLookTarget = false;
        _isLooking = true;
    }

    private void UpdateLookAt()
    {
        Vector3 dir = _lookTargetPos - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f)
        {
            _hasReachedLookTarget = true;
            _isLooking = false;
            return;
        }
        
        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            lookRotationSpeed * Time.deltaTime);

        float angleDiff = Quaternion.Angle(transform.rotation, targetRot);

        if (angleDiff <= lookFinishAngle)
        {
            _hasReachedLookTarget = true;
            _isLooking = false;
        }
    }

    public bool IsLooking           => _isLooking;
    public bool HasReachedLookTarget => _hasReachedLookTarget;
    #endregion

    #region Animation Helper
    public void SetAnimation(
        bool idle   = false,
        bool charge = false,
        bool detect = false,
        bool rage   = false,
        bool stun   = false)
    {
        animator.SetBool(BossAnimationData.IdleParameterHash,   idle);
        animator.SetBool(BossAnimationData.ChargeParameterHash, charge);
        animator.SetBool(BossAnimationData.DetectParameterHash, detect);
        animator.SetBool(BossAnimationData.RageParameterHash,   rage);
        animator.SetBool(BossAnimationData.StunParameterHash,   stun);
    }
    #endregion

    #region Blackboard Helper
    public void SetVariable<T>(string key, T value)
    {
        if (BB == null)
            return;

        BB.GetVariable<Variable<T>>(key).Value = value;
    }

    protected bool HasSetting(MonsterSetting setting) 
        => (monsterSetting & setting) == setting;
    #endregion
}
