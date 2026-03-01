using UnityEngine;
using UnityEngine.UI;

public class PlayerInteractable : MonoBehaviour
{
    [Header("CurInteractInfo")]
    [field: SerializeField] private GameObject curInteractGameObject;
    public IInteractable CurInteractable
    {
        get
        {
            if (_curInteractable == null) return null;
            else return _curInteractable;
        }
        private set
        {
            _curInteractable = value;
        }
    }
    public bool isCarry = false;
    private IInteractable _curInteractable;
    private bool _isHoldInterating;
    
    public InteractableBase HeldInteractable { get; private set; }

    public void SetHeldInteractable(InteractableBase interactable)
    {
        HeldInteractable = interactable;
        isCarry = interactable != null;
    }

    [Header("Raycast")]
    public float maxCheckDistance = 10f;
    public LayerMask layerMask;
    public LayerMask obstacleMask;
    public LayerMask npcMask;
    private Camera _camera;

    [Header("Reticle")]
    [field: SerializeField] private GameObject reticle;
    [field: SerializeField] private Sprite dotImg;
    [field: SerializeField] private Sprite ringImg;
    [field: SerializeField] private Sprite holdImg;
    [field: SerializeField] private Image holdInteract;
    private float _holdDuration = 0f;
    private Image _reticleImage;

    [Header("UI")]
    public GameObject interactUI;
    public GameObject playerUI;
    [field: SerializeField] private Sprite InteractImg;
    [field: SerializeField] private Sprite carryImg;
    [field: SerializeField] private Image InteractIcon;

    [field: SerializeField] public bool ShowInteractUI { get; set; } = true;

    private Player _player;

    private void Awake()
    {
        _reticleImage = reticle.GetComponent<Image>();
        _camera = Camera.main;
    }

    private void Start()
    {
        _player = GameManager.Instance.Player;
        // playerUI.SetActive(true);
    }

    private void FixedUpdate()
    {
        PerformRaycast();
        if (_isHoldInterating) CheckHoldInteract();
    }

    private void PerformRaycast()
    {   
        Ray ray = _camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        //if (Physics.Raycast(ray, out hit, maxCheckDistance, npcMask))
        //{
        //    if (_player.CarryNpc != null && _player.CarryNpc.IsInjured)
        //    {
        //        InteractIcon.sprite = carryImg;
        //        isCarry = true;
        //    }
        //    else
        //    {
        //        InteractIcon.sprite = InteractImg;
        //        isCarry = false;

        //        if (GameManager.Instance.IsTimeStop)
        //        {
        //            curInteractGameObject = null;
        //            _curInteractable = null;
        //            SetDotImg();
        //            return;
        //        }
        //    }            
        //}
        //else
        //{
        //    InteractIcon.sprite = InteractImg;
        //    isCarry = false;
        //}

        float obstacleDistance = Mathf.Infinity;
        if (Physics.Raycast(ray, out hit, maxCheckDistance, obstacleMask))
        {
            obstacleDistance = hit.distance;
        }

        if (Physics.Raycast(ray, out hit, maxCheckDistance, layerMask) && hit.distance < obstacleDistance)
        {
            
            if (hit.collider.gameObject != curInteractGameObject)
            {
                curInteractGameObject = hit.collider.gameObject;
                
                _curInteractable = hit.collider.GetComponent<IInteractable>();

                if (_curInteractable == null) return;

                if (_curInteractable.InteractType == InteractableBase.InteractTypeEnum.TapAndHold)
                {
                    if (_curInteractable.CanTap || _curInteractable.CanHold)
                    {
                        //Debug.LogWarning($"PlayerInteractable - InteractTypeEnum.TapAndHold - can tap or hold.");
                        SetRingImg();
                        return;
                    }  
                }

                if (!_curInteractable.IsInteractable) return;

                //if (_curInteractable == null || !_curInteractable.IsInteractable) return;
                SetRingImg();
                return;
            }

            if (_curInteractable.InteractType == InteractableBase.InteractTypeEnum.TapAndHold)
            {
                if (_curInteractable.CanTap || _curInteractable.CanHold)
                {
                    //Debug.LogWarning($"PlayerInteractable - InteractTypeEnum.TapAndHold - can tap or hold - 2.");
                    SetRingImg();
                    return;
                }
            }

            if (!_curInteractable.IsInteractable) SetDotImg();
            else SetRingImg();
        }
        else
        {
            curInteractGameObject = null;
            _curInteractable = null;
            SetDotImg();
        }
    }

    public void SetUIPrompt()
    {
        if (_curInteractable != null)
        {
            if (_curInteractable.InteractType == InteractableBase.InteractTypeEnum.TapAndHold)
            {
                if (_curInteractable.CanTap || _curInteractable.CanHold)
                {
                    if (!ShowInteractUI) return;
                    interactUI.SetActive(true);//E 아이콘 출력
                    return;
                }
            }

            if (_curInteractable.IsInteractable)
            {
                if (!ShowInteractUI) return;
                interactUI.SetActive(true);//E 아이콘 출력
                return;
            }            
        }
        interactUI.SetActive(false);
    }

    public void OnInteracted()
    {
        //Debug.Log("PlayerInteractable - OnInteracted() called");
        //if (_curInteractable == null || !_curInteractable.IsInteractable) return;

        if (_curInteractable == null)
        {
            //Debug.LogWarning("PlayerInteractable - OnInteracted() - _curInteractable is null.");
            return;
        }

        if (_curInteractable.InteractType == InteractableBase.InteractTypeEnum.TapAndHold)
        {
            //Debug.LogWarning($"PlayerInteractable - OnInteracted() - t: {_curInteractable.CanTap}, h : {_curInteractable.CanHold}");

            if (_curInteractable.CanTap)
            {
                _curInteractable.Interact();
                SetUIPrompt();
                return;
            }
            else if (_curInteractable.CanHold)
            {
                _isHoldInterating = true;
                return;
            }
        }

        if (_curInteractable.InteractHoldTime > 0f && !_curInteractable.IsTabAndHold)
        {
            _isHoldInterating = true;
        }
        else
        {
            _curInteractable.Interact();
            SetUIPrompt();
        }
    }

    public void OnHoldInteracted()
    {
        _isHoldInterating = true;
    }

    public void OffInteracted()
    {
        if (_curInteractable == null) return;

        _isHoldInterating = false;
        _holdDuration = 0f;
        holdInteract.fillAmount = 0f;
    }

    private void CheckHoldInteract()
    {
        if (_curInteractable.IsTabAndHold && !_curInteractable.CanHold) return;

        if (_curInteractable == null || _curInteractable.InteractHoldTime <= 0f || !_curInteractable.IsInteractable)
        {
            _isHoldInterating = false;
            _holdDuration = 0f;
            holdInteract.fillAmount = 0f;
            return;
        }

        _holdDuration += Time.deltaTime;
        holdInteract.fillAmount = Mathf.Clamp01(_holdDuration / _curInteractable.InteractHoldTime);
        
        if (_holdDuration >= _curInteractable.InteractHoldTime)
        {
            _curInteractable.Hold();
            SetUIPrompt();
            _holdDuration = 0f;
            holdInteract.fillAmount = 0f;
        }
    }

    private void SetDotImg()
    {
            if (!_player.IsTestMode) UIManager.Instance.objective.SetHidden(HideReason.Hover, false, 0.1f);
        // bool showCrosshair = _player.ShowCrosshair;
        // if (showCrosshair && !reticle.activeSelf) reticle.SetActive(showCrosshair);
        // else if (!showCrosshair && reticle.activeSelf) reticle.SetActive(showCrosshair);

        if (_reticleImage.sprite != dotImg)
            _reticleImage.sprite = dotImg;
        
        reticle.transform.localScale = Vector3.one;
        SetUIPrompt();
    }

    private void SetRingImg()
    {
        if (!_player.IsTestMode) UIManager.Instance.objective.SetHidden(HideReason.Hover, true, 0.1f);
        // bool showCrosshair = _player.ShowCrosshair;
        // if (showCrosshair && !reticle.activeSelf) reticle.SetActive(showCrosshair);
        // else if (!showCrosshair && reticle.activeSelf) reticle.SetActive(showCrosshair);
        
        if (_reticleImage.sprite != ringImg)
            _reticleImage.sprite = ringImg;

        reticle.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
        // set ui text, image 
        SetUIPrompt();
    }
}
