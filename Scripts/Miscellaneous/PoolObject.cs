using UnityEngine;
using UnityEngine.Events;

public class PoolObject : MonoBehaviour , IPooledObject
{
    [SerializeField] private float inactivationDelay = 1.5f;
    [SerializeField] bool inactivateAtStart = true;
    [SerializeField] UnityEvent OnInitialize;
    [SerializeField] UnityEvent OnActivate;
    [SerializeField] UnityEvent OnDeactivate;
    public void Initialize()
    {
        OnInitialize?.Invoke();
        Deactivate();
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        OnActivate?.Invoke();
        if(inactivateAtStart)
        {
            Invoke(nameof(Deactivate), inactivationDelay);
        }
    }
    public void Deactivate()
    {
        OnDeactivate?.Invoke();
        gameObject.SetActive(false);
    }
}
