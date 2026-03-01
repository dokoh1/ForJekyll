using System;
using System.Collections;
using System.Collections.Generic;
using Michsky.UI.Dark;
using UnityEngine;

public class UI_LoadSavePopup : UI_DarkPopup
{
	protected override void OnClickContinue()
	{
		Time.timeScale = 1;
		DataManager.Instance.sceneDataManager.DataLoad();
	}

	protected override void OnClickCancel()
	{
		PManagers.DarkUI.ClosePopupUI(this);
	}
}