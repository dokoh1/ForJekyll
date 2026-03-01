using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class GameSettingManager
{
	private readonly Dictionary<EGameSetting, SettingEntry> settings = new();

	public void LoadSettings(SettingTableSO tableSO)
	{
		foreach (SettingEntrySO so in tableSO.Entries)
		{
			object defaultValue = so.ValueType switch
			{
				ESettingValueType.Int => so.DefaultInt,
				ESettingValueType.Float => so.DefaultFloat,
				ESettingValueType.String => so.DefaultString,
				_ => null
			};
			if (so.Type == EGameSetting.Language) defaultValue = ChangeLanguageToIndex(DataManager.Instance.GetSystemLocale);
			settings.Add(so.Type, new SettingEntry(so.Type, so.ValueType, defaultValue));
		}
	}

	public void Init()
	{
		RegisterAction(EGameSetting.MasterVolume, v => PManagers.Instance.Mixer.SetFloat("Master", ConvertToDecibel((float)v)));
		RegisterAction(EGameSetting.MusicVolume, v => PManagers.Instance.Mixer.SetFloat("Music", ConvertToDecibel((float)v)));
		RegisterAction(EGameSetting.SFXVolume, v => PManagers.Instance.Mixer.SetFloat("SFX", ConvertToDecibel((float)v)));
		RegisterAction(EGameSetting.VoiceVolume, v => PManagers.Instance.Mixer.SetFloat("Voice", ConvertToDecibel((float)v)));
		RegisterAction(EGameSetting.LookSens, v => GameManager.Instance?.Player?.PlayerLookController?.ChangeSensitivity((float)v));
		RegisterAction(EGameSetting.Brightness, v => RenderSettings.ambientIntensity = (float)v);
		RegisterAction(EGameSetting.SubSize, v => DataManager.Instance.ChangeFontSize((float)v));
		RegisterAction(EGameSetting.SubSpeed, v => DataManager.Instance.ChangePlaySpeed((float)v));
		RegisterAction(EGameSetting.ScreenMode, v => SetScreenMode((int)v));
		RegisterAction(EGameSetting.Resolution, v => SetResolution((int)v));
		RegisterAction(EGameSetting.Language, v => SetLanguage((int)v));

		SceneManager.sceneLoaded += OnSceneLoaded;

		RefreshAllLayouts();
	}

	#region Event

	// 언어 변경시 글자 수 변화로 인한 레이아웃들의 크기들이 이상해지는 버그 개선을 위한 코드
	private void RefreshAllLayouts(Locale locale = null)
	{
		PManagers.Instance.StartCoroutine(RefreshAllLayoutsCoroutine());
		return;

		IEnumerator RefreshAllLayoutsCoroutine()
		{
			for (int i = 0; i < 60; i++)
			{
				foreach (RectTransform rect in GetAllRectTransformsInScene())
				{
					if (rect.GetComponent<HorizontalLayoutGroup>() != null ||
					    rect.GetComponent<VerticalLayoutGroup>() != null ||
					    rect.GetComponent<GridLayoutGroup>() != null ||
					    rect.GetComponent<ContentSizeFitter>() != null)
					{
						LayoutRebuilder.MarkLayoutForRebuild(rect);
					}
				}

				yield return null;
			}
		}
	}


	/// <summary>
	/// 현재 씬의 모든 RectTransform을 검색 (비활성화된 오브젝트도 포함)
	/// </summary>
	private RectTransform[] GetAllRectTransformsInScene()
	{
		List<RectTransform> results = new();
		Scene scene = SceneManager.GetActiveScene();

		// 씬에 있는 모든 루트 오브젝트 가져오기
		GameObject[] rootObjects = scene.GetRootGameObjects();

		foreach (GameObject rootObject in rootObjects)
		{
			// 재귀적으로 모든 RectTransform을 검색
			results.AddRange(rootObject.GetComponentsInChildren<RectTransform>(true));
		}

		return results.ToArray();
	}


	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		GetSettingType(EGameSetting.Brightness).Apply();
		GetSettingType(EGameSetting.LookSens).Apply();
		GetSettingType(EGameSetting.SubSize).Apply();
		GetSettingType(EGameSetting.SubSpeed).Apply();
	}

	#endregion

	#region Screen

	public void SetResolution(int index)
	{
		if (index == -1)
		{
			GetSettingType(EGameSetting.Resolution).SetValue(Screen.resolutions.Length - 1);
		}
		else
		{
			Resolution resolution = Screen.resolutions[index];
			Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
			Application.targetFrameRate = Mathf.RoundToInt((float)resolution.refreshRateRatio.value);
		}
	}

	public void SetScreenMode(int index)
	{
		switch (index)
		{
			case 0:
				Screen.fullScreen = false;
				Screen.fullScreenMode = FullScreenMode.Windowed;
				break;
			case 1:
				Screen.fullScreen = true;
				Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
				break;
			case 2:
				Screen.fullScreen = true;
				Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
				break;
		}
	}

	#endregion

	#region Language

	private void SetLanguage(int index)
	{
		DataManager.Instance.LocaleSetting = ChangeIndexToLanguage(index);
		RefreshAllLayouts();
	}

	#endregion

	#region Helper

	public int ChangeLanguageToIndex(string str)
	{
		switch (str)
		{
			case "ko": return 0;
			case "en": return 1;
			case "ja": return 2;
			case "zh-Hans": return 3;
			case "zh-Hant": return 4;
			case "ru": return 5;
			default: return 1;
		}
	}

	public string ChangeIndexToLanguage(int index)
	{
		switch (index)
		{
			case 0: return "ko";
			case 1: return "en";
			case 2: return "ja";
			case 3: return "zh-Hans";
			case 4: return "zh-Hant";
			case 5: return "ru";
			default: return "en";
		}
	}

	public SettingEntry GetSettingType(EGameSetting eGameSetting) => settings[eGameSetting];
	private float ConvertToDecibel(float volume) => volume > 0 ? Mathf.Log10(volume / 100) * 20 : -80f;

	private void RegisterAction(EGameSetting type, Action<object> action)
	{
		settings[type].ApplyAction += action;
		settings[type].Apply();
	}

	#endregion
}

public class SettingEntry
{
	public EGameSetting Type;
	public ESettingValueType ValueType;
	public object Value;
	public object DefaultValue;

	public event Action OnValueUpdated;
	public event Action<object> ApplyAction;

	public SettingEntry(EGameSetting type, ESettingValueType valueType, object defaultValue)
	{
		Type = type;
		ValueType = valueType;
		DefaultValue = defaultValue;
		Initialize();
	}

	public void Initialize()
	{
		switch (ValueType)
		{
			case ESettingValueType.Int:
				Value = PlayerPrefs.GetInt(GetKey(), DefaultValue != null ? (int)DefaultValue : 0);
				break;
			case ESettingValueType.Float:
				Value = PlayerPrefs.GetFloat(GetKey(), DefaultValue != null ? (float)DefaultValue : 0f);
				break;
			case ESettingValueType.String:
				Value = PlayerPrefs.GetString(GetKey(), DefaultValue != null ? (string)DefaultValue : "");
				break;
		}
	}

	public void SetValue(object value)
	{
		Value = value;
		OnValueUpdated?.Invoke();
		Apply();
		switch (ValueType)
		{
			case ESettingValueType.Int:
				PlayerPrefs.SetInt(GetKey(), (int)value);
				break;
			case ESettingValueType.Float:
				PlayerPrefs.SetFloat(GetKey(), (float)value);
				break;
			case ESettingValueType.String:
				PlayerPrefs.SetString(GetKey(), (string)value);
				break;
		}
	}

	public void Apply() => ApplyAction?.Invoke(Value);

	public int GetInt() => ValueType == ESettingValueType.Int ? (int)Value : 0;
	public float GetFloat() => ValueType == ESettingValueType.Float ? (float)Value : 0;
	public string GetString() => ValueType == ESettingValueType.String ? (string)Value : null;
	private string GetKey() => Type.ToString();
}

public enum EGameSetting
{
	// float
	MasterVolume,
	MusicVolume,
	SFXVolume,
	VoiceVolume,
	SubSize,
	SubSpeed,
	LookSens,
	Brightness,

	// int
	ScreenMode,
	Resolution,

	// string
	Language,
}

public enum ESettingValueType
{
	Int,
	Float,
	String,
}