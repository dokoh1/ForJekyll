using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SavePopup : UI_DarkPopup
{
	public Action OnContinueAction;
	protected override void OnClickCancel()
	{
		PManagers.DarkUI.ClosePopupUI(this);
	}

	protected override void OnClickContinue()
	{
		DataManager.Instance.sceneDataManager.DataSave();
		OnContinueAction?.Invoke();
		PManagers.DarkUI.ClosePopupUI(this);
	}
}