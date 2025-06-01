using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardSlot : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI quantity;

    [SerializeField] MMF_Player revealFeedbacks;
    [SerializeField] MonoBehaviour[] revealFeedbacks2;
    public void SetReward(RewardSO rewardSO)
    {
        icon.sprite = rewardSO.Icon;
        quantity.text = rewardSO.GetRewardAmount().ToString();

        if (revealFeedbacks != null) {
            revealFeedbacks.PlayFeedbacks();
        }
        if (revealFeedbacks2.Length > 0) 
        {
            foreach (var fb in revealFeedbacks2)
            {
                (fb as IVisualFeedback)?.Perform();
            }
        }
    }
}
