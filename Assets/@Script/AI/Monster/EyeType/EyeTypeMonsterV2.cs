using MBT;
using UnityEngine;

public class EyeTypeMonsterV2 : MonsterV2
{
    [field: Header("SO")]
    [field: SerializeField] private EyeTypeMonsterDataSO MonsterData { get; set; }

    [field: Header("Target")]
    [field: SerializeField] protected Transform TargetNpc { get; set; }

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
                SetVariable("isOriginPosition", true);
            }

            // target -> destination -> work
            if (HasSetting(MonsterSetting.HaveTarget))
            {
                if (TargetNpc != null)
                {
                    SetVariable("targetNpc", TargetNpc);
                    SetVariable("haveTarget", true);
                }
                else
                {
                    Debug.LogWarning($"EyeTypeMonsterV2 - 타켓이 빠져있습니다 - {gameObject.name}");
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

    public void ChasePlayer()
    {
        if (BB.GetVariable<Variable<Transform>>("target").Value != null && BB.GetVariable<Variable<Transform>>("target").Value.gameObject.layer == 3)
        {
            //Debug.LogWarning($"ChasePlayer() - target layer is 3");
            return;
        }

        //Debug.Log($"ChasePlayer() - {gameObject.name} is chasing player");

        MoveToDestination(GameManager.Instance.Player.transform.position);
        SetAnimation(false, false, true, false, false, false);

        //MonsterState state = (MonsterState)curState.Value;

        MonsterState state = (MonsterState)BB.GetVariable<Variable<int>>("curState").Value;
        if (!state.HasFlag(MonsterState.Run))
        {
            SetVariable("curState", (int)MonsterState.Run);
        }
    }
}