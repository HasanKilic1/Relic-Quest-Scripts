using System.Collections.Generic;
using UnityEngine;

public class RuntimePooler
{
    private string poolName;
    private GameObject prefab;
    private int poolSize;
    private Transform parent;
    private Transform poolParent;
    private List<GameObject> pool;
    public RuntimePooler(string poolName, GameObject prefab, int poolSize, Transform parent)
    {
        this.poolName = poolName;
        this.prefab = prefab;
        this.poolSize = poolSize;
        this.parent = parent;
        pool = new List<GameObject>();

        GameObject emptyObject = CreateEmptyParent(poolName, parent);

        CreatePool(prefab, poolSize, emptyObject.transform);
    }

    private static GameObject CreateEmptyParent(string poolName, Transform parent)
    {
        GameObject emptyObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        emptyObject.GetComponent<MeshRenderer>().enabled = false;
        emptyObject.GetComponent<Collider>().enabled = false;
        emptyObject.name = poolName + "Pool";
        emptyObject.transform.SetParent(parent, true);
        return emptyObject;
    }

    public void CreatePool(GameObject prefab , int poolSize, Transform parent)
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject toPool = GameObject.Instantiate(prefab.gameObject);
            toPool.transform.parent = parent;
            toPool.gameObject.SetActive(false);
            pool.Add(toPool);            
        }
    }

    public GameObject GetObject()
    {
        GameObject pooledObject = null;

        foreach (var go in pool)
        {
            if (!go.activeInHierarchy && go != null)
            {               
                pooledObject = go;
            }
        }

        if(pooledObject == null) pooledObject = CreateObject();
        pooledObject.gameObject.SetActive(true);
        return pooledObject;
    }

    private GameObject CreateObject()
    {
        GameObject toPool = GameObject.Instantiate(prefab);
        if (toPool.TryGetComponent(out IPooledObject pooledObject))
        {
            pooledObject.Initialize();
        }
        toPool.transform.parent = parent;
        toPool.gameObject.SetActive(false);
        pool.Add(toPool);
        
        return toPool;
    }

    public string GetPoolName() => this.poolName;
}
