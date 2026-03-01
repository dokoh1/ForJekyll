using MBT;
using UnityEngine;
using UnityEngine.AI;

public class MonsterV2 : MonoBehaviour, IDeactivate, IPausableMove
{
    [field: Header("References")]
    [field: SerializeField] protected Blackboard BB { get; set; }
    [field: SerializeField] public GameObject PatrolPosition { get; set; }
    // [field: SerializeField] public UnitSoundSystem Sound_ { get; set; }
    public MonsterSoundSystem Sound { get; private set; }

    [field: Header("Animations")]
    [field: SerializeField] public MonsterAnimationData MonsterAnimationData { get; set; }
    public Animator animator;

    [field: Header("Nav")]
    public NavMeshAgent agent;

    [field: Header("Setting")]
    public MonsterSetting monsterSetting;

    private float _rotationSpeed = 8f;
    private float _finishAngle = 5f; 
    private bool _hasReachedPlayer = false;
    public bool isLooking = false;

    private Patrol Patrol { get; set; }

    public float PrevSpeed { get; set; }
    public bool WasStopped { get; set; }

    private void Awake()
    {
        Patrol = new Patrol(this);
        if (PatrolPosition != null) Patrol.PatrolTransforms = PatrolPosition.GetComponentsInChildren<Transform>();
        if (Sound == null) Sound = GetComponent<MonsterSoundSystem>();
        // if (Sound_ == null) Sound_ = GetComponent<UnitSoundSystem>();
        Patrol.SetPatrolPositions(HasSetting(MonsterSetting.KeepPosition));
        MonsterAnimationData.Initialize();
    }

    protected virtual void Start()
    {
        if (HasSetting(MonsterSetting.KeepPosition))
        {
            Destroy(PatrolPosition);
            PatrolPosition = null;
        }
    }

    protected virtual void Update()
    {
        if(GameManager.Instance.IsTimeStop) 
            return;

        if (isLooking && !_hasReachedPlayer)
        {
            LookAtPlayer();
        }
    }

    public void LookAtPlayer()
    {
        if (BB.GetVariable<Variable<Transform>>("target").Value != null && BB.GetVariable<Variable<Transform>>("target").Value.gameObject.layer == 3) return; 

        Vector3 direction = GameManager.Instance.Player.transform.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                _rotationSpeed * Time.deltaTime
            );

            float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);
            if (angleDifference <= _finishAngle)
            {
                //Debug.Log($"EyeTypeMonsterV2 - LookAtPlayer() - Reached player: {gameObject.name}");
                _hasReachedPlayer = true;
                isLooking = false;
            }
        }
    }

    public bool MoveToDestination(Vector3 vector3)
    {
        if (NavMesh.SamplePosition(vector3, out NavMeshHit hit, Vector3.Distance(transform.position, vector3), NavMesh.AllAreas))
        {
            if (agent.isStopped) agent.isStopped = false;

            agent.SetDestination(hit.position);
            //Debug.Log($"Moving to destination: {hit.position}");
            return false;
        }
        else
        {
            return true;
        }
    }

    public void SetAnimation(bool idle, bool walk, bool run, bool attack, bool lost, bool find)
    {
        animator.SetBool(MonsterAnimationData.IdleParameterHash, idle);
        animator.SetBool(MonsterAnimationData.WalkParameterHash, walk);
        animator.SetBool(MonsterAnimationData.RunParameterHash, run);
        animator.SetBool(MonsterAnimationData.AttackParameterHash, attack);
        animator.SetBool(MonsterAnimationData.LostTargetParameterHash, lost);
        animator.SetBool(MonsterAnimationData.FindTargetParameterHash, find);
    }

    public void SetAnimation(bool idle, bool walk, bool run, bool attack, bool lost)
    {
        animator.SetBool(MonsterAnimationData.IdleParameterHash, idle);
        animator.SetBool(MonsterAnimationData.WalkParameterHash, walk);
        animator.SetBool(MonsterAnimationData.RunParameterHash, run);
        animator.SetBool(MonsterAnimationData.AttackParameterHash, attack);
        animator.SetBool(MonsterAnimationData.LostTargetParameterHash, lost);
    }

    public void SetAnimation(bool idle, bool walk, bool run, bool lost)
    {
        animator.SetBool(MonsterAnimationData.IdleParameterHash, idle);
        animator.SetBool(MonsterAnimationData.WalkParameterHash, walk);
        animator.SetBool(MonsterAnimationData.RunParameterHash, run);
        animator.SetBool(MonsterAnimationData.LostTargetParameterHash, lost);
    }

    public Vector3 GetPatrolPosition()
    {
        return Patrol.GetPatrolPosition(HasSetting(MonsterSetting.KeepPosition));
    }

    private void OnEnable()
    {
        GameManager.Instance.OnGameover += Deactivate;
        GameManager.Instance.OnTimeStop += PauseMove;
        GameManager.Instance.OffTimeStop += ResumeMove;
    }

    private void OnDisable()
    {
        Sound.StopAllSound();
        GameManager.Instance.OnGameover -= Deactivate;
        GameManager.Instance.OnTimeStop -= PauseMove;
        GameManager.Instance.OffTimeStop -= ResumeMove;
    }

    public void MonsterWork(bool isWork)
    {
        SetVariable("isWork", isWork);
    }

    protected void SetVariable<T>(string key, T value)
    {
        BB.GetVariable<Variable<T>>(key).Value = value;
    }

    protected bool HasSetting(MonsterSetting setting) => (monsterSetting & setting) == setting;

    public virtual void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void PauseMove()
    {
        PrevSpeed = animator.speed;
        animator.speed = 0;
        WasStopped = agent.isStopped;
        agent.isStopped = true;
    }

    public void ResumeMove()
    {
        animator.speed = PrevSpeed;
        agent.isStopped = WasStopped;
    }
}