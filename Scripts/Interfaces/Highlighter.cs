using MoreMountains.Feedbacks;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Highlighter : MonoBehaviour 
{
    public UnityEvent OnHighlightComplete;
    public float HighLightDuration = 2f;
    public float interval = 1f;
    [SerializeField] MMF_Player highLightFeedbacks;
    [SerializeField] MonoBehaviour[] IVisualFeedbacks;    

    public void HighLight()
    {
        PlayFeedbacks();
        StartCoroutine(HighlightRoutine());
    }

    private IEnumerator HighlightRoutine()
    {
        float t0 = 0f;
        float t1 = 0f;
        while (t0 < HighLightDuration)
        {
            t0 += Time.deltaTime;
            t1 += Time.deltaTime;
            if (t1 > interval)
            {
                t1 = 0f;
                PlayFeedbacks();
            }
            yield return null;
        }
        UnHighLight();
    }

    private void PlayFeedbacks()
    {
        if (highLightFeedbacks != null) highLightFeedbacks.PlayFeedbacks();

        foreach (var visualFeedback in IVisualFeedbacks)
        {
            (visualFeedback as IVisualFeedback).Perform();
        }
    }

    public void UnHighLight()
    {
        OnHighlightComplete?.Invoke();
        HKDebugger.LogSuccess("Highlight completed");
        StopFeedbacks();
    }

    private void StopFeedbacks()
    {
        if (highLightFeedbacks != null) highLightFeedbacks.StopFeedbacks();

        foreach (var visualFeedback in IVisualFeedbacks)
        {
            (visualFeedback as IVisualFeedback).Stop();
        }
    }
}
