/*
 *
 * Use the object pools to help reduce object instantiation time and performance
 * with objects that are frequently created and used.
 *
 * http://blogs.msdn.com/b/dave_crooks_dev_blog/archive/2014/07/21/object-pooling-for-unity3d.aspx
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ObjectPoolManager : MonoSingleton<ObjectPoolManager>
{
	// look up list of various object pools.
	private Dictionary<String, ObjectPool> objectPools;

	private Transform mTransform;

	public override void Init()
	{
		if (Application.isPlaying)
			DontDestroyOnLoad(gameObject);

		mTransform = transform;
		objectPools = new Dictionary<String, ObjectPool>();
	}

	public static void CreatePool(GameObject objToPool, int initialPoolSize, int maxPoolSize, Transform parentTransform = null)
	{
		//Check to see if the pool already exists.
		if (Instance.objectPools.ContainsKey(objToPool.name))
		{
			Debug.LogWarning("ObjectPoolManager.CreatePool() pool with key " + objToPool.name + " already exist!");
		}
		else
		{
			if (parentTransform == null)
				parentTransform = Instance.mTransform;

			//create a new pool using the properties
			ObjectPool pool = new ObjectPool(objToPool, initialPoolSize, maxPoolSize, parentTransform);

			//Add the pool to the dictionary of pools to manage
			//using the object name as the key and the pool as the value.
			Instance.objectPools.Add(objToPool.name, pool);
		}
	}

	public static void RemovePool(string objName)
	{
		if (Instance.objectPools.ContainsKey(objName))
		{
			Instance.objectPools[objName].Destroy();
			Instance.objectPools.Remove(objName);
		}
	}

	public static GameObject GetObject(string objName)
	{
		if (Instance.objectPools.ContainsKey(objName))
		{
			return Instance.objectPools[objName].GetObject();
		}
		else
		{
			Debug.LogWarning("ObjectPoolManager.CreatePool() pool with key " + objName + " doesn't exist!");
			return null;
		}
	}

	public static void PutObject(GameObject obj)
	{
		if (Instance.objectPools.ContainsKey(obj.name))
		{
			Instance.objectPools[obj.name].PutObject(obj);
		}
	}

	public static void Cleanup()
	{
		if (Exists)
			Instance.objectPools.Clear();
	}
}