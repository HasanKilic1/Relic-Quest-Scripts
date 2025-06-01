using MoreMountains.Feedbacks;
using UnityEngine;

public class PlayerFeedbackController : MonoBehaviour
{
    PlayerHealth playerHealth;
    MMF_FloatingText healthFloatingText;
    [SerializeField] MMF_Player healthIncreaseEffectFb;
    [SerializeField] MMF_Player damageTakenFb;
    [SerializeField] MMF_Player projectileEffectFb;
    [SerializeField] MMF_Player poisonFeedbacks;
    [SerializeField] MMF_Player flameFeedbacks;
    [SerializeField] MMF_Player electrictyFeedbacks;
    
    void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
    }

    public void PlayCardSettleFeedbacks(IRelic relic)
    {
        switch (relic)
        {
            case GiveHealthAbility:
                string value ="+" + ((int)playerHealth.GetMaxHealth / 5).ToString();
                PlayHealthIncraseFeedbacks(value);
                break;

            case MaxHealthAbility:
                string value2 = $"MAX HEALTH INCREASED";
                PlayHealthIncraseFeedbacks(value2);
                break;

            case ProjectileEffect:
                projectileEffectFb.PlayFeedbacks();
                break;
            default:
                break;
        }

    }

    public void PlayHealthIncraseFeedbacks(string value)
    {
        healthFloatingText = healthIncreaseEffectFb.GetFeedbackOfType<MMF_FloatingText>();
        healthFloatingText.Value = value;
        healthIncreaseEffectFb.PlayFeedbacks();
    }

    public void PlayDamageFeedbacks()
    {
        damageTakenFb?.PlayFeedbacks();
    }

    public void PlayInfluenceFeedbacks(InfluenceType influenceType)
    {
        switch (influenceType)
        {
            case InfluenceType.Flame:
                flameFeedbacks?.PlayFeedbacks();
                break;
            case InfluenceType.Poison:
                poisonFeedbacks?.PlayFeedbacks();
                break;
            case InfluenceType.Electricity:
                electrictyFeedbacks?.PlayFeedbacks();   
                break;
        }
    }
}
