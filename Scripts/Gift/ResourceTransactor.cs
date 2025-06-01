using System;
using UnityEngine;

public abstract class ResourceTransactor : MonoBehaviour
{
    protected EconomyManager economyManager;
    protected virtual void Start()
    {
        economyManager = EconomyManager.Instance;
    }
    public abstract void PerformTransaction();
}
