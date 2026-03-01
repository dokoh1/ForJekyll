using Michsky.UI.Dark;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_SavePanel : UI_Base
{
    private MainPanelManager mainPanelManager;

    protected override void Awake()
    {
        base.Awake();
        gameObject.SetActive(false);

        foreach (Button btn in gameObject.FindChilds<Button>("Button_Close", recursive: true))
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(ClosePanel);
        }
    }

    public void OpenPanel()
    {
        UI_PauseMenuPopup.Instance.SwitchActiveESC(false);
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.Saving, true);
        GameManager.Instance.fadeManager.SceneLoading += ClosePanel;
        PManagers.Instance.input.GameUI.ESC.performed += HandleESCInput;
        gameObject.SetActive(true);
    }

    public void ClosePanel()
    {
        UI_PauseMenuPopup.Instance.SwitchActiveESC(true);
        GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.Saving, false);
        GameManager.Instance.fadeManager.SceneLoading -= ClosePanel;
        PManagers.Instance.input.GameUI.ESC.performed -= HandleESCInput;
        gameObject.SetActive(false);
    }

    private void HandleESCInput(InputAction.CallbackContext context)
    {
        ClosePanel();
    }
} 
