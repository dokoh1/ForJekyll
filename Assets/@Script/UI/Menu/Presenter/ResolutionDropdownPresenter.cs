using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionDropdownPresenter
{
	private readonly SettingEntry _model;
	private readonly ResolutionDropdownView _view;

	public ResolutionDropdownPresenter(ResolutionDropdownView view, EGameSetting modelType)
	{
		_view = view;
		_model = PManagers.Setting.GetSettingType(modelType);
		_model.OnValueUpdated += OnModelUpdated;
		
		OnModelUpdated();
	}

	public void UpdateModel(int value)
	{
		_model.SetValue(value);
	}

	private void OnModelUpdated()
	{
		_view.UpdateUI(_model.GetInt());
	}

	public void Release()
	{
		_model.OnValueUpdated -= OnModelUpdated;
	}
}