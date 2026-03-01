using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using static Define;

public abstract class UI_Base : MonoBehaviour
{
	protected Dictionary<Type, Object[]> Objects = new();

	protected virtual void Awake()
	{
	}
	
	protected virtual void Start()
	{
	}
	
	protected void BindObjects(Type type) { Bind<GameObject>(type); }
	protected void BindImages(Type type) { Bind<Image>(type); }
	protected void BindTexts(Type type) { Bind<TMP_Text>(type); }
	protected void BindButtons(Type type) { Bind<Button>(type); }
	protected void BindToggles(Type type) { Bind<Toggle>(type); }
	protected void BindSliders(Type type) { Bind<Slider>(type); }
	
	protected void Bind<T>(Type type) where T : Object
	{
		string[] names = Enum.GetNames(type);
		Object[] objects = new Object[names.Length];
		Objects.Add(typeof(T), objects);

		for (int i = 0; i < names.Length; i++)
		{
			if (typeof(T) == typeof(GameObject))
				objects[i] = Utils.FindChild(gameObject, names[i], true);
			else
				objects[i] = Utils.FindChild<T>(gameObject, names[i], true);

			if (objects[i] == null)
				Debug.Log($"Failed to bind({names[i]})");
		}
	}
	
	protected GameObject GetObject(int idx) { return Get<GameObject>(idx); }
	protected TMP_Text GetText(int idx) { return Get<TMP_Text>(idx); }
	protected Button GetButton(int idx) { return Get<Button>(idx); }
	protected Image GetImage(int idx) { return Get<Image>(idx); }
	protected Toggle GetToggle(int idx) { return Get<Toggle>(idx); }
	protected Slider GetSlider(int idx)
	{
		Slider ret = Get<Slider>(idx);
		ret.interactable = false;
		return ret;
	}
	
	protected T Get<T>(int idx) where T : Object
	{
		if (Objects.TryGetValue(typeof(T), out Object[] objects) == false)
			return null;

		return objects[idx] as T;
	}
	
	public static void BindEvent(GameObject go, Action<PointerEventData> action = null, Define.ETouchEvent type = Define.ETouchEvent.Click)
	{
		UI_EventHandler evt = Utils.GetOrAddComponent<UI_EventHandler>(go);

		switch (type)
		{
			case ETouchEvent.Click:
				evt.OnClickHandler -= action;
				evt.OnClickHandler += action;
				break;
			case ETouchEvent.PointerDown:
				evt.OnPointerDownHandler -= action;
				evt.OnPointerDownHandler += action;
				break;
			case ETouchEvent.PointerUp:
				evt.OnPointerUpHandler -= action;
				evt.OnPointerUpHandler += action;
				break;
			case ETouchEvent.Drag:
				evt.OnDragHandler -= action;
				evt.OnDragHandler += action;
				break;
			case ETouchEvent.BeginDrag:
				evt.OnBeginDragHandler -= action;
				evt.OnBeginDragHandler += action;
				break;
			case ETouchEvent.EndDrag:
				evt.OnEndDragHandler -= action;
				evt.OnEndDragHandler += action;
				break;
			case ETouchEvent.LongPressed:
				evt.OnLongPressHandler -= action;
				evt.OnLongPressHandler += action;
				break;
			case ETouchEvent.Enter:
				evt.OnEnterHandler -= action;
				evt.OnEnterHandler += action;
				break;
		}
	}
}