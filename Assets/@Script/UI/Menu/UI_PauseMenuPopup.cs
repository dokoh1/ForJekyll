using System;
using System.Collections;
using System.Collections.Generic;
using Michsky.UI.Dark;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class UI_PauseMenuPopup : UI_Popup
{
	public static UI_PauseMenuPopup Instance;

	[SerializeField] private AudioClip escOpenSound;
	[SerializeField] private HorizontalSelectorView horizontalSelectorView;
	private readonly Stack<PanelController> panelStack = new();
	private PanelController mainPanel;
	private bool canOpen = true;
	public bool CanOpen => canOpen;

	private bool isOpen;
	public bool IsOpen => isOpen;

	public event Action OnReturn;

	protected override void Awake()
	{
		base.Awake();
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}

		mainPanel = GetComponent<PanelController>();
		panelStack.Push(mainPanel);

		foreach (Button btn in gameObject.FindChilds<Button>("Button_Close", recursive: true))
		{
			btn.onClick.RemoveAllListeners();
			btn.onClick.AddListener(ESC);
		}

		foreach (Button btn in gameObject.FindChilds<Button>("Button_Prev", recursive: true))
		{
			btn.onClick.RemoveAllListeners();
			btn.onClick.AddListener(Previous);
		}

		foreach (Button btn in gameObject.FindChilds<Button>("Button_Next", recursive: true))
		{
			btn.onClick.RemoveAllListeners();
			btn.onClick.AddListener(Next);
		}

		foreach (Button btn in gameObject.FindChilds<Button>("Button_ReturnMain", recursive: true))
		{
			btn.onClick.RemoveAllListeners();
			btn.onClick.AddListener(() => PManagers.DarkUI.ShowPopupUI<UI_ReturnMainPopup>().ModalWindowIn());
		}

		foreach (Button btn in gameObject.FindChilds<Button>("Button_CloseMenu", recursive: true))
		{
			btn.onClick.RemoveAllListeners();
			btn.onClick.AddListener(CloseMenu);
		}

		for (int i = 0; i <= 3; i++)
		{
			string buttonName = $"Button_Panel{i}";
			foreach (Button btn in gameObject.FindChilds<Button>(buttonName, recursive: true))
			{
				int capturedIndex = i;
				btn.onClick.RemoveAllListeners();
				btn.onClick.AddListener(() => OpenPanel(capturedIndex));
			}
		}

		gameObject.SetActive(false);
		PManagers.Instance.input.GameUI.ESC.performed += HandleESCInput;
		PManagers.Instance.input.GameUI.PrevPage.performed += HandlePrevInput;
		PManagers.Instance.input.GameUI.NextPage.performed += HandleNextInput;
	}

	public override IEnumerator CorClosePopupUI()
	{
		throw new NotImplementedException();
	}

	private void HandlePrevInput(InputAction.CallbackContext context)
	{
		if (SceneManager.GetActiveScene().buildIndex == 0) return;
		Previous();
	}

	private void HandleNextInput(InputAction.CallbackContext context)
	{
		if (SceneManager.GetActiveScene().buildIndex == 0) return;
		Next();
	}

	private void HandleESCInput(InputAction.CallbackContext context)
	{
		if (SceneManager.GetActiveScene().buildIndex == 0 || UIManager.Instance.LogPanelOn) return;
		ESC();
	}

	private void OpenPanel(int panelIndex)
	{
		PanelController panel = panelStack.Peek().OpenPanel(panelIndex);
		if (panel != null)
			panelStack.Push(panel);

		if (panelIndex == 2)
		{
			if (horizontalSelectorView != null)
			{
				horizontalSelectorView.UpdateUI();
				horizontalSelectorView.RefreshUIAfterDelay();
			}
		}
	}

	private void Previous()
	{
		PanelController panelController = panelStack.Peek();
		if (panelController.CanSwitchPanel)
		{
			panelController.PreviousPage();
		}
	}

	private void Next()
	{
		PanelController panelController = panelStack.Peek();
		if (panelController.CanSwitchPanel)
		{
			panelController.NextPage();
		}
	}

	private void ESC()
	{
		if (NotebookManager.Instance != null && NotebookManager.Instance.IsPaused)
		{
			return;
		}

		if (UI_InteractionPopup.Instance != null && UI_InteractionPopup.Instance.IsOpen)
		{
			UI_InteractionPopup.Instance.ClosePopup();
			return;
		}

		if (SceneManager.GetActiveScene().buildIndex == 0 ||
			ScenarioManager.Instance.escActive == false) return;

		if (isOpen)
		{
			if (PManagers.DarkUI.PopupCount > 0)
			{
				PManagers.DarkUI.ClosePopupUI();
			}
			else
			{
				if (panelStack.Count > 1)
				{
					panelStack.Pop();
					panelStack.Peek().OpenPanel(0);
				}
				else if (mainPanel.CurrentPanelIndex != 0)
				{
					mainPanel.OpenPanel(0);
				}
				else
				{
					CloseMenu();
				}
			}
		}
		else if (canOpen)
		{
			OpenMenu();
		}
	}

	public void OpenMenu()
	{
		if (isOpen) return;

		if (escOpenSound != null)
			PManagers.Sound.Play(ESound.SFX, escOpenSound);

		GameManager.Instance.IsPaused = true;
		isOpen = true;
		gameObject.SetActive(true);
		GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.EscMenu, true);
		GameManager.Instance.fadeManager.SceneLoading += CloseMenu;
		AudioListener.pause = true;
		Time.timeScale = 0;
		AudioListener.pause = true;
	}

	public void CloseMenu()
	{
		if (isOpen == false) return;

		GameManager.Instance.IsPaused = false;
		isOpen = false;
		gameObject.SetActive(false);
		GameManager.Instance.playerSettingManager.SetState(PlayerAchieve.EscMenu, false);
		GameManager.Instance.fadeManager.SceneLoading -= CloseMenu;
		AudioListener.pause = false;
		Time.timeScale = 1;
		AudioListener.pause = false;
	}

	public void ReturnMainMenu()
	{
		OnReturn?.Invoke();
		OnReturn = null;
	}

	public void SwitchActiveESC(bool active) => this.canOpen = active;

	private void OnEnable()
	{
		panelStack.Clear();
		panelStack.Push(mainPanel);
	}

	private void OnDestroy()
	{
		PManagers.Instance.input.GameUI.ESC.performed -= HandleESCInput;
		PManagers.Instance.input.GameUI.PrevPage.performed -= HandlePrevInput;
		PManagers.Instance.input.GameUI.NextPage.performed -= HandleNextInput;
	}
}