using MoreMountains.Feedbacks;
using UnityEngine;

public class NotificationVisual : MonoBehaviour
{
    public bool Notifying = false;
    [SerializeField] MonoBehaviour[] feedbacks;
    [SerializeField] MMF_Player feedbacks2;
    [SerializeField] float notifyInterval = 3f;
    [SerializeField] GameObject[] highLightObjects;
    float notifyTimer;

    private void Update()
    {
        if(Notifying && Time.time > notifyTimer)
        {
            PlayFeedbacks();
        }
    }
    public void StartNotifying()
    {
        Notifying = true;
        foreach (var item in highLightObjects)
        {
            item.SetActive(true);
        }
    }

    private void PlayFeedbacks()
    {
        notifyTimer = Time.time + notifyInterval;
        foreach (var feedback in feedbacks)
        {
            (feedback as IVisualFeedback)?.Perform();
        }
        if(feedbacks2) feedbacks2.PlayFeedbacks();
    }

    public void StopNotifying()
    {
        Notifying = false;
        foreach (var feedback in feedbacks)
        {
            (feedback as IVisualFeedback)?.Stop();
        }
        foreach (var item in highLightObjects)
        {
            item.SetActive(false);
        }
    }
}
