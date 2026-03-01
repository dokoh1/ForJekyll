using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class Utils
{
	public static T GetOrAddComponent<T>(GameObject go) where T : Component
	{
		T component = go.GetComponent<T>();
		if (component == null)
			component = go.AddComponent<T>();
		return component;
	}
	
	public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
	{
		Transform transform = FindChild<Transform>(go, name, recursive);
		return transform == null ? null : transform.gameObject;
	}

	public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : Object
	{
		if (go == null)
			return null;

		if (recursive == false)
		{
			for (int i = 0; i < go.transform.childCount; i++)
			{
				Transform transform = go.transform.GetChild(i);
				if (string.IsNullOrEmpty(name) == false && transform.name != name) continue;
                
				T component = transform.GetComponent<T>();
				if (component != null)
					return component;
			}
		}
		else
		{
			foreach (T component in go.GetComponentsInChildren<T>(true))
			{
				if (string.IsNullOrEmpty(name) || component.name == name)
					return component;
			}
		}

		return null;
	}
	
	public static T[] FindChilds<T>(GameObject go, string name = null, bool recursive = false) where T : Object
	{
		if (go == null)
			return null;
		
		List<T> list = new List<T>();

		if (recursive == false)
		{
			for (int i = 0; i < go.transform.childCount; i++)
			{
				Transform transform = go.transform.GetChild(i);
				if (string.IsNullOrEmpty(name) == false && transform.name != name) continue;
                
				T component = transform.GetComponent<T>();
				if (component != null)
					list.Add(component);
			}
		}
		else
		{
			foreach (T component in go.GetComponentsInChildren<T>(true))
			{
				if (string.IsNullOrEmpty(name) || component.name == name)
					list.Add(component);
			}
		}
		
		return list.Count == 0 ? null : list.ToArray();
	}
}