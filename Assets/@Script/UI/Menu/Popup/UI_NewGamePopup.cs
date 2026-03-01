using System;
using System.Collections;
using System.Collections.Generic;
using Michsky.UI.Dark;
using UnityEngine;

public class UI_NewGamePopup : UI_DarkPopup
{
	protected override void OnClickContinue()
	{
		DataManager.Instance.Chapter = 0;
		GameManager.Instance.fadeManager.MoveScene(SceneEnum.Tutorial);
		UIManager.Instance.view.ClearLog();
	}

	protected override void OnClickCancel()
	{
		PManagers.DarkUI.ClosePopupUI(this);
	}
}