using System;

public class SettingSliderPresenter
{
	private readonly SettingSliderView _view;
	private readonly SettingEntry _model;

	private bool _isEnabled;

	public SettingSliderPresenter(SettingSliderView view, EGameSetting modelType)
	{
		_view = view;
		_model = PManagers.Setting.GetSettingType(modelType);
	}

	/// <summary>
	/// Presenter가 활성화될 때 이벤트를 등록
	/// </summary>
	public void Enable()
	{
		if (_isEnabled) return;

		_model.OnValueUpdated += OnModelUpdated;
		OnModelUpdated(); // View 초기화
		_isEnabled = true;
	}

	/// <summary>
	/// Presenter가 비활성화될 때 이벤트를 해제
	/// </summary>
	public void Disable()
	{
		if (!_isEnabled) return;

		_model.OnValueUpdated -= OnModelUpdated;
		_isEnabled = false;
	}

	/// <summary>
	/// 모델 값을 업데이트
	/// </summary>
	public void UpdateModel(float value)
	{
		_model.SetValue(value);
	}

	/// <summary>
	/// 모델 업데이트가 발생했을 때 View를 갱신
	/// </summary>
	private void OnModelUpdated()
	{
		if (_view != null)
		{
			_view.UpdateUI(_model.GetFloat());
		}
	}
}