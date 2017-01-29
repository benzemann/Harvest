using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool {

    #region private
    List<GameObject> pooledObjects;
    GameObject _prefab;
    int _poolAmount;
    bool _willGrow;
    #endregion

    #region constructers
    public ObjectPool(GameObject prefab, int poolAmount, bool willGrow)
    {
        _poolAmount = poolAmount;
        _willGrow = willGrow;
        _prefab = prefab;

        pooledObjects = new List<GameObject>();

        for(int i = 0; i < _poolAmount; i++)
        {
            GameObject go = Object.Instantiate(_prefab) as GameObject;
            go.SetActive(false);
            pooledObjects.Add(go);
        }
    }
    #endregion

    #region methods
    /// <summary>
    /// Get the next pooled object
    /// </summary>
    /// <returns>The pooled object, is null if no available</returns>
    public GameObject GetPooledObject()
    {
        for(int i = 0; i < _poolAmount; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }

        if (_willGrow)
        {
            GameObject go = Object.Instantiate(_prefab) as GameObject;
            pooledObjects.Add(go);
            _poolAmount = pooledObjects.Count;
            return go;
        }

        return null;
    }
    #endregion

}
