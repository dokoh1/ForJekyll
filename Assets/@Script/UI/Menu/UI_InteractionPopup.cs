using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_InteractionPopup : SingletonManager<UI_InteractionPopup>
{
    [SerializeField] private GameObject interaction;
    [SerializeField] private Image interactionImage;
    [SerializeField] private TextMeshProUGUI interactionText;

    public bool IsOpen { get; set; } = false;
    [SerializeField] private UIScriptData scriptData;


    public void OpenPopup()
    {
        GameManager.Instance.IsPaused = true;
        IsOpen = true;
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.PlayerStop, true);
        interaction.SetActive(true);
    }

    public void ClosePopup()
    {
        GameManager.Instance.IsPaused = false;
        IsOpen = false;
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.PlayerMove, true);
        interaction.SetActive(false);
    }

    public void SetInteractionUI(Sprite sprite, string objectID)
    {
        if (interactionImage != null)
        {
            interactionImage.sprite = sprite;
        }
        if (interactionText != null)
        {
            string interactText = scriptData.GetText(objectID, ScriptDataType.Object);
            interactionText.text = interactText;
        }
    }
}