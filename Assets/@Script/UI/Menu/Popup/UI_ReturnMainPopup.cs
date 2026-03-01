using System;
using System.Collections;
using System.Collections.Generic;
using Michsky.UI.Dark;
using UnityEngine;

public class UI_ReturnMainPopup : UI_DarkPopup
{
	protected override void OnClickContinue()
	{
		Time.timeScale = 1;
		UI_PauseMenuPopup.Instance.CloseMenu();
		UI_PauseMenuPopup.Instance.ReturnMainMenu();
		GameManager.Instance.fadeManager.fadeComplete += UIManager.Instance.DialogueCloseImmediate;
		GameManager.Instance.fadeManager.MoveScene(SceneEnum.MainMenu);
	}

	protected override void OnClickCancel()
	{
		PManagers.DarkUI.ClosePopupUI(this);
	}
}