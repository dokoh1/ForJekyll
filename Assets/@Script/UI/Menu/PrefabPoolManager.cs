using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

internal class PrefabPool
{
	private readonly GameObject _prefab;
	private readonly IObjectPool<GameObject> _pool;

	private Transform _root;

	private Transform Root
	{
		get
		{
			if (_root != null) return _root;
			
			GameObject go = new() { name = $"@{_prefab.name}Pool" };
			_root = go.transform;

			return _root;
		}
	}
	
	public PrefabPool(GameObject prefab)
	{
		_prefab = prefab;
		_pool = new ObjectPool<GameObject>(OnCreate, OnGet, OnRelease, OnDestroy);
	}

	public void Push(GameObject go)
	{
		if(go.activeSelf)
			_pool.Release(go);
	}

	public GameObject Pop()
	{
		return _pool.Get();
	}

	#region Funcs

	private GameObject OnCreate()
	{
		GameObject go = Object.Instantiate(_prefab, Root, true);
		go.name = _prefab.name;
		return go;
	}

	private void OnGet(GameObject go)
	{
		go.SetActive(true);
	}

	private void OnRelease(GameObject go)
	{
		go.transform.SetParent(Root);
		go.SetActive(false);
	}

	private void OnDestroy(GameObject go)
	{
		Object.Destroy(go);
	}
	#endregion
}

public class PrefabPoolManager
{
	private readonly Dictionary<string, PrefabPool> _pools = new();

	public GameObject Pop(GameObject prefab)
	{
		if(_pools.ContainsKey(prefab.name) == false)
			CreatePool(prefab);

		return _pools[prefab.name].Pop();
	}
	
	public bool Push(GameObject go)
	{
		if (_pools.TryGetValue(go.name, out PrefabPool pool) == false)
			return false;

		pool.Push(go);
		return true;
	}

	public void Clear()
	{
		_pools.Clear();
	}

	private void CreatePool(GameObject original)
	{
		PrefabPool pool = new PrefabPool(original);
		_pools.Add(original.name, pool);
	}
}