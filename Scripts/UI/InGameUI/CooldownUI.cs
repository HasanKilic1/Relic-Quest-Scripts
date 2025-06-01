using MoreMountains.Feedbacks;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CooldownUI : MonoBehaviour
{
    public enum FillType
    {
        Image,
        Slider
    }
    public FillType fillType;
    [SerializeField] UnityEvent OnFinish;
    [SerializeField] Slider slider;
    [SerializeField] Image image;
    [SerializeField] MMF_Player finishFeedbacks;

    public void EnterCooldown(float cooldownDuration)
    {
        switch (fillType)
        {
            case FillType.Image:
                image.fillAmount = 0f;
                break;
            case FillType.Slider:
                slider.value = 0f;
                slider.maxValue = 1f;
                break;
        }
        
        StartCoroutine(RefreshRoutine(cooldownDuration));
    }

    private IEnumerator RefreshRoutine(float cooldownDuration)
    {
        float elapsed = 0f;
        while (elapsed < cooldownDuration) 
        {
            elapsed += Time.deltaTime;
            float fill = elapsed / (float)cooldownDuration;
            switch (fillType)
            {
                case FillType.Image:
                    image.fillAmount = fill;
                    break;
                case FillType.Slider:
                    slider.value = fill;
                    break;
            }
            
            yield return null;    
        }

        if (finishFeedbacks != null)
        {
            finishFeedbacks.PlayFeedbacks();
        }
        OnFinish?.Invoke();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        switch (fillType)
        {
            case FillType.Image:
                image.fillAmount = 1f;
                break;
            case FillType.Slider:
                slider.value = slider.maxValue;
                break;
        }
    }
    private void OnDestroy()
    {
        OnFinish.RemoveAllListeners();
    }
}
