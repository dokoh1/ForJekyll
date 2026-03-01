using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PanelController : MonoBehaviour
{
	private static readonly int ANIM_SPEED = Animator.StringToHash("Anim Speed");

	#region Animator State

	public const string panelOpen = "Open Panel";
	public const string panelFadeIn = "Panel In";
	public const string panelFadeOut = "Panel Out";
	public const string panelFadeOutHelper = "Panel Out Helper";
	public const string panelInstantIn = "Instant In";
	public const string buttonFadeIn = "Hover to Pressed";
	public const string buttonFadeOut = "Pressed to Normal";
	public const string buttonFadeNormal = "Pressed to Normal";

	#endregion

	public List<PanelItem> Panels = new();
	public int CurrentPanelIndex { get; private set; }
	public bool CanSwitchPanel = true;
	public bool EnableInstant = false;
	private Coroutine prevCoroutine;

	[SerializeField] private float animationSpeed = 1.0f;
	[SerializeField] private float animationSmoothness = 0.25f;

	private void OnEnable()
	{
		foreach (PanelItem panelItem in Panels)
		{
			panelItem.panelObject.SetActive(false);
		}

		CurrentPanelIndex = 0;
		PanelItem currentPanelItem = Panels[CurrentPanelIndex];
		currentPanelItem.panelObject.SetActive(true);
		currentPanelItem.PanelAnimator.Play(EnableInstant ? panelInstantIn : panelFadeIn);
		currentPanelItem.ButtonAnimator?.Play(buttonFadeIn);
	}

	public PanelController OpenPanel(int nextPanelIndex)
	{
		if (prevCoroutine != null)
			StopCoroutine(prevCoroutine);

		PanelItem currentPanel = Panels[CurrentPanelIndex];
		PanelItem nextPanel = Panels[nextPanelIndex];

		GameObject currentPanelObj = currentPanel.panelObject;
		GameObject nextPanelObj = nextPanel.panelObject;

		Animator currentAnimator = currentPanel.PanelAnimator;
		Animator nextAnimator = nextPanel.PanelAnimator;

		currentAnimator.SetFloat(ANIM_SPEED, animationSpeed);
		currentAnimator.CrossFade(panelFadeOut, animationSmoothness);

		nextPanelObj.SetActive(true);
		nextAnimator.SetFloat(ANIM_SPEED, animationSpeed);
		nextAnimator.CrossFade(panelFadeIn, animationSmoothness);

		prevCoroutine = StartCoroutine(DisablePanelDelayed(currentPanelObj));
		if (currentPanel.panelButton != null)
		{
			GameObject currentButton = currentPanel.panelButton;
			GameObject nextButton = nextPanel.panelButton;

			Animator currentButtonAnimator = currentButton.GetComponent<Animator>();
			Animator nextButtonAnimator = nextButton.GetComponent<Animator>();

			currentButtonAnimator.Play(buttonFadeOut);
			nextButtonAnimator.Play(buttonFadeIn);
		}

		CurrentPanelIndex = nextPanelIndex;

		return nextPanel.SubController;

		IEnumerator DisablePanelDelayed(GameObject panel)
		{
			yield return new WaitForSeconds(1f);
			panel.SetActive(false);
			prevCoroutine = null;
		}
	}

	public void NextPage()
	{
		if (CurrentPanelIndex < Panels.Count - 1)
		{
			OpenPanel(CurrentPanelIndex + 1);
		}
	}

	public void PreviousPage()
	{
		if (CurrentPanelIndex > 0)
		{
			OpenPanel(CurrentPanelIndex - 1);
		}
	}
}


[Serializable]
public class PanelItem
{
	public GameObject panelObject;
	public GameObject panelButton;
	public GameObject defaultSelected;

	public PanelController SubController
	{
		get
		{
			if (subController == null)
				subController = panelObject.GetComponent<PanelController>();
			return subController;
		}
	}

	public Animator PanelAnimator
	{
		get
		{
			if (panelAnimator == null)
				panelAnimator = panelObject.GetComponent<Animator>();
			return panelAnimator;
		}
	}

	public Animator ButtonAnimator
	{
		get
		{
			if (buttonAnimator == null && panelButton != null)
				buttonAnimator = panelButton.GetComponent<Animator>();
			return buttonAnimator;
		}
	}

	private Animator panelAnimator;
	private Animator buttonAnimator;
	private PanelController subController;
}