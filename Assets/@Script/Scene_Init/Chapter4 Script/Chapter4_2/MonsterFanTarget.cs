using UnityEngine;
using UnityEngine.AI;

public class MonsterFanTarget : FanTarget
{
    private NavMeshAgent agent;
    private Vector3 fanVelocity;

    [SerializeField] private float damping = 0.85f;

    private bool soundPlayed = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.avoidancePriority = Random.Range(30, 60);
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
    }

    public override void AddVelocity(Vector3 v)
    {
        fanVelocity += v;
    }

    public void OnInitialPush(Vector3 v)
    {
        if (!soundPlayed)
        {
            var sound = GetComponent<MonsterSoundSystem>();
            if (sound != null)
                sound.PlaySoundByKey(MonsterSoundKey.Attack, false);
            Debug.Log("Monster Attack");

            soundPlayed = true;
        }

        fanVelocity += v;
    }

    private void Update()
    {
        if (fanVelocity.sqrMagnitude > 0.001f)
        {
            //agent.nextPosition += fanVelocity * Time.deltaTime;
            agent.Move(fanVelocity * Time.deltaTime);
            fanVelocity *= damping;
        }
    }

    public override void ResetFan()
    {
        fanVelocity = Vector3.zero;
        agent.isStopped = false;
        soundPlayed = false;
    }
}
