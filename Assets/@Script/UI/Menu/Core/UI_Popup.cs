using System.Collections;
using UnityEngine;

public abstract class UI_Popup : UI_Base
{
	public Canvas UICanvas { get; set; }

	protected override void Awake()
	{
		base.Awake();

		UICanvas = PManagers.DarkUI.SetCanvas(gameObject);
		gameObject.SetActive(false);
	}

	public void ClosePopupUI() => StartCoroutine(nameof(CorClosePopupUI));
	public abstract IEnumerator CorClosePopupUI();
	public void StopClosePopupUI() => StopCoroutine(nameof(CorClosePopupUI));
}