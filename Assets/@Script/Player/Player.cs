using DG.Tweening;
using NPC;
using UnityEngine;
using UnityEngine.Rendering;
using static UnitEnum;

public class Player : MonoBehaviour, INoise, IAttackable, IDeactivate, IPausableAudio
{
    [field: Header("References")]
    [field: SerializeField] public PlayerDataSO Data { get; private set; }
    [field: SerializeField] public InputController Input { get; private set; }
    [field: SerializeField] public Transform FPView { get; private set; }
    [field: SerializeField] public PlayerLookController PlayerLookController { get; private set; }
    [field: SerializeField] public GameObject PlayerCrosshairUI { get; private set; }
    [field: SerializeField] public ChaseBGMController BGMController { get; private set; }
    [field: SerializeField] public Transform ThrowHoldPoint;
    public CharacterController Controller { get; private set; }
    public PlayerInteractable PlayerInteractable { get; private set; }
    public JumpScareManager JumpScareManager { get; private set; }
    public PlayerSkillEffect PlayerSkillEffect { get; private set; }    
    public PlayerUIBlinker PlayerUIBlinker { get; private set; }
    //public PlayerCooldownUI PlayerCooldownUI { get; private set; }
    public PlayerSoundSystem PlayerSoundSystem { get; private set; }
    [field: SerializeField] private PlayerDetectSystem PlayerDetectSystem { get; set; }

    [field: Header("State")]
    [field: SerializeField] public PlayerEnum.PlayerState CurState { get; set; }
    public bool CanLook { get; set; }
    public bool IsDead { get; set; } = false;

    // INoise
    [field: Header("Noise")]
    [field: SerializeField] public float CurNoiseAmount { get; set; }
    [field: SerializeField] public float NoiseCheckAmount { get; set; }
    public float SumNoiseAmount { get; set; }
    public float DecreaseSpeed { get; set; } = 5.0f;
    public float MaxNoiseAmount { get; set; } = 13f;

    [field: Header("Flash")]
    [field: SerializeField] public GameObject FlashCollider { get; private set; }
    [field: SerializeField] public Transform FlashTransform { get; private set; }
    [field: SerializeField] public bool IsDetectFlash { get; set; }
    [field: SerializeField] private bool StartUseFlash { get; set; }
    public FlashLightController Flash { get; private set; }
    public float FlashRange { get; set; }

    public LayerMask flashObstacle;

    //Karma
    [field: Header("Karma")]
    [field: SerializeField] public float KarmaPoint { get; set; }

    [field: Header("Npc")]
    [field: SerializeField] public Transform WithNpc { get; set; }
    public bool CanCarry = false;
    public NpcUnit CarryNpc { get; set; }
    public bool IsCarrying = false;
    public PlayerStuckDetector StuckDetector { get; private set; }
    public PlayerDropCheck DropCheck { get; private set; }

    [field: Header("Test")]
    public bool IsTestMode = false;

    [field: SerializeField] public bool CanHeadMove { get; set; } = false;
    public bool UseSkill = true;

    // [field: SerializeField] public bool ShowCrosshair { get; set; }

    private PlayerStateMachine _stateMachine;
    public Vector3 FanForce { get; set; }
    public float CrouchSpeedModifier { get; private set; }
    private float _defaultCrouchSpeed;

    private void Awake()
    {
        //Debug.Log("Player - Awake()");
        if (PlayerSoundSystem == null) PlayerSoundSystem = GetComponent<PlayerSoundSystem>();
        if (Controller == null) Controller = GetComponent<CharacterController>();
        if (PlayerInteractable == null) PlayerInteractable = GetComponent<PlayerInteractable>();
        if (PlayerLookController == null) PlayerLookController = GetComponent<PlayerLookController>();
        if (JumpScareManager == null) JumpScareManager = GetComponent<JumpScareManager>();
        if (PlayerSkillEffect == null) PlayerSkillEffect = GetComponent<PlayerSkillEffect>();
        //if (PlayerCooldownUI == null) PlayerCooldownUI = GetComponent<PlayerCooldownUI>();
        if (PlayerUIBlinker == null) PlayerUIBlinker = GetComponent<PlayerUIBlinker>();
        if (Flash == null) Flash = GetComponent<FlashLightController>();

        _stateMachine = new PlayerStateMachine(this);
        StuckDetector = new PlayerStuckDetector(transform);
        DropCheck = new PlayerDropCheck(transform);
        GameManager.Instance.Player = this;
        //DOTweenAnimeManager.AddExceptionId("TimeStop");
        
        var volume = GameObject.FindWithTag("Volume").GetComponent<Volume>();
        PlayerSkillEffect.AddVolume(volume);

        _defaultCrouchSpeed = Data.GroundData.CrouchSpeedModifier;
        CrouchSpeedModifier = _defaultCrouchSpeed;
    }

    private void Start()
    {
        if (IsTestMode)
        {
            Input.PlayerInputSwitch(true);
            PlayerSetting(true);
            // ShowCrosshair = true;
        }

        if (StartUseFlash)
        {
            SetFlash();
        }

        //Input.PlayerSkillInputSetting(UseSkill);

        GameManager.Instance.OnGameover += Deactivate;
        _stateMachine.ChangeState(_stateMachine.IdleState);

        FlashCollider.SetActive(IsDetectFlash);
        IsDead = false;
        FlashRange = Data.GroundData.FlashRangeZ;
        IgnorePauseOn();
        //GetSkill();
    }

    private void Update()
    {
        _stateMachine.HandleInput();
        _stateMachine.Update();
        CheckNoise();
    }

    private void FixedUpdate()
    {
        _stateMachine.PhysicsUpdate();
    }

    private void CheckNoise()
    {
        if (CurNoiseAmount > 0)
        {
            CurNoiseAmount -= DecreaseSpeed * Time.deltaTime;
            if (CurNoiseAmount <= 0) CurNoiseAmount = 0;
        }

        if (CurNoiseAmount >= MaxNoiseAmount) CurNoiseAmount = MaxNoiseAmount;
    }

    public PlayerStateMachine GetStateMachine() { return _stateMachine; }

    public void OnHitSuccess()
    {
        // gameover
        Debug.Log($"OnHitSuccess - gameover");
    }

    public void OnHitSuccess(UnitType unitType)
    {
        IsDead = true;
        GameManager.Instance.IsGameover = true;

        if (unitType == UnitType.EarTypeMonster)
        {
            Debug.Log($"귀쟁이에게 사망 gameover");
            JumpScareManager.PlayerDead(JumpScareType.EarTypeMonster);// 게임 매니저로 이동
        }
        else if (unitType == UnitType.EyeTypeMonster)
        {
            Debug.Log($"눈쟁이에게 사망 gameover");
            JumpScareManager.PlayerDead(JumpScareType.EyeTypeMonster);// 게임 매니저로 이동
        }
    }

    public void PlayerAngle(float rotation)
    {
        PlayerLookController.PlayerAngle(rotation);
    }

    public void PlayerSetting(bool move, bool interact, bool flash, bool look)
    {
        Input.PlayerInputSetting(move, interact, flash, look);
        PlayerLookController.LookLocked = !look;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        PlayerInteractable.ShowInteractUI = interact;
        PlayerCrosshairUI.SetActive(interact);
        // ShowCrosshair = interact; 
        //if (UseSkill) Input.PlayerSkillInputSetting(interact);
    }

    public void PlayerSetting(bool all)
    {
        Input.PlayerInputSetting(all, all, all, all);
        PlayerLookController.LookLocked = !all;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        PlayerInteractable.ShowInteractUI = all;
        PlayerCrosshairUI.SetActive(all);        
        // ShowCrosshair = all;
        //if (UseSkill) Input.PlayerSkillInputSetting(all);
    }

    public void SkillSetting(bool skill)
    {
        // 임시 수정 - 마스크, 시간정지 비활성화
        //if (UseSkill) Input.PlayerSkillInputSetting(skill);
        //if (UseSkill && IsTestMode) Input.PlayerSkillInputSetting(skill);
    }

    private void OnDestroy()
    {
        Input.PlayerInputSwitch(false);
        _stateMachine.ChangeState(null);
        _stateMachine = null;
        GameManager.Instance.Player = null;
    }

    public void Deactivate()
    {
        if (this == null) return;
        //Debug.Log("Player - Deactivate()");
        //PlayerCooldownUI.ResetCooldown();
        PlayerLookController.StopHeadMove();
        PlayerSetting(false);
        //Input.PlayerSkillInputSetting(false);
        Flash.FlashObject.SetActive( false );
        PlayerSoundSystem.StopAllSound();
        PlayerDetectSystem.StopDetect();
        //Debug.Log("Player - Deactivate() - DialogueCloseImmediate");
        UIManager.Instance.DialogueCloseImmediate();
        DOTween.KillAll();
        GameManager.Instance.OnGameover -= Deactivate;
    }

    public void SetFlash()
    {
        Flash.ToggleFlashLight();
    }

    //public void OnSkillEvent()
    //{
    //    Debug.Log("Player - OnSkillEvent()");
    //    GameManager.Instance.IsTimeStop = true;
    //    // 화면 이펙트 효과 로직 추가
    //    //PlayerCooldownUI.SetSkillEvent();
    //    //Input.PlayerSkillInputSetting(false);
    //    AudioListener.pause = true;
    //    //DOTween.PauseAll();
    //}

    public void OffSkillEvent()
    {
        if (!GameManager.Instance.IsTimeStop) return;
        Debug.Log("Player - OffSkillEvent()");
        //PlayerCooldownUI.ResetCooldown();
        GameManager.Instance.IsTimeStop = false;
        // 화면 이펙트 효과 로직 해제 추가
        PlayerSkillEffect.CutAllSkillEffect();
        //Input.PlayerSkillInputSetting(true);
        AudioListener.pause = false;
        //DOTween.PlayAll();
    }

    private void IgnorePauseOn()
    {
        PlayerSoundSystem.SetIgnorePause(true);
    }

    private void IgnorePauseOff()
    {
        PlayerSoundSystem.SetIgnorePause(false);
    }

    public void PauseAudio()
    {
        IgnorePauseOff();
    }

    public void ResumeAudio()
    {
        IgnorePauseOn();
    }

    private void OnEnable()
    {
        GameManager.Instance.OnPause += PauseAudio;
        GameManager.Instance.OnResume += ResumeAudio;

        if (!IsTestMode && ScenarioManager.Instance.GetAchieve(ScenarioAchieve.PlayerFlashLight))
        {
            if (!CurState.HasFlag(PlayerEnum.PlayerState.Flash)) SetFlash();
        }
    }

    private void OnDisable()
    {
        GameManager.Instance.OnPause -= PauseAudio;
        GameManager.Instance.OnResume -= ResumeAudio;
    }
    public void SetCrouchSpeedModifier(float value)
    {
        CrouchSpeedModifier = value;
    }

    public void RestoreCrouchSpeed()
    {
        CrouchSpeedModifier = _defaultCrouchSpeed;
    }
    //public void GetSkill()
    //{
    //    if (!UseSkill)
    //    {
    //        UseSkill = true;
    //        Input.PlayerSkillInputSetting(true);
    //        PlayerCooldownUI.Init();
    //    }
    //}
}