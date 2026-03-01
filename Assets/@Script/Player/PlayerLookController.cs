using Cinemachine;
using Unity.Mathematics;
using UnityEngine;

public class PlayerLookController : MonoBehaviour
{
    [Header("References")]
    [field: SerializeField] private Player Player { get; set; }
    [field: SerializeField] private Transform PlayerTransform { get; set; }
    public CinemachineVirtualCamera playerVC;
 
    [Header("Mouse Settings")]
    public float sensitivityX = 0.5f;
    public float sensitivityY = 0.5f;
    public bool smoothLook = false;
    public float smoothSpeed = 5f;

    [Header("Rotation Limits")]
    public float minVertical = -80f;
    public float maxVertical = 90f;
    public float minHorizontal = -360f;
    public float maxHorizontal = 360f;

    private Vector2 lookRotation;
    private Quaternion targetYRotation;
    private Quaternion targetXRotation;
    private CinemachineBasicMultiChannelPerlin _noise;
    private bool blockLook;

    [Header("Input System")]
    private Vector2 inputDelta;

    [field: Header("HeadMove")]
    [field: SerializeField] public NoiseSettings RunHeadMove { get; private set; }
    [field: SerializeField] public NoiseSettings WalkHeadMove { get; private set; }
    [field: SerializeField] public NoiseSettings CrouchHeadMove { get; private set; }

    public bool LookLocked
    {
        get => blockLook;
        set => blockLook = value;
    }

    private void Awake()
    {
        _noise = playerVC.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        lookRotation.x = PlayerTransform.localEulerAngles.y;
        lookRotation.y = transform.localEulerAngles.x;
        if (lookRotation.y > 180f) lookRotation.y -= 360f;

        targetYRotation = transform.localRotation;
        targetXRotation = PlayerTransform.localRotation;
        //ChangeSensitivity(15f);
        if (Player.CanHeadMove)
        {
            _noise.m_AmplitudeGain = 0.5f;
            _noise.m_FrequencyGain = 0.5f;
        }
        else
        {
            _noise.m_AmplitudeGain = 0f;
            _noise.m_FrequencyGain = 0f;
        }
    }

    private void Update()
    {
        if (!blockLook && Cursor.lockState != CursorLockMode.None)
        {
            inputDelta = Player.Input.Player.Look.ReadValue<Vector2>();
            if (Player.CanHeadMove)
            {
                _noise.m_AmplitudeGain = 0.5f;
                _noise.m_FrequencyGain = 0.5f;

                HeadMove();
            }
            else
            {
                _noise.m_AmplitudeGain = 0f;
                _noise.m_FrequencyGain = 0f;
            }
        }
        else inputDelta = Vector2.zero;

        lookRotation.x += inputDelta.x * sensitivityX * Time.deltaTime;
        lookRotation.y -= inputDelta.y * sensitivityY * Time.deltaTime;
        if (minHorizontal != -360f || maxHorizontal != 360f) lookRotation.x = Mathf.Clamp(lookRotation.x, minHorizontal, maxHorizontal);
        lookRotation.y = Mathf.Clamp(lookRotation.y, minVertical, maxVertical);
        ApplyRotation(lookRotation);
    }


    void ApplyRotation(Vector2 rotation)
    {
        Quaternion xRot = Quaternion.Euler(0f, rotation.x, 0f);
        Quaternion yRot = Quaternion.Euler(rotation.y, 0f, 0f);
        targetYRotation = yRot;
        targetXRotation = xRot;

        if (smoothLook)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetYRotation, smoothSpeed * Time.deltaTime);
            PlayerTransform.localRotation = Quaternion.Slerp(PlayerTransform.localRotation, targetXRotation, smoothSpeed * Time.deltaTime);
        }
        else
        {
            transform.localRotation = targetYRotation;
            PlayerTransform.localRotation = targetXRotation;
        }
    }

    public void ChangeSensitivity(float value)
    {
        //float sensitivity = Mathf.Lerp(0.001f, 0.6f, Mathf.InverseLerp(10f, 50f, value));
        //sensitivityX = sensitivity;
        //sensitivityY = sensitivity;
        sensitivityX = value;
        sensitivityY = value;
    }

    public void PlayerAngle(float rotation)
    {
        lookRotation.x = rotation;
        Quaternion xRot = Quaternion.Euler(0f, lookRotation.x, 0f);
        PlayerTransform.localRotation = xRot;
    }

    private void HeadMove()
    {
        if (Player.CurState.HasFlag(PlayerEnum.PlayerState.Run) && Player.CurState.HasFlag(PlayerEnum.PlayerState.Move))
        {
            _noise.m_NoiseProfile = RunHeadMove;
        }
        else if (Player.CurState.HasFlag(PlayerEnum.PlayerState.Walk) && Player.CurState.HasFlag(PlayerEnum.PlayerState.Move))
        {
            _noise.m_NoiseProfile = WalkHeadMove;
        }
        else
        {
            _noise.m_NoiseProfile = CrouchHeadMove;
        }


        //else if (Player.CurState.HasFlag(PlayerEnum.PlayerState.Idle))
        //{
        //    _noise.m_NoiseProfile = CrouchHeadMove;
        //}
    }

    public void StopHeadMove()
    {
        _noise.m_NoiseProfile = null;
        _noise.m_AmplitudeGain = 0f;
    }

    public void PlayHeadMove()
    {
        _noise.m_AmplitudeGain = 0.5f;
    }
}
