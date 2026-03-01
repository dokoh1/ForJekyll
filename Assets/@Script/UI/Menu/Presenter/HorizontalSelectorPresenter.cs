using System;
using Michsky.UI.Dark;
using UnityEngine;

public class HorizontalSelectorPresenter
{
	private readonly SettingEntry _model;
	private readonly HorizontalSelectorView _view;

	private bool isEnabled;

	public HorizontalSelectorPresenter(EGameSetting modelType, HorizontalSelectorView view)
	{
		_view = view;
		_model = PManagers.Setting.GetSettingType(modelType);
	}

	/// <summary>
	/// Presenter 활성화 - 모델 이벤트 등록 및 초기 UI 업데이트.
	/// </summary>
	public void Enable()
	{
		if (isEnabled) return;

		_model.OnValueUpdated += OnModelUpdated;
		OnModelUpdated();
		isEnabled = true;
	}

	/// <summary>
	/// Presenter 비활성화 - 모델 이벤트 제거.
	/// </summary>
	public void Disable()
	{
		if (!isEnabled) return;

		_model.OnValueUpdated -= OnModelUpdated;
		isEnabled = false;
	}

	/// <summary>
	/// 모델 값을 업데이트합니다.
	/// </summary>
	public void UpdatedModel(int value)
	{
		_model.SetValue(value);
	}

	/// <summary>
	/// 모델 값 변경 이벤트 발생 시 호출되어 View를 업데이트합니다.
	/// </summary>
	private void OnModelUpdated()
	{
		if (_view != null)
		{
			_view.index = _model.GetInt();
			_view.UpdateUI();
		}
	}
}