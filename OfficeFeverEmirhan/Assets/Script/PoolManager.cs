using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PoolType
{
    Paper,

}

[System.Serializable]
public class Pool
{
    public PoolType poolType;
    public GameObject prefab;
    public int poolSize;
    public List<GameObject> poolObjects;
}

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance;
    public List<Pool> pools;

    void Awake()
    {
        instance = this;
        InitializePools();
    }

    private void InitializePools()
    {
        foreach (Pool pool in pools)
        {
            pool.poolObjects = new List<GameObject>();

            for (int i = 0; i < pool.poolSize; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                pool.poolObjects.Add(obj);
            }
        }
    }

    public GameObject GetPooledObject(PoolType poolType, Vector3 position)
    {
        Pool pool = pools.Find(p => p.poolType == poolType);

        if (pool != null)
        {
            GameObject obj;
            if (pool.poolObjects.Count > 0)
            {
                obj = pool.poolObjects[0];
                pool.poolObjects.RemoveAt(0);
            }
            else
            {
                obj = Instantiate(pool.prefab);
            }

            obj.transform.position = position;
            obj.SetActive(true);
            return obj;
        }
        return null;
    }

    public void DeactivatePooledObject(GameObject obj, PoolType poolType)
    {
        Pool pool = pools.Find(p => p.poolType == poolType);

        obj.SetActive(false);
        pool.poolObjects.Add(obj);
    }
}
