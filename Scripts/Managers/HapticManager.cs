using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class HapticManager : MonoBehaviour
{
    public static HapticManager instance;
    private Gamepad currentGamepad;
    private bool isImpulsing;
    private bool isDisabled;
    private void Awake()
    {        
        if(instance != null && instance != this) Destroy(gameObject);
        else instance = this;

        currentGamepad = Gamepad.current;
    }

    public void Impulse(float lowFrequency , float highFrequency , float duration)
    {
        currentGamepad = Gamepad.current;
        if(currentGamepad == null) { return; }
        if(isImpulsing || isDisabled) { return; }

        currentGamepad.SetMotorSpeeds(lowFrequency, highFrequency);
        StartCoroutine(StopImpulse(duration));
    }

    private IEnumerator StopImpulse(float delay)
    {
        float t = 0f;
        while (t < delay)
        {
            t += Time.deltaTime;
            yield return null;
        }

        currentGamepad?.ResetHaptics();
        isImpulsing = false;
    }
    public void DisableHaptics()
    {
        isDisabled = true;
        currentGamepad?.SetMotorSpeeds(0f, 0f);
    }
    public void EnableHaptics()
    {
        isDisabled = false;
    }
    public Gamepad GetGamepad() { return currentGamepad; }

    public void StopImpulseImmediately()
    {
        currentGamepad?.SetMotorSpeeds(0f, 0f);
        currentGamepad?.ResetHaptics();
    }
    private void OnApplicationQuit()
    {
        currentGamepad?.ResetHaptics();
    }
}
