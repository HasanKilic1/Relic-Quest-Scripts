using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolableVisualEffectObject : MonoBehaviour , IPooledObject
{
    [SerializeField] float deactivationTime = 2f;

    private void Start()
    {
        StartCoroutine(DeactivationRoutine());
    }

    private IEnumerator DeactivationRoutine()
    {
        yield return new WaitForSeconds(deactivationTime);
        Deactivate();
    }

    public void Activate()
    {
       gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void Initialize()
    {
        Deactivate();
    }

    
}
