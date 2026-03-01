using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class UI_MainMenuScene : UI_Scene
{
	public static bool isFirst;

	public GameObject SplashScreenPopupPrefab;
	private PanelController mainPanel;
	private readonly Stack<PanelController> panelStack = new();

	protected override void Awake()
	{
		base.Awake();

		if (isFirst == false)
		{
			PManagers.Asset.LoadAllAsync<Object>("preload", (key, count, totalCount) =>
			{
				if (count == totalCount)
				{
					PManagers.DarkUI.CacheAllPopups();
				}
			});
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

		foreach (Button btn in gameObject.FindChilds<Button>("Button_NewGame", recursive: true))
		{
			btn.onClick.RemoveAllListeners();
			btn.onClick.AddListener(() => PManagers.DarkUI.ShowPopupUI<UI_NewGamePopup>().ModalWindowIn());
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
	}

	protected override void Start()
	{
		PManagers.Instance.input.GameUISwitch(true);
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

		if (isFirst == false)
		{
			// Addressable이 로드되기 전이므로 여기서만 예외적으로 직접등록한다.
			PManagers.DarkUI.CachePopup(SplashScreenPopupPrefab);
			PManagers.DarkUI.ShowPopupUI<UI_SplashScreenPopup>().EnableSplashScreen();
			isFirst = true;
		}

		SoundManager.Instance.PlayBGM(SoundManager.Instance.BGM_Source, BGM_Sound.MainMenu, true, 1f, 0f, false);
		SoundManager.Instance.BGM_Group.audioMixer.SetFloat("Music", 1.5f);
	}

	private void OnEnable()
	{
		PManagers.Instance.input.GameUI.ESC.performed += HandleESCInput;
		PManagers.Instance.input.GameUI.PrevPage.performed += HandlePrevInput;
		PManagers.Instance.input.GameUI.NextPage.performed += HandleNextInput;
	}


	private void OnDisable()
	{
		PManagers.Instance.input.GameUI.ESC.performed -= HandleESCInput;
		PManagers.Instance.input.GameUI.PrevPage.performed -= HandlePrevInput;
		PManagers.Instance.input.GameUI.NextPage.performed -= HandleNextInput;
	}

	private void HandlePrevInput(InputAction.CallbackContext context) => Previous();
	private void HandleNextInput(InputAction.CallbackContext context) => Next();
	private void HandleESCInput(InputAction.CallbackContext context) => ESC();

	private void OpenPanel(int panelIndex)
	{
		PanelController panel = panelStack.Peek().OpenPanel(panelIndex);
		if (panel != null)
			panelStack.Push(panel);
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
		if (PManagers.DarkUI.PopupCount > 0)
		{
			PManagers.DarkUI.ClosePopupUI();
		}
		else if (IsAtMainPanel())
		{
			PManagers.DarkUI.ShowPopupUI<UI_ExitPopup>().ModalWindowIn();
		}
		else
		{
			ResetToMainPanel();
		}
	}

	#region Help

	private bool IsAtMainPanel()
	{
		return panelStack.Count == 1 && mainPanel.CurrentPanelIndex == 0;
	}

	private void ResetToMainPanel()
	{
		panelStack.Clear();
		panelStack.Push(mainPanel);
		mainPanel.OpenPanel(0);
	}

	#endregion
}