using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroyer : MonoBehaviour
{
    [SerializeField] bool destroysOnStart = true;
    [SerializeField] float destroyTime;    
    public bool DisableMode = false;

    private void OnEnable()
    {
        if (DisableMode)
        {
            StopCoroutine(DisableRoutine()); // Refresh coroutine
            StartCoroutine(DisableRoutine());
        }
    }
    void Start()
    {
        if(destroysOnStart)
        {
            Destroy(gameObject, destroyTime);
        }
    }

    public void SelfDestroy(float time)
    {
        Destroy(gameObject , time);
    }

    private IEnumerator DisableRoutine()
    {
        float t = 0f;
        while (t < destroyTime) 
        {
            t += Time.deltaTime;
            yield return null;
        }
        gameObject.SetActive(false);    
    }
}
