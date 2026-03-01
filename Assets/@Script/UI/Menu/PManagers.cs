using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class PManagers : MonoBehaviour
{
	public static PManagers Instance;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(this);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	private void Start()
	{
		Init();
		SceneManager.activeSceneChanged += Clear;
	}

	public InputController input;
	public AudioMixer Mixer;
	public SettingTableSO SettingTableSO;

	public static DarkUIManager DarkUI = new();
	public static PSoundManager Sound = new();
	public static GameSettingManager Setting = new();
	public static AssetManager Asset = new();
	public static PrefabPoolManager PrefabPool = new();

	public void Init()
	{
		if (Mixer == null)
			Debug.LogError("Mixer is null");

		Sound.Init(Mixer);
		Setting.LoadSettings(SettingTableSO);
		Setting.Init();
	}

	private void Clear(Scene scene, Scene mode)
	{
		DarkUI.Clear();
	}
}