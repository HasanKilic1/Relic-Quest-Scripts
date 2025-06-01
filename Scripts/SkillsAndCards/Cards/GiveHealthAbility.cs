using UnityEngine;

public class GiveHealthAbility : MonoBehaviour, IRelic
{
    public RelicSO RelicSO { get; set; }

    public string Declaration() => "Restore %20 of your health";
    public void SettleEffect(PlayerStateMachine anyPlayerScript)
    {
        PlayerHealth playerHealth = anyPlayerScript.GetComponent<PlayerHealth>();
        playerHealth.IncreaseHealth((int)(playerHealth.GetMaxHealth / 5));
    }

    public void ResetEffect(PlayerStateMachine anyPlayerScript)
    {
        // none , stackable
    }

    
}
