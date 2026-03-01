using System;
using System.Collections;
using System.Collections.Generic;
using Michsky.UI.Dark;
using UnityEngine;
using UnityEngine.UI;

public class UI_ExitPopup : UI_DarkPopup
{
	protected override void OnClickContinue()
	{
		Application.Quit();
	}

	protected override void OnClickCancel()
	{
		PManagers.DarkUI.ClosePopupUI(this);
	}
}