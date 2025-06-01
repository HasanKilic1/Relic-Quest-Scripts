using System.Collections;
using UnityEngine;

public class ScaleAnimationer : MonoBehaviour , IVisualFeedback
{    
    public TimeScaleMode TimeScaleType = TimeScaleMode.Scaled;
    public Transform targetTransform;
    [SerializeField] public Vector3 startScale;
    [SerializeField] public Vector3 targetScale;
    [SerializeField] private float scaleDuration;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private bool applyAtStart;
    [SerializeField] private bool applyAtEnable;
    [SerializeField] private bool inactivateOnFinish;
    [SerializeField] private bool returnStdScale;
    private Vector3 constantScale;
    private void OnEnable()
    {
        if (applyAtEnable) 
        {
            Perform();
        }
    }
    private void Start()
    {
        constantScale =targetTransform ? targetTransform.localScale : transform.localScale;
        if(applyAtStart)
        {
            Perform();
        }
    }

    public void Perform()
    {
        transform.localScale = startScale;
        StartCoroutine(ScaleRoutine());
    }

    private IEnumerator ScaleRoutine()
    {
        float elapsed = 0f;
        Transform target = targetTransform != null ? targetTransform : transform;
        while (elapsed < scaleDuration)
        {
            if(TimeScaleType == TimeScaleMode.Scaled)
            {
                elapsed += Time.deltaTime;
            }
            else { elapsed += Time.unscaledDeltaTime; }
            float value = curve.Evaluate(elapsed / scaleDuration);
            target.localScale = Vector3.Lerp(startScale, targetScale, value);
            yield return null;
        }
        
        StopAllCoroutines();
        if(inactivateOnFinish) { gameObject.SetActive(false); }
    }

    public void Stop()
    {
        if(targetTransform) targetTransform.localScale = constantScale;
        else
        {
            transform.localScale = constantScale;
        }
    }
}

public enum TimeScaleMode
{
    Scaled,
    Unscaled
}