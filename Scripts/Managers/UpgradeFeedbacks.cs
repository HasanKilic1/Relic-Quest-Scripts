using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeFeedbacks : MonoBehaviour
{
    [SerializeField] Transform sliderHandle;
    [SerializeField] MMF_Player finishFeedbacks;
    [SerializeField] MMF_Player sliderFeedbacks;
    [SerializeField] GameObject sliderVfx;
    private MMF_ScaleShake scaleShake;
    private MMF_InstantiateObject instantiateObject;

    private MMF_InstantiateObject sliderInstantiate;
    [SerializeField] int playCount = 7;
    [SerializeField] float xDiff;
    [SerializeField] float yDiff;
    private void Start()
    {
        scaleShake = finishFeedbacks.GetFeedbackOfType<MMF_ScaleShake>();
        instantiateObject = finishFeedbacks.GetFeedbackOfType<MMF_InstantiateObject>();
        sliderInstantiate = sliderFeedbacks.GetFeedbackOfType<MMF_InstantiateObject>();
    }

    public void PlayStartFeedbacks()
    {
        StartCoroutine(FollowSliderHandle());
    }
    private IEnumerator FollowSliderHandle()
    {
        for (int i = 0; i < playCount; i++)
        {
            Vector3 pos = new(sliderHandle.transform.position.x + xDiff, sliderHandle.transform.position.y + yDiff, 80); // world z of canvas is 90
            sliderInstantiate.TargetPosition = pos;
            sliderFeedbacks.PlayFeedbacks();
            yield return new WaitForSeconds(0.35f);           
        }
    }
    public void PlayFinishFeedbacks(PlayerAttributeUpgrader controller)
    {
        scaleShake.TargetShaker = controller.GetComponent<MMScaleShaker>();
        instantiateObject.TargetPosition = controller.transform.position;
        finishFeedbacks.PlayFeedbacks();
    }
}
