using UnityEngine;

public class EyeTypeMonster : Monster
{
    [field: Header("SO")]
    [field: SerializeField] private EyeTypeMonsterDataSO MonsterData { get; set; }

    [field: Header("Destination")]
    [field: SerializeField] protected Transform Destination { get; set; }

    protected override void Start()
    {
        base.Start();
        Initialization();
    }

    private void Initialization()
    {
        if (BB != null)
        {
            SetVariable("findRange", MonsterData.Data.FindRange);
            SetVariable("chaseRange", MonsterData.Data.ChaseRange);
            SetVariable("attackRange", MonsterData.Data.AttackRange);
            SetVariable("viewAngle", MonsterData.Data.ViewAngle);
            SetVariable("baseSpeed", MonsterData.Data.BaseSpeed);
            SetVariable("walkSpeedModifier", MonsterData.Data.WalkSpeedModifier);
            SetVariable("runSpeedModifier", MonsterData.Data.RunSpeedModifier);
            SetVariable("player", GameManager.Instance.Player.transform);

            if (HasSetting(MonsterSetting.CanPatrol)) SetVariable("canPatrol", true);
            else
            {
                SetVariable("originPosition", transform.position);
                SetVariable("originDirection", transform.rotation);
            }

            // target -> destination -> work
            if (HasSetting(MonsterSetting.HaveTarget))
            {
                if (TargetNpc != null)
                {
                    SetVariable("targetNpc", TargetNpc);
                    SetVariable("haveTarget", true);
                }
            }

            if (HasSetting(MonsterSetting.HaveDestination))
            {
                if (Destination != null)
                {
                    SetVariable("destination", Destination.position);
                    SetVariable("haveDestination", true);
                    Destroy(Destination.gameObject);
                }
            }

            SetVariable("isWork", HasSetting(MonsterSetting.IsWork));
        }
    }

    public void LookAt(Transform target)
    {
        transform.LookAt(target);
    }
}
