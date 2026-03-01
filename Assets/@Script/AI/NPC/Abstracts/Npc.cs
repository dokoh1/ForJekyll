using MBT;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace NPC
{
    public abstract class Npc : InteractableBase, IAttackable, IPausableMove
    {
        [field: Header("References")]
        [field: SerializeField] public NPCDataSO NpcData { get; protected set; }
        public Blackboard bb;
        public CharacterSoundSystem SoundSystem { get; private set; }
        public NavMeshAgent Agent { get; private set; }
        public CapsuleCollider capsuleCollider;
        public Transform player;
        public Transform destination;

        [field: Header("Datas")]
        [field: SerializeField] public NPCDataSO BasicData { get; private set; }
        [field: SerializeField] public NPCDataSO InjuryData { get; private set; }

        [field: Header("Animations")]
        public Animator animator;

        [field: Header("Look")]
        public bool canLook;
        public NPCLook npcLook;

        [field: Header("Setting")]
        public NPCSetting npcSetting;
        [field: SerializeField] public bool IsInjured { get; set; } = false;
        private bool _isDead = false;

        protected Transform _self;
        protected bool _hasReachedPlayer = false;
        protected bool _lookAtPlayer = false;
        protected bool _isFollowing;
        private bool _isMoving;
        private float time = 0;
        public event Action<NpcUnit> onNpcStop;
        protected abstract NpcUnit GetNpcUnit();

        public bool IsCrouching { get; set; }

        [field: Header("InteractType")]
        [field: SerializeField] public override InteractTypeEnum InteractType { get; set; } = InteractTypeEnum.Tap;

        public float PrevSpeed { get; set; }
        public bool WasStopped { get; set; }

        private void Awake()
        {
            if(SoundSystem == null) SoundSystem = GetComponent<CharacterSoundSystem>();
            if (Agent == null) Agent = GetComponent<NavMeshAgent>();
            _isDead = false;
        }

        private void Update()
        {
            if (!_hasReachedPlayer && _lookAtPlayer)
            {
                LookAtPlayer();
            }

            if (_isMoving) Moving();
        }


        protected void LookAtPlayer()
        {
            Vector3 direction = player.position - _self.position;
            direction.y = 0;

            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                _self.rotation = Quaternion.Slerp(
                    _self.rotation,
                    targetRotation,
                    Time.deltaTime * 2f
                );

                float angleDifference = Quaternion.Angle(_self.rotation, targetRotation);
                if (angleDifference <= 1f)
                {
                    _hasReachedPlayer = true;
                    _lookAtPlayer = false;
                }
            }            
        }

        protected void NPCDead()
        {
            _isDead = true;
            npcLook.StopLook();
            bool isImportant = gameObject.layer == LayerMask.NameToLayer("ImportantNPC");
            gameObject.layer = 0;
            capsuleCollider.enabled = false;
            bb.GetVariable<Variable<int>>("curState").Value = (int)NPCState.Dead;
            bb.GetVariable<Variable<bool>>("isFollow").Value = false;
            animator.SetBool("@Follow", false);
            animator.SetBool("@Crouch", false);
            animator.SetBool("Idle", false);
            animator.SetBool("Walk", false);
            animator.SetBool("Run", false);
            animator.SetBool("Dead", true);           
  
            if (isImportant)
            {
                UI_PauseMenuPopup.Instance.SwitchActiveESC(false);
                Invoke(nameof(PlayJumpScare), 2f);
            }
        }

        private void PlayJumpScare()
        {
            GameManager.Instance.IsGameover = true;
            GameManager.Instance.Player.JumpScareManager.NpcDead();
        }

        public void OnHitSuccess()
        {
            Invoke(nameof(NPCDead), 0.7f);
        }

        public void OnHitSuccess(UnitEnum.UnitType type)
        {
            if (_isDead) return;

            if (type == UnitEnum.UnitType.EyeTypeMonster)
            {
                Invoke(nameof(NPCDead), 0.7f);
            }
            else if (type == UnitEnum.UnitType.EarTypeMonster)
            {
                NPCDead();
            }
        }

        public void NpcStop()
        {
            bb.GetVariable<Variable<bool>>("isFollow").Value = false;
            Agent.isStopped = true;
            animator.SetBool("@Follow", false);
            animator.SetBool("@Crouch", false);
            animator.SetBool("Idle", true);
            animator.SetBool("Walk", false);
            animator.SetBool("Run", false);
            animator.SetBool("Dead", false);
            SoundSystem.StopAllSound();
        }

        public void SetCrouch(bool isCrouch)
        {
            if (isCrouch)
            {
                if (!IsCrouching)
                {
                    IsCrouching = true;
                    animator.SetBool("@Crouch", true);
                }
                    
            }
            else
            {
                if (IsCrouching)
                {
                    IsCrouching = false;
                    animator.SetBool("@Crouch", false);
                }
            }
        }

        public virtual void FollowStart()
        {
            animator.SetBool("Idle", false);
            animator.SetBool("@Follow", true);
            _isFollowing = true;
            bb.GetVariable<Variable<bool>>("isFollow").Value = true;
            npcSetting &= ~NPCSetting.CanFollow;
            GameManager.Instance.Player.WithNpc = gameObject.transform;
        }

        public void Deactivate()
        {
            if (this == null) return;
            if (GameManager.Instance.Player.IsDead) gameObject.SetActive(false);
            GameManager.Instance.OnGameover -= Deactivate;
        }

        public void MoveToDestination()
        {
            if (destination == null) return;

            bb.GetVariable<Variable<bool>>("isFollow").Value = false;
            animator.SetBool("@Follow", false);
            animator.SetBool("@Crouch", false);
            animator.SetBool("Idle", false);
            animator.SetBool("Walk", false);
            animator.SetBool("Run", true);
            animator.SetBool("Dead", false);

            Agent.isStopped = false;
            Agent.SetDestination(destination.position);
            Agent.speed = NpcData.Data.BaseSpeed * NpcData.Data.RunSpeedModifier;
            _isMoving =true;
            time = 0f;
        }

        public void Moving()
        {
            time += Time.deltaTime;
            if (time > 0.4f)
            {
                time = 0f;

                if (Agent.pathPending) return;
                if (Agent.hasPath) return;

                if (Agent.remainingDistance < 1f)
                {
                    animator.SetBool("@Follow", false);
                    animator.SetBool("@Crouch", false);
                    animator.SetBool("Idle", true);
                    animator.SetBool("Walk", false);
                    animator.SetBool("Run", false);
                    animator.SetBool("Dead", false);
                    Agent.isStopped = true;
                    _isMoving = false;
                    onNpcStop?.Invoke(GetNpcUnit());
                    onNpcStop = null;
                    return;
                }
            }            
        }

        public void PauseMove()
        {
            PrevSpeed = animator.speed;
            animator.speed = 0;
            WasStopped = Agent.isStopped;
            Agent.isStopped = true;
        }

        public void ResumeMove()
        {
            animator.speed = PrevSpeed;
            Agent.isStopped = WasStopped;
        }

        private void OnEnable()
        {
            GameManager.Instance.OnTimeStop += PauseMove;
            GameManager.Instance.OffTimeStop += ResumeMove;
        }

        private void OnDisable()
        {
            GameManager.Instance.OnTimeStop -= PauseMove;
            GameManager.Instance.OffTimeStop -= ResumeMove;
        }

        public void SetInjury(bool isInjured)
        {
            IsInjured = isInjured;
            animator.SetLayerWeight(1, isInjured ? 1f : 0f);
            if (isInjured)
            {
                bb.GetVariable<Variable<float>>("baseSpeed").Value = InjuryData.Data.BaseSpeed;
                bb.GetVariable<Variable<float>>("walkModifier").Value = InjuryData.Data.WalkSpeedModifier;
                bb.GetVariable<Variable<float>>("runModifier").Value = InjuryData.Data.RunSpeedModifier;
                bb.GetVariable<Variable<float>>("crouchModifier").Value = InjuryData.Data.CrouchSpeedModifier;                
            }
            else
            {
                bb.GetVariable<Variable<float>>("baseSpeed").Value = NpcData.Data.BaseSpeed;
                bb.GetVariable<Variable<float>>("walkModifier").Value = NpcData.Data.WalkSpeedModifier;
                bb.GetVariable<Variable<float>>("runModifier").Value = NpcData.Data.RunSpeedModifier;
                bb.GetVariable<Variable<float>>("crouchModifier").Value = NpcData.Data.CrouchSpeedModifier;
            }
        }
    }
}

