using System.Collections;
using UnityEngine;

public abstract class Rune : MonoBehaviour
{
    [SerializeField] protected float maxDurationWhenEnabled = 3f;
    [HideInInspector] public bool isRunning;

    public abstract void Settle(RuneUser runeUser);
    public virtual void Run()
    {
        if (HapticManager.instance)
        {
            HapticManager.instance.DisableHaptics();
        }
        isRunning = true;
    }
    public virtual void Stop()
    {
        if (!isRunning) return;
        isRunning = false;

        if (HapticManager.instance)
        {
            HapticManager.instance.EnableHaptics();
        }        
    }

}
