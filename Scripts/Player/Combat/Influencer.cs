using System.Collections;
using UnityEngine;

public class Influencer : MonoBehaviour
{
    private PlayerHealth playerHealth;
    private PlayerFeedbackController playerFeedbackController;

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
        playerFeedbackController = GetComponent<PlayerFeedbackController>();
    }

    public void TakeDamageOverTime(int totalDamage , float totalDuration = 5f, int loops = 5 , InfluenceType influenceType = InfluenceType.None)
    {
        StartCoroutine(DamageOverTimeCoroutine(totalDuration, totalDamage , loops));
        playerFeedbackController.PlayInfluenceFeedbacks(influenceType);
    }

    private IEnumerator DamageOverTimeCoroutine(float totalDuration , int totalDamage , int loops)
    {
        float waitBetweenLoops = totalDuration/loops;
        float damage = totalDamage/loops;
        for (int i = 0; i < loops; i++)
        {
            yield return new WaitForSeconds(waitBetweenLoops);
            playerHealth.TakeDamage((int)damage);
        }
    }
}
