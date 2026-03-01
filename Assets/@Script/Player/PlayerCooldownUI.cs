using UnityEngine;
using UnityEngine.UI;

public class PlayerCooldownUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Image hideIconImage;
    [SerializeField] private Player player;

    [Header("Settings")]
    public PlayerEnum.SkillIcon skillIconType = PlayerEnum.SkillIcon.HideWhenReady;

    private bool _isUse = true;
    private bool _isCooling;
    private float _endTime;
    private float _cooldownSeconds;

    public bool IsCooling => _isCooling;

    private void Start()
    {
        if (iconImage == null || hideIconImage == null)
        {
            Debug.LogError("PlayerCooldownUI: iconImage or hideIconImage is not assigned.");
            return;
        }

        if (hideIconImage.enabled) hideIconImage.enabled = false;

        if (skillIconType == PlayerEnum.SkillIcon.AlwaysShow)
        {
            if (!iconImage.enabled) iconImage.enabled = true;
        }
        else if (skillIconType == PlayerEnum.SkillIcon.HideWhenReady)
        {
            if (iconImage.enabled) iconImage.enabled = false;
        }
        else if (skillIconType == PlayerEnum.SkillIcon.AlwaysHide)
        {
            if (iconImage.enabled) iconImage.enabled = false;
        }

        Init();
    }

    public void Init()
    {
        if (!GameManager.Instance.Player.UseSkill) _isUse = false;
        else _isUse = true;
    }

    public void StartCooldown(float cooldownSeconds)
    {
        if (iconImage == null || hideIconImage == null)
        {
            Debug.LogError("PlayerCooldownUI: iconImage or hideIconImage is not assigned.");
            return;
        }

        _cooldownSeconds = Mathf.Max(0.0001f, cooldownSeconds);
        _endTime = Time.time + _cooldownSeconds;
        _isCooling = true;

        if (skillIconType == PlayerEnum.SkillIcon.AlwaysShow || skillIconType == PlayerEnum.SkillIcon.HideWhenReady)
        {
            hideIconImage.fillAmount = 1f;
            if (!hideIconImage.enabled) hideIconImage.enabled = true;
            if (!iconImage.enabled) iconImage.enabled = true;
        }              
    }

    private void Update()
    {
        if (!_isUse) return;
        if (iconImage == null || hideIconImage == null) return;

        if (_isCooling)
        {
            float remainingTime = _endTime - Time.time;
            if (remainingTime <= 0f)
            {
                _isCooling = false;
                remainingTime = 0f;
                if (hideIconImage.enabled) hideIconImage.enabled = false;

                if (skillIconType == PlayerEnum.SkillIcon.AlwaysShow)
                {
                    if (!iconImage.enabled) iconImage.enabled = true; 
                }
                else if (skillIconType == PlayerEnum.SkillIcon.HideWhenReady)
                {
                    if (iconImage.enabled) iconImage.enabled = false;
                }
                else if (skillIconType == PlayerEnum.SkillIcon.AlwaysHide)
                {
                    if (iconImage.enabled) iconImage.enabled = false;
                }

                return;
            }

            if (skillIconType == PlayerEnum.SkillIcon.AlwaysShow || skillIconType == PlayerEnum.SkillIcon.HideWhenReady)
            {
                hideIconImage.fillAmount = Mathf.Clamp01(remainingTime / _cooldownSeconds);
            }
        }
    }

    public void SetSkillEvent()
    {
        _isCooling = false;
        if (iconImage.enabled) iconImage.enabled = false;
        if (hideIconImage.enabled) hideIconImage.enabled = false;
    }

    public void ResetCooldown()
    {
        _isCooling = false;
        if (iconImage.enabled) iconImage.enabled = false;
        if (hideIconImage.enabled) hideIconImage.enabled = false;
    }
}