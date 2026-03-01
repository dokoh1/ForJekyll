using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResolutionDropdownView : MonoBehaviour
{
	private TMP_Dropdown dropdown;
	private Resolution[] resolutions;
	private ResolutionDropdownPresenter presenter;

	private void Start()
	{
		resolutions = Screen.resolutions;
		dropdown = GetComponent<TMP_Dropdown>();
		dropdown.options.Clear();
		dropdown.onValueChanged.AddListener(OnDropdownValueChanged);

		foreach (Resolution resolution in resolutions)
		{
			int hz = Mathf.RoundToInt((float)resolution.refreshRateRatio.value);
			TMP_Dropdown.OptionData option =
				new($"{resolution.width}x{resolution.height} {hz}Hz");
			dropdown.options.Add(option);
		}

		presenter = new ResolutionDropdownPresenter(this, EGameSetting.Resolution);
	}

	public void UpdateUI(int index)
	{
		dropdown.SetValueWithoutNotify(index);
		dropdown.RefreshShownValue();
	}

	private void OnDropdownValueChanged(int index)
	{
		presenter.UpdateModel(index);
	}

	private void OnDestroy()
	{
		presenter.Release();
	}
}