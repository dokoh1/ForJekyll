using MBT;
using UnityEngine;
using UnityEngine.AI;

public abstract class Monster : MonoBehaviour, IDeactivate
{
    [field: Header("References")]
    [field: SerializeField] protected Blackboard BB { get; set; }
    [field: SerializeField] public GameObject PatrolPosition { get; set; }
    [field: SerializeField] public MonsterSoundSystem Sound { get; private set; }

    [field: Header("Animations")]
    [field: SerializeField] public MonsterAnimationData MonsterAnimationData { get; set; }
    public Animator animator;

    [field: Header("Nav")]
    public NavMeshAgent agent;

    [field: Header("Setting")]
    public MonsterSetting monsterSetting;
    [field: SerializeField] protected Transform TargetNpc { get; set; }
    private Patrol Patrol { get; set; }

    private void Awake()
    {
        Patrol = new Patrol(this);
        if (PatrolPosition != null) Patrol.PatrolTransforms = PatrolPosition.GetComponentsInChildren<Transform>();
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

    public bool MoveToDestination(Vector3 vector3)
    {
        if (NavMesh.SamplePosition(vector3, out NavMeshHit hit, Vector3.Distance(transform.position, vector3), NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            return false;
        }
        return true;
    }

    public bool MoveToDestination(Vector3 vector3, float distance)
    {
        if (NavMesh.SamplePosition(vector3, out NavMeshHit hit, distance, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            return false;
        }
        return true;
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
    }

    private void OnDisable()
    {
        GameManager.Instance.OnGameover -= Deactivate;
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
}
