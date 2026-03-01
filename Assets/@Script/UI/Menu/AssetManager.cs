using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using Object = UnityEngine.Object;

public class AssetManager
{
	private readonly Dictionary<string, Object> _resources = new();
	
	public bool CheckResource<T>(string key) where T : Object
	{
		if (_resources.TryGetValue(key, out Object resource))
			return true;

		if (typeof(T) != typeof(Sprite)) 
			return false;
		
		key = key + ".sprite";
		
		return _resources.TryGetValue(key, out Object temp);
	}

	public T Load<T>(string key) where T : Object
	{
		//Debug.Log("캐시된 리소스 수: " + _resources.Count);
		//Debug.Log("Sprite 수: " + _resources.Count(x => x.Value is Sprite));

		if (_resources.TryGetValue(key, out Object resource))
			return resource as T;

		return null;
	}
	
	public GameObject Instantiate(string key, Transform parent = null, bool pooling = false)
	{
		GameObject prefab = Load<GameObject>($"{key}");
		if (prefab == null)
		{
			Debug.LogError($"Failed to load prefab : {key}");
			return null;
		}

		if (pooling)
			return PManagers.PrefabPool.Pop(prefab);

		GameObject go = Object.Instantiate(prefab, parent);
		
		go.name = prefab.name;
		return go;
	}
	
	public void Destroy(GameObject go)
	{
		if (go == null)
			return;

		if (PManagers.PrefabPool.Push(go))
			return;

		Object.Destroy(go);
	}
	
	public void Destroy(GameObject go, float delay)
	{
		PManagers.Instance.StartCoroutine(ReserveDestroy(go, delay));
	}

	private IEnumerator ReserveDestroy(GameObject go, float delay)
	{
		yield return new WaitForSeconds(delay);
		Destroy(go);
	}

	public void LoadAsync<T>(string key, Action<T> callback = null) where T : Object
	{
		//스프라이트인 경우 하위객체의 찐이름으로 로드하면 스프라이트로 로딩이 됌
		string loadKey = key;
		if (typeof(T) == typeof(Sprite))
		{
			//loadKey = $"{key}[{key}]";
		}
        
		AsyncOperationHandle<T> asyncOperation = Addressables.LoadAssetAsync<T>(loadKey);
		asyncOperation.Completed += (op) =>
		{
			// 캐시 확인.
			if (_resources.TryGetValue(key, out Object resource))
			{
				callback?.Invoke(op.Result);
				return;
			}

			_resources.Add(key, op.Result);
			callback?.Invoke(op.Result);
		};
	}
	
	public void LoadAllAsync<T>(string label, Action<string, int, int> callback) where T : Object
	{
		AsyncOperationHandle<IList<IResourceLocation>> opHandle = Addressables.LoadResourceLocationsAsync(label, typeof(T));
		opHandle.Completed += (op) =>
		{
			int loadCount = 0;

			int totalCount = op.Result.Count;

			foreach (IResourceLocation result in op.Result)
			{
				if (result.ResourceType == typeof(Texture2D) ||  result.ResourceType == typeof(Sprite))
				{
					LoadAsync<Sprite>(result.PrimaryKey, (obj) =>
					{
						loadCount++;
						callback?.Invoke(result.PrimaryKey, loadCount, totalCount);
					});
				}
				else
				{ 
					LoadAsync<T>(result.PrimaryKey, (obj) =>
					{
						loadCount++;
						callback?.Invoke(result.PrimaryKey, loadCount, totalCount);
					});
				}
			}
		};
	}
}