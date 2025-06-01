using UnityEngine;

public class HapticUser : MonoBehaviour
{
    [SerializeField] public float lowFrequency;
    [SerializeField] public float highFrequency;
    [SerializeField] private float duration;
    [Range(0,100)][SerializeField] private float PlayChance = 100f;
    public void Play()
    {
        if(HapticManager.instance != null)
        {
            if (UnityEngine.Random.Range(0f, 100f) > PlayChance) { return; }
            HapticManager.instance.Impulse(lowFrequency, highFrequency, duration);
        }       
    }
}
