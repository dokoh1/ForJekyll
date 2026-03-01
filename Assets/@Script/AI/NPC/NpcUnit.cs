using MBT;
using System;
using UnityEngine;

namespace NPC
{
    public class NpcUnit : Npc, INoise
    {
        [field: Header("Interactable")]
        [field: SerializeField] public override bool IsInteractable { get; set; }
        [field: SerializeField] public override float InteractHoldTime { get; set; }
        
        // INoise
        [field: SerializeField] public float CurNoiseAmount { get; set; }
        [field: SerializeField] public float NoiseCheckAmount { get; set; }
        public float SumNoiseAmount { get; set; }
        public float DecreaseSpeed { get; set; }
        public float MaxNoiseAmount { get; set; }

        [field: Header("Dialog")]
        [field: SerializeField] public int DialogNum { get; set; }
        [field: SerializeField] public NpcName npcName { get; private set; }
        [field: SerializeField] public Dialogue dialogue { get; set; }
        [field: SerializeField] public bool is2D { get; set; } = true;

        public event Action<NpcName> onInteractEnd;
        public event Action<NpcName> onInteractStart;
        
        protected override NpcUnit GetNpcUnit() => this;
        private string npcReName;

        protected override void Start()
        {
            base.Start();
            Init();
            GameManager.Instance.OnGameover += Deactivate;
        }

        private void Init()
        {
            if (IsInjured && InjuryData != null) NpcData = InjuryData;
            else NpcData = BasicData;

            if (canLook && npcLook != null)
            {
                npcLook.canLook = true;
                npcLook.playerTransform = GameManager.Instance.Player.FPView;
            }

            if (player == null) player = GameManager.Instance.Player.transform;
            animator.SetBool("Idle", true);
            _self = transform;

            if (bb != null)
            {
                bb.GetVariable<Variable<float>>("nearDistance").Value = NpcData.Data.NearDistance;
                bb.GetVariable<Variable<float>>("farDistance").Value = NpcData.Data.FarDistance;
                bb.GetVariable<Variable<float>>("baseSpeed").Value = NpcData.Data.BaseSpeed;
                bb.GetVariable<Variable<float>>("walkModifier").Value = NpcData.Data.WalkSpeedModifier;
                bb.GetVariable<Variable<float>>("runModifier").Value = NpcData.Data.RunSpeedModifier;
                bb.GetVariable<Variable<float>>("crouchModifier").Value = NpcData.Data.CrouchSpeedModifier;
                bb.GetVariable<Variable<float>>("restTime").Value = NpcData.Data.RestTime;
                bb.GetVariable<Variable<int>>("curState").Value = (int)NPCState.Idle;
                bb.GetVariable<Variable<Transform>>("player").Value = player;
            }

            if ((npcSetting & NPCSetting.StartFollowing) == NPCSetting.StartFollowing && !bb.GetVariable<Variable<bool>>("isFollow").Value)
            {
                animator.SetBool("Idle", false);
                animator.SetBool("@Follow", true);
                _isFollowing = true;
                bb.GetVariable<Variable<bool>>("isFollow").Value = true;
                npcSetting &= ~NPCSetting.CanFollow;
                npcSetting &= ~NPCSetting.StartFollowing;
                GameManager.Instance.Player.WithNpc = gameObject.transform;
            }

            if (IsInjured) SetInjury(IsInjured);
        }

        public override void Interact()
        {
            _hasReachedPlayer = false;
            _lookAtPlayer = true;
            IsInteractable = false;
            //if ((npcSetting & NPCSetting.CanRepeat) != NPCSetting.CanRepeat) IsInteractable = false;

            if ((npcSetting & NPCSetting.CanTalk) == NPCSetting.CanTalk)
            {
                if (!is2D) GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.OnlyInteractLocked,true);
                // NPC dialog
                switch (npcName)
                {
                    case NpcName.YHJ: npcReName = "연효진"; break;
                    case NpcName.PSY: npcReName = "표서윤"; break;
                    case NpcName.KJM: npcReName = "강주명"; break;
                        default: npcReName = null; break;
                }
                onInteractStart?.Invoke(npcName);
                onInteractStart = null;
                
                UIManager.Instance.dialogueEnd += InteractEnd;
                UIManager.Instance.DialogueOpen(dialogue, is2D, DialogNum, npcReName);
            }

            if ((npcSetting & NPCSetting.CanFollow) == NPCSetting.CanFollow && !bb.GetVariable<Variable<bool>>("isFollow").Value) FollowStart();
        }

        private void InteractEnd()
        {
            if ((npcSetting & NPCSetting.CanRepeat) == NPCSetting.CanRepeat) IsInteractable = true;
            onInteractEnd?.Invoke(npcName);
            if (!is2D) GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.OnlyInteractLocked,false);
        }

        public override void FollowStart()
        {
            base.FollowStart();
        }

        public void SetActivation()
        {
            gameObject.SetActive(true);
            Init();
            FollowStart();
        }

        protected void SetDeActivation()
        {
            gameObject.SetActive(false);
        }

    }
}

