using MoreMountains.Feedbacks;
using System.Collections;
using UnityEngine;

public class PlayerRangeIndicator : MonoBehaviour
{
    private float range;
    [SerializeField] float scaleByRange = 2.25f;
    [SerializeField] MMF_Player enemyInSightFeedbacks;
    [SerializeField] MMF_Player enemyOutSideFeedbacks;
    GameObject swordRuntime;
    PlayerStateMachine playerStateMachine;
    bool insightFeedBackPlayed;
    bool outsightFeedBackPlayed;
    private void Start()
    {
        playerStateMachine = PlayerHealth.Instance.GetComponent<PlayerStateMachine>();
        StartCoroutine(CheckRoutine());
    }

    private void Update()
    {
        transform.position = PlayerHealth.Instance.transform.position + Vector3.up * 0.7f;
    }
    private IEnumerator CheckRoutine()
    {
        while (true)
        {
            Check();
            yield return new WaitForSeconds(0.1f);            
        }
    }

    private void Check()
    {
        if (playerStateMachine.GetClosestEnemy())
        {
            if (!insightFeedBackPlayed)
            {
                insightFeedBackPlayed = true;
                outsightFeedBackPlayed = false;
                enemyInSightFeedbacks.PlayFeedbacks();
            }
        }
        else
        {
            if (!outsightFeedBackPlayed)
            {
                insightFeedBackPlayed = false;
                outsightFeedBackPlayed = true;
                enemyOutSideFeedbacks.PlayFeedbacks();
            }
        }
    }

    public void SetRange(float range)
    {
        transform.localScale = range * scaleByRange * Vector3.one;
    }
}
