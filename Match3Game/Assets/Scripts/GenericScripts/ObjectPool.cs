using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// The object pool is a list of already instantiated game objects of the same type.
/// </summary>
public class ObjectPool
{
	//the list of free objects.
	private Stack<GameObject> freeObjects;

	//the list of spawned objects.
	private List<GameObject> spawnedObjects;

	//sample of the actual object to store.
	//used if we need to grow the list.
	private GameObject prefabObj;

	//maximum number of objects to have in the list.
	private int maxPoolSize;

	//initial and default number of objects to have in the list.
	private int initialPoolSize;

	private Transform parentTransform;

	/// <summary>
	/// Constructor for creating a new Object Pool.
	/// </summary>
	/// <param name="obj">Game Object for this pool</param>
	/// <param name="initialPoolSize">Initial and default size of the pool.</param>
	/// <param name="maxPoolSize">Maximum number of objects this pool can contain.</param>
	public ObjectPool(GameObject obj, int initialPoolSize, int maxPoolSize, Transform ParentTransform)
	{
		parentTransform = ParentTransform;

		freeObjects = new Stack<GameObject>();
		spawnedObjects = new List<GameObject>();

		//create and add an object based on initial size.
		for (int i = 0; i < initialPoolSize; i++)
		{
			//instantiate and create a game object with useless attributes.
			//these should be reset anyways.
			GameObject gameObject = GameObject.Instantiate(obj, Vector3.zero, Quaternion.identity) as GameObject;

			gameObject.name = obj.name;

			if (parentTransform)
			{
				gameObject.transform.SetParent(parentTransform);
			}

			//make sure the object isn't active.
			gameObject.SetActive(false);

			//add the object too our list.
			freeObjects.Push(gameObject);
		}

		//store our other variables that are useful.
		this.maxPoolSize = maxPoolSize;
		this.prefabObj = obj;
		this.initialPoolSize = initialPoolSize;
	}

	/// <summary>
	/// Returns an active object from the object pool without resetting any of its values.
	/// You will need to set its values and set it inactive again when you are done with it.
	/// </summary>
	/// <returns>Game Object of requested type if it is available, otherwise null.</returns>
	public GameObject GetObject()
	{
		if (freeObjects.Count > 0)
		{
			var gameObject = freeObjects.Pop();
			spawnedObjects.Add(gameObject);
			gameObject.SetActive(true);
			return gameObject;
		}

		//if we make it this far, we obviously didn't find an inactive object.
		//so we need to see if we can grow beyond our current count.
		if (this.maxPoolSize > this.freeObjects.Count)
		{
			//Instantiate a new object.
			GameObject gameObject = GameObject.Instantiate(prefabObj, Vector3.zero, Quaternion.identity) as GameObject;

			gameObject.name = prefabObj.name;

			if (parentTransform)
				gameObject.transform.SetParent(parentTransform);

			//add it to the pool of objects
			spawnedObjects.Add(gameObject);

			//set it to active since we are about to use it.
			gameObject.SetActive(true);

			//return the object to the requestor.
			return gameObject;
		}

		Debug.LogWarning("ObjectPool.GetObject() pool totaly full! All objects in use!");
		//if we made it this far obviously we didn't have any inactive objects
		//we also were unable to grow, so return null as we can't return an object.
		return null;
	}

	public void PutObject(GameObject gameObject)
	{
		if (spawnedObjects.Contains(gameObject))
		{
			if (parentTransform)
				gameObject.transform.SetParent(parentTransform);

			gameObject.SetActive(false);
			freeObjects.Push(gameObject);
			spawnedObjects.Remove(gameObject);
		}
	}

	public void Destroy()
	{
		while (freeObjects.Count > 0)
		{
			GameObject.Destroy(freeObjects.Pop());
		}
		for (int i = spawnedObjects.Count - 1; i >= 0; i--)
		{
			GameObject.Destroy(spawnedObjects[i]);
		}
		spawnedObjects.Clear();
	}

	/// <summary>
	/// Iterate through the pool and releases as many objects as
	/// possible until the pool size is back to the initial default size.
	/// </summary>
	public void Shrink()
	{
		//how many objects are we trying to remove here?
		int objectsToRemoveCount = freeObjects.Count + spawnedObjects.Count - initialPoolSize;
		//Are there any objects we need to remove?
		if (objectsToRemoveCount <= 0)
		{
			//cool lets get out of here.
			return;
		}

		//iterate through our list and remove some objects
		//we do reverse iteration so as we remove objects from
		//the list the shifting of objects does not affect our index
		//Also notice the offset of 1 to account for zero indexing
		//and i >= 0 to ensure we reach the first object in the list.
		for (int i = Mathf.Min(objectsToRemoveCount, freeObjects.Count - 1); i >= 0; i--)
		{
			var go = freeObjects.Pop();
			GameObject.Destroy(go);
		}
	}

}