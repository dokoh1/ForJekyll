using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SaveSlotSubItem : UI_SubItem
{
	private enum Texts
	{
		Description,
	}

	private enum Images
	{
		Image,
	}

	[SerializeField] private int slotNum;
	private Button button;

	protected override void Awake()
	{
		base.Awake();
		BindImages(typeof(Images));
		BindTexts(typeof(Texts));
		button = GetComponent<Button>();
		button.onClick.AddListener(OnClick);
	}

	private void OnClick()
	{
		PManagers.DarkUI.ShowPopupUI<UI_LoadSavePopup>().ModalWindowIn();
		PushData();
	}

	private void OnEnable()
	{
		DataToSave data = DataManager.Instance.sceneDataManager.DataLoad(slotNum);
		if (data != null)
		{
			button.interactable = true;
			GetText((int)Texts.Description).text = data.saveTxt;
			GetImage((int)Images.Image).sprite =
				DataManager.Instance.sceneDataManager.sceneImages[(SceneEnum)data.SceneEnum];
		}
		else
		{
			button.interactable = false;
			GetText((int)Texts.Description).text = "";
		}
	}

	private void PushData()
	{
		DataManager.Instance.sceneDataManager.nowTextMeshPro = GetText((int)Texts.Description) as TextMeshProUGUI;
		DataManager.Instance.sceneDataManager.nowSaveDataNum = slotNum;
		DataManager.Instance.sceneDataManager.nowLoadDataNum = slotNum;
	}
}