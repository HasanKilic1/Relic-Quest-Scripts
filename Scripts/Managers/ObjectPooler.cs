using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler<T> : Object where T : MonoBehaviour
{
    // A generic object pooling class 

    public static ObjectPooler<T> Instance;
    private List<T> pool;
    public T prefab;
    private Transform parent;
    public void InitializeObjectPooler(T objectType, Transform parentObject, int initialSize)
    {
        this.prefab = objectType;
        parent = parentObject;
        pool = new List<T>();
        for (int i = 0; i < initialSize; i++)
        {
            T obj = CreateObject();
            if(parent != null)
            {
                obj.transform.parent = parentObject;
            }
        }
    }

    private T CreateObject()
    {
        T obj = Instantiate(prefab);
        if(parent != null) { obj.transform.SetParent(parent); }
        obj.gameObject.SetActive(false);
        (obj as IPooledObject).Initialize();
        pool.Add(obj);
        return obj;
    }

    public T GetObject()
    {
        foreach (T obj in pool)
        {
            if (!obj.gameObject.activeInHierarchy)
            {
                (obj as IPooledObject).Activate();
                return obj;
            }
        }

        return CreateObject();
    }

    public void ClearPool(PoolClearMethod method)
    {
        foreach (var item in pool)
        {
            if(method == PoolClearMethod.Destroy)
            {
                item.gameObject.SetActive(true);
                Destroy(item.gameObject);
            }
            else if(method == PoolClearMethod.Deactivate)
            {
                item.gameObject.SetActive(false);
            }
        }
        pool.Clear();
    }
}
public enum PoolClearMethod
{
    Destroy,
    Deactivate
}