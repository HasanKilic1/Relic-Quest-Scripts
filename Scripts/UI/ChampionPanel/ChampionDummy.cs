using MoreMountains.Feedbacks;
using System.Collections;
using UnityEngine;
[RequireComponent(typeof(PositionAnimationer))]
public class ChampionDummy : MonoBehaviour
{
    private PositionAnimationer positionAnimationer;
    [SerializeField] private Animator Animator;
    [SerializeField] private Vector3 RevealOffset;
    [SerializeField] private float animationPlayDelay;
    [SerializeField] MMF_Player revealFeedbacks;
    [SerializeField] float feedbackDelay;
    private void Awake()
    {
        positionAnimationer = GetComponent<PositionAnimationer>();
        AdjustFeedbackDelays();
    }

    public void Reveal(Vector3 targetRevealPos)
    {
        positionAnimationer.startPoint = targetRevealPos + RevealOffset;
        positionAnimationer.endPoint = targetRevealPos;
        positionAnimationer.AnimateExternal(transform);

        if (revealFeedbacks != null) { revealFeedbacks.PlayFeedbacks(); }
        
        Invoke(nameof(PlayAnim), animationPlayDelay);
    }

    private void AdjustFeedbackDelays()
    {
        if(revealFeedbacks == null) { return; }
        foreach (var feedback in revealFeedbacks.FeedbacksList)
        {
            feedback.Timing.InitialDelay = feedbackDelay;
        }
    }

    private void PlayAnim() => Animator.SetTrigger("Reveal");
}