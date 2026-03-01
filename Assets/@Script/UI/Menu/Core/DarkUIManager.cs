using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class DarkUIManager
{
	private UI_Scene sceneUI;

	public int PopupCount => _popupList.Count;

	private int _popupOrder = 100;

	private readonly List<UI_Popup> _popupList = new();
	private readonly Dictionary<string, GameObject> _popups = new();

	public GameObject Root
	{
		get
		{
			GameObject root = GameObject.Find("@UI_Root");
			if (root == null)
			{
				root = new GameObject { name = "@UI_Root" };
				Object.DontDestroyOnLoad(root);
			}

			return root;
		}
	}

	public Canvas SetCanvas(GameObject go, bool sort = true, int sortOrder = 0)
	{
		Canvas canvas = Utils.GetOrAddComponent<Canvas>(go);

		if (canvas != null)
		{
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.overrideSorting = true;
		}

		CanvasScaler cs = Utils.GetOrAddComponent<CanvasScaler>(go);
		if (cs != null)
		{
			cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			//cs.referenceResolution = new Vector2(REFERENCE_RESOLUTION_WIDTH, REFERENCE_RESOLUTION_HEIGHT);
		}

		Utils.GetOrAddComponent<GraphicRaycaster>(go);

		if (sort)
		{
			canvas.sortingOrder = _popupOrder;
			_popupOrder++;
		}

		return canvas;
	}

	public T GetSceneUI<T>() where T : UI_Base
	{
		return sceneUI as T;
	}

	public T ShowPopupUI<T>() where T : UI_Popup
	{
		T popup = _popups[typeof(T).Name].GetComponent<T>();
		_popupList.Add(popup);

		popup.StopClosePopupUI();
		popup.transform.SetParent(Root.transform);
		popup.gameObject.SetActive(true);
		_popupOrder++;
		popup.UICanvas.sortingOrder = _popupOrder;

		return popup;
	}

	public void ClosePopupUI(UI_Popup popup)
	{
		_popupList.Remove(popup);
		popup.ClosePopupUI();

		// 팝업이 최상단에 있는 경우에만 Order를 감소
		if (_popupList.Count != 0 && _popupList.Last() == popup)
		{
			_popupOrder--;
		}
	}

	public void ClosePopupUI()
	{
		if (_popupList.Count == 0)
			return;

		UI_Popup popup = _popupList.Last();
		ClosePopupUI(popup);
	}

	public void CloseAllPopupUI()
	{
		while (_popupList.Count > 0)
			ClosePopupUI();
	}

	public void CachePopup(GameObject popupPrefab)
	{
		GameObject instance = Object.Instantiate(popupPrefab, Root.transform);
		instance.name = popupPrefab.name;
		instance.SetActive(false);
		_popups.Add(instance.name, instance);
		Debug.Log($"캐싱 완료: {popupPrefab.name}");
	}

	public void CacheAllPopups()
	{
		foreach (string className in GetAllPopupClassNames())
		{
			if (_popups.ContainsKey(className))
				continue; // 이미 캐싱된 경우 건너뜀

			GameObject popupPrefab = PManagers.Asset.Load<GameObject>(className);
			if (popupPrefab != null)
			{
				CachePopup(popupPrefab);
			}
			else
			{
				Debug.LogWarning($"AssetManager에서 {className}를 불러오지 못했습니다.");
			}
		}
	}

	private List<string> GetAllPopupClassNames()
	{
		// 현재 애셈블리에서 'UI_Popup'을 상속받는 모든 타입 탐색
		IEnumerable<Type> popupTypes = Assembly.GetAssembly(typeof(UI_Popup))
		                                       .GetTypes()
		                                       .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(UI_Popup)));

		// 클래스명(key 값) 리스트로 반환
		return popupTypes.Select(type => type.Name).ToList();
	}


	public void Clear()
	{
		CloseAllPopupUI();
		sceneUI = null;
	}
}