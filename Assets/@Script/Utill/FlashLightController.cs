using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FlashLightController : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Light flashlightLight;
    [SerializeField] private GameObject flashLightAnims;
    [SerializeField] private Animator animator;
    [SerializeField] private BoxCollider flashCollider;


    //public Animator animator;
    public string FlashlightDrawState = "FlashlightDraw";
    public string FlashlightHideState = "FlashlightHide";
    public string FlashlightReloadState = "FlashlightReload";
    public string FlashlightIdleState = "FlashlightIdle";

    public float FlashlightHideTrim = 0.5f;

    public string FlashlightHideTrigger = "Hide";
    public string FlashlightReloadTrigger = "Reload";

    public AudioClip FlashlightClickOn;
    public AudioClip FlashlightClickOff;

    public GameObject FlashObject { get { return flashLightAnims; } }

    [field: SerializeField] public PlayerAnimationData AnimationData { get; private set; }
    [SerializeField] private bool flashlight_Enable { get; set; }
    [SerializeField] private bool aniEnd { get; set; }

    private void Awake()
    {
        if (player == null) player = GameManager.Instance.Player;
        if (flashLightAnims == null) flashLightAnims = GameObject.FindWithTag("PlayerFlash");
        if (audioSource == null) audioSource = flashLightAnims.GetComponent<AudioSource>();
        if (animator == null) animator = flashLightAnims.GetComponent<Animator>();
        if (flashlightLight == null) flashlightLight = flashLightAnims.GetComponentInChildren<Light>();
    }

    private void Start()
    {        
        aniEnd = true;
        if (!ScenarioManager.Instance.GetAchieve(ScenarioAchieve.PlayerFlashLight))
        {
            flashlight_Enable = false;
            flashLightAnims?.SetActive(false);
        }
    }

    // Player 의 SetFlash() 메서드에서 호출 할 것(on, off 기능)

    //public void FlashLighOn()
    //{
    //    flashlight_Enable = true;
    //    flashLightAnims?.SetActive(true);
    //}
    //public void FlashLightOff()
    //{
    //    flashlight_Enable = false;
    //    flashLightAnims?.SetActive(false);
    //}

    public void ToggleFlashLight()
    {
        if (aniEnd)
        {
            aniEnd = false;
            if (flashlight_Enable)
            {
                //Debug.Log("Flashlight Hide");

                //player.FlashCollider.layer = 0;
                flashCollider.enabled = false;
                player.CurState &= ~PlayerEnum.PlayerState.Flash;

                StartCoroutine(HideFlashlight());
                Animator.SetTrigger(FlashlightHideTrigger);
            }
            else
            {
                //Debug.Log("Flashlight Draw");
                //player.FlashCollider.layer = 18;
                flashCollider.enabled = true;
                player.CurState |= PlayerEnum.PlayerState.Flash;

                StartCoroutine(IdleFlashlight());
                //Animator.SetTrigger(FlashlightIdleState);
            }
        }
    }

    IEnumerator IdleFlashlight()
    {
        flashLightAnims.SetActive(true);
        yield return new WaitForAnimatorClip(Animator, FlashlightIdleState);
        SetLightState(true);
        aniEnd = true;
        flashlight_Enable = true;
    }
    IEnumerator HideFlashlight()
    {
        yield return new WaitForAnimatorClip(Animator, FlashlightHideState, FlashlightHideTrim);

        SetLightState(false);
        aniEnd = true;
        flashlight_Enable = false;
        flashLightAnims.SetActive(false);
    }
    public void SetLightState(bool state)
    {
        flashlightLight.enabled = state;
        if (!state) audioSource.PlayOneShot(FlashlightClickOff);
        else audioSource.PlayOneShot(FlashlightClickOn);
    }

    public Animator Animator
    {
        get
        {
            if (animator == null)
                animator = GetComponentInChildren<Animator>();

            return animator;
        }
    }

    public void FlashAnimationOn(PlayerEnum.PlayerState playerState)
    {
        if (playerState == PlayerEnum.PlayerState.Run)
        {
            Animator.SetBool(AnimationData.WalkParameterHash, false);
            Animator.SetBool(AnimationData.RunParameterHash, true);
            Animator.SetBool(AnimationData.CrouchParameterHash, false);
            Animator.SetBool(AnimationData.IdleParameterHash, false);
        }
        else if (playerState == PlayerEnum.PlayerState.Crouch)
        {
            Animator.SetBool(AnimationData.WalkParameterHash, false);
            Animator.SetBool(AnimationData.RunParameterHash, false);
            Animator.SetBool(AnimationData.CrouchParameterHash, true);
            Animator.SetBool(AnimationData.IdleParameterHash, false);
        }
        else if (playerState == PlayerEnum.PlayerState.Walk)
        {
            Animator.SetBool(AnimationData.WalkParameterHash, true);
            Animator.SetBool(AnimationData.RunParameterHash, false);
            Animator.SetBool(AnimationData.CrouchParameterHash, false);
            Animator.SetBool(AnimationData.IdleParameterHash, false);
        }
    }

    public void FlashAnimationOff()
    {
        Animator.SetBool(AnimationData.WalkParameterHash, false);
        Animator.SetBool(AnimationData.RunParameterHash, false);
        Animator.SetBool(AnimationData.CrouchParameterHash, false);
        Animator.SetBool(AnimationData.IdleParameterHash, true);
    }
    
    public void FlashEnableToggle(bool value) { flashlightLight.enabled = value; }

    private float _defaultIntensity = -1f;

    public void SetIntensityOverride(float intensity)
    {
        if (_defaultIntensity < 0f)
            _defaultIntensity = flashlightLight.intensity;
        flashlightLight.intensity = intensity;
    }

    public void ResetIntensity()
    {
        if (_defaultIntensity >= 0f)
        {
            flashlightLight.intensity = _defaultIntensity;
            _defaultIntensity = -1f;
        }
    }

    private Coroutine _flickerRoutine;

    public void Flicker(float duration, int flickerCount)
    {
        if (!flashlightLight.enabled) return;
        if (_flickerRoutine != null) StopCoroutine(_flickerRoutine);
        _flickerRoutine = StartCoroutine(FlickerRoutine(duration, flickerCount));
    }

    private IEnumerator FlickerRoutine(float duration, int flickerCount)
    {
        float interval = duration / (flickerCount * 2);
        float originalIntensity = flashlightLight.intensity;

        for (int i = 0; i < flickerCount; i++)
        {
            flashlightLight.intensity = originalIntensity * 0.1f;
            yield return new WaitForSecondsRealtime(interval);
            flashlightLight.intensity = originalIntensity;
            yield return new WaitForSecondsRealtime(interval);
        }

        flashlightLight.intensity = originalIntensity;
        _flickerRoutine = null;
    }

    public class WaitForAnimatorClip : CustomYieldInstruction
    {
        const string BaseLayer = "Base Layer";

        private readonly Animator animator;
        private readonly float timeOffset;
        private readonly int stateHash;

        private bool isStateEntered;
        private float stateWaitTime;
        private float timeWaited;

        public WaitForAnimatorClip(Animator animator, string state, float timeOffset = 0, bool normalized = false)
        {
            this.animator = animator;
            this.timeOffset = timeOffset;
            stateHash = Animator.StringToHash(BaseLayer + "." + state);
        }

        public override bool keepWaiting
        {
            get
            {
                AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);

                if (info.fullPathHash == stateHash && !isStateEntered)
                {
                    float stateLength = info.length / info.speed;
                    stateWaitTime = stateLength - timeOffset;
                    isStateEntered = true;
                }
                else if (isStateEntered)
                {
                    if (timeWaited < stateWaitTime)
                        timeWaited += Time.deltaTime;
                    else return false;
                }

                return true;
            }
        }
    }
}
