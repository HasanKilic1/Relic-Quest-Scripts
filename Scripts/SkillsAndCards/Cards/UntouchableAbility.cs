using UnityEngine;

public class UntouchableAbility : MonoBehaviour , IRelic
{
    [SerializeField] float duration;
    float timeToRemove;
    public RelicSO RelicSO { get; set; }
    PlayerHealth playerHealth;

    private void Update()
    {
        if(Time.time > timeToRemove)
        {

        }
    }

    public string Declaration() => "Provides you temporary protection";

    public void SettleEffect(PlayerStateMachine anyPlayerScript)
    {
        anyPlayerScript.GetComponent<PlayerHealth>().CanTakeDamage = false;
        timeToRemove = Time.time + duration;
    }
    public void ResetEffect(PlayerStateMachine anyPlayerScript)
    {
     //   anyPlayerScript.GetComponent<PlayerHealth>().canBeUntouchable = false;
    }    
}
