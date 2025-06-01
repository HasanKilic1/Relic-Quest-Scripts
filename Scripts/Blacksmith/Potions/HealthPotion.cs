
public class HealthPotion : Potion
{
    public override void UsePotion(PotionUser potionUser)
    {
        int increase = (int)(PlayerHealth.Instance.GetMaxHealth * 20 / 100);
        PlayerHealth.Instance.IncreaseHealth(increase , playFeedbacks:true);
    }
}
