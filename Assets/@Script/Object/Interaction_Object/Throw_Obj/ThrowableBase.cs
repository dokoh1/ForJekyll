using UnityEngine;

/// <summary>
/// GlassBottle / TimerLure / BrokenTimerLure 공통 베이스.
/// PickUp, ThrowObject, INoise 스텁, 홀드포인트 세팅 등 공통 로직 담당.
/// 서브클래스는 OnPickedUp() / OnThrown() 훅만 override.
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public abstract class ThrowableBase : InteractableBase, INoise, IThrowable
{
    [Header("Interact Settings")]
    [SerializeField] private InteractTypeEnum interactType = InteractTypeEnum.Tap;
    [SerializeField] private bool isInteractable = true;
    [SerializeField] private float interactHoldTime = 0f;

    [Header("Throw Settings")]
    [SerializeField] protected float throwForce = 10f;
    [SerializeField] private Transform holdPointOverride;
    [SerializeField] private Vector3 holdRotationEuler;

    [Header("Highlight")]
    [SerializeField] private Renderer highlightRenderer;
    [SerializeField] private Material defaultMaterial;

    [Header("Audio")]
    [SerializeField] protected AudioClip pickupClip;
    [SerializeField] protected AudioClip throwClip;
    [SerializeField] protected AudioSource audioSource;

    protected Rigidbody _rb;
    protected Transform _holdPoint;
    protected bool _isHeld;
    protected PlayerInteractable _playerInteractable;

    // INoise 구현
    public float CurNoiseAmount { get; set; }
    [field: SerializeField] public float NoiseCheckAmount { get; set; }
    public float SumNoiseAmount { get; set; }
    public float DecreaseSpeed { get; set; }
    public float MaxNoiseAmount { get; set; }

    // InteractableBase 프로퍼티
    public override InteractTypeEnum InteractType
    {
        get => interactType;
        set => interactType = value;
    }

    public override bool IsInteractable
    {
        get => isInteractable;
        set => isInteractable = value;
    }

    public override float InteractHoldTime
    {
        get => interactHoldTime;
        set => interactHoldTime = value;
    }

    protected override void Start()
    {
        base.Start();
        _rb = GetComponent<Rigidbody>();

        _holdPoint = holdPointOverride != null
            ? holdPointOverride
            : GameManager.Instance.Player.ThrowHoldPoint;

        _playerInteractable = GameManager.Instance.Player.PlayerInteractable;
    }

    public override void Interact()
    {
        if (!IsInteractable)
            return;

        if (!_isHeld)
            PickUp();
    }

    public override void Hold() { }

    protected void PickUp()
    {
        if (_holdPoint == null)
            return;

        _isHeld = true;

        _rb.isKinematic = true;
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;

        transform.SetParent(_holdPoint, false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = holdRotationEuler != Vector3.zero
            ? Quaternion.Euler(holdRotationEuler)
            : Quaternion.identity;

        _playerInteractable?.SetHeldInteractable(this);

        if (audioSource != null && pickupClip != null)
            audioSource.PlayOneShot(pickupClip);

        if (highlightRenderer != null && defaultMaterial != null)
            GameManager.Instance.HighLightMaterialDelete(highlightRenderer, defaultMaterial);

        OnPickedUp();
    }

    public void ThrowObject()
    {
        if (!_isHeld)
            return;

        _isHeld = false;

        transform.SetParent(null);

        _rb.isKinematic = false;
        _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;

        Camera cam = Camera.main;
        Vector3 dir = cam != null ? cam.transform.forward : _holdPoint.forward;
        const float MinUpwardThrowAngle = 0.05f;
        dir.y = Mathf.Max(dir.y, MinUpwardThrowAngle);

        _rb.AddForce(dir * throwForce, ForceMode.VelocityChange);

        _playerInteractable?.SetHeldInteractable(null);

        if (audioSource != null && throwClip != null)
            audioSource.PlayOneShot(throwClip);

        OnThrown();
    }

    protected virtual void OnPickedUp() { }
    protected virtual void OnThrown() { }
}
