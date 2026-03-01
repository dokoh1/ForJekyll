using UnityEngine;

public static class Extension
{
	public static T FindChild<T>(this GameObject go, string name = null, bool recursive = false) where T : Component
	{
		return Utils.FindChild<T>(go, name, recursive);
	}

	public static T[] FindChilds<T>(this GameObject go, string name = null, bool recursive = false) where T : Component
	{
		return Utils.FindChilds<T>(go, name, recursive);
	}
}