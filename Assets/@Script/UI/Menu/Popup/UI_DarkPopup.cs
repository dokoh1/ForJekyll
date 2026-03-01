using System;
using System.Collections;
using System.Collections.Generic;
using Michsky.UI.Dark;
using UnityEngine;

public class UI_DarkPopup : UI_Popup
{
	private enum Buttons
	{
		Continue,
		Cancel,
	}

	private BlurManager blurManager;
	private UIDissolveEffect dissolveEffect;
	private Animator animator;
	private bool isIn = false;

	protected override void Awake()
	{
		base.Awake();
		BindButtons(typeof(Buttons));
		GetButton((int)Buttons.Continue).onClick.AddListener(OnClickContinue);
		GetButton((int)Buttons.Cancel).onClick.AddListener(OnClickCancel);
		blurManager = gameObject.FindChild<BlurManager>("Blur", recursive: true);
		animator = gameObject.FindChild<Animator>("ModalWindow", recursive: true);
		dissolveEffect = gameObject.FindChild<UIDissolveEffect>("Content", recursive: true);
	}

	public void ModalWindowIn()
	{
		if (isIn) return;

		blurManager.BlurInAnim();
		animator.gameObject.SetActive(true);
		animator.Play("In");

		dissolveEffect.location = 1;
		dissolveEffect.DissolveIn();

		isIn = true;
	}

	protected virtual void OnClickContinue()
	{
	}

	protected virtual void OnClickCancel()
	{
	}

	public override IEnumerator CorClosePopupUI()
	{
		animator.Play("Out");
		dissolveEffect.DissolveOut();
		blurManager.BlurOutAnim();
		isIn = false;
		yield return new WaitForSeconds(1.5f);
		gameObject.SetActive(false);
	}
}