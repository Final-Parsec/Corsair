using System.Collections.Generic;
using UnityEngine;

public static class ObjectPooling
{
    private static readonly Dictionary<string, LinkedList<GameObject>> PooledObjects = new Dictionary<string, LinkedList<GameObject>>();

    public static T GetObjectFromPool<T>(this GameObject prefab, string objectPoolName)
    {
        var objectPool = GetObjectPool(objectPoolName);

        if (objectPool.Count > 0)
        {
            var objectFromPool = objectPool.First.Value;
            objectPool.RemoveFirst();

            // Initialize the object.
            objectFromPool.SetActive(true);

            return objectFromPool.GetComponent<T>();    
        }

        var newGameObject = Object.Instantiate(prefab);
        return newGameObject.GetComponent<T>();
    }

    public static T GetObjectFromPool<T>(
        this GameObject prefab,
        string objectPoolName,
        Vector3 position,
        Quaternion rotation)
    {
		return prefab.GetObjectFromPool(objectPoolName, position, rotation).GetComponent<T>();
    }

	public static GameObject GetObjectFromPool(
		this GameObject prefab,
		string objectPoolName,
		Vector3 position,
		Quaternion rotation)
	{
		var objectPool = GetObjectPool(objectPoolName);
		
		if (objectPool.Count > 0)
		{
			var objectFromPool = objectPool.First.Value;
			objectPool.RemoveFirst();
			
			// Initialize the object.
			objectFromPool.transform.position = position;
			objectFromPool.transform.rotation = rotation;
			objectFromPool.SetActive(true);
			
			return objectFromPool;
		}
		
		var newGameObject = Object.Instantiate(prefab, position, rotation) as GameObject;
		return newGameObject;
	}

    public static void ReturnToPool(this GameObject gameobject, string objectPoolName)
    {
        var objectPool = GetObjectPool(objectPoolName);
        gameobject.SetActive(false);
        objectPool.AddFirst(gameobject);
    }

    public static void WarmUpPool(
        this GameObject prefab,
        string objectPoolName,
        int initialSize)
    {
        var objectPool = GetObjectPool(objectPoolName);

        while (objectPool.Count < initialSize)
        {
            var newGameObject = Object.Instantiate(prefab);
            newGameObject.SetActive(false);
            objectPool.AddFirst(newGameObject);
        }
    }

    /// <summary>
    ///     Retrieves an object pool or creates one if it does not exist.
    /// </summary>
    /// <param name="objectPoolName">
    ///     The unique name of the object pool.
    /// </param>
    /// <returns>
    ///     An object pool for the name specified.
    /// </returns>
    private static LinkedList<GameObject> GetObjectPool(string objectPoolName)
    {
        LinkedList<GameObject> objectPool;
        if (!ObjectPooling.PooledObjects.TryGetValue(objectPoolName, out objectPool))
        {
            var newPool = new LinkedList<GameObject>();
            ObjectPooling.PooledObjects.Add(objectPoolName, newPool);
            objectPool = newPool;

			Debug.Log(objectPoolName);
        }

        return objectPool;
    }
}