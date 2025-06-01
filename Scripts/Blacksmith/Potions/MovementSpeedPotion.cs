using MoreMountains.Feedbacks;
using UnityEngine;

public class MovementSpeedPotion : Potion , IResettablePotion
{
    [SerializeField] private float influenceToMovementSpeed = 7f;
    [SerializeField] MMF_Player useFeedbacks;
    [SerializeField] GameObject vfx;
    [SerializeField] Vector3 vfxOffset;
    public void ResetPotionEffect()
    {
        PlayerController.Instance.GetComponent<PlayerStateMachine>().InfluenceMovementSpeed(-influenceToMovementSpeed);
    }

    public override void UsePotion(PotionUser potionUser)
    {
        PlayerController.Instance.GetComponent<PlayerStateMachine>().InfluenceMovementSpeed(influenceToMovementSpeed);
        Instantiate(vfx , potionUser.transform.position + vfxOffset , Quaternion.identity);
        if (useFeedbacks != null) { useFeedbacks.PlayFeedbacks(); }
    }

}
