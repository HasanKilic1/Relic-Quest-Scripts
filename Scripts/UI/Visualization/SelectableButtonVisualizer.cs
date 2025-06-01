using MoreMountains.Feedbacks;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SelectableButtonVisualizer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    public float scaleMultiplier = 1.2f;
    public float animationDuration = 0.2f;
    private Vector3 originalScale;
    public MMF_Player HighLightFeedbacks;
    public MMF_Player UnHighLightFeedbacks;
    public UnityEvent OnHighlight;
    public UnityEvent OnUnHighlight;
    private void Awake()
    {
        originalScale = transform.localScale;
    }
    private void Start()
    {
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        HighlightButton();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UnhighlightButton();
    }

    public void OnSelect(BaseEventData eventData)
    {
        HighlightButton();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        UnhighlightButton();
    }

    private void HighlightButton()
    {
        StopAllCoroutines();
        StartCoroutine(ScaleButton(originalScale * scaleMultiplier));

        if(HighLightFeedbacks != null) HighLightFeedbacks.PlayFeedbacks();
        OnHighlight?.Invoke();
    }

    private void UnhighlightButton()
    {
        StopAllCoroutines();
        StartCoroutine(ScaleButton(originalScale));

        if(UnHighLightFeedbacks != null) UnHighLightFeedbacks.PlayFeedbacks();
        OnUnHighlight?.Invoke();
    }

    private IEnumerator ScaleButton(Vector3 targetScale)
    {
        Vector3 initialScale = transform.localScale;
        float time = 0;

        while (time < animationDuration)
        {
            transform.localScale = Vector3.Lerp(initialScale, targetScale, time / animationDuration);
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
    }
}
