using UnityEngine;

public class MaxHealthAbility : MonoBehaviour, IRelic
{
    public int increaseMaxHealth = 20;
    public RelicSO RelicSO { get; set; }

    public void SettleEffect(PlayerStateMachine anyPlayerScript)
    {
        PlayerHealth.Instance.InfluenceMaxHealth(increaseMaxHealth);
        Destroy(gameObject);
    }

}
