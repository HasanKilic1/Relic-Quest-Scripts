using MoreMountains.Feedbacks;
using System.Collections;
using UnityEngine;

public class DangerSign : MonoBehaviour
{
    [SerializeField] MMF_Player activationFeedBacks;

    public void ShowDangerOnTime(float time)
    {
        activationFeedBacks?.PlayFeedbacks();
    }

}
