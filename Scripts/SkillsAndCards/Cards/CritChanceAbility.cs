using UnityEngine;

public class CritChanceAbility : ProjectileEffect, IRelic  
{
 //   [SerializeField] private float increaseBase = 5f;
  //  [SerializeField] private float increasePerLevel = 3f;

    public RelicSO RelicSO { get; set;}

    private void Start()
    {
        
    }
    public override void ApplyEffectToEnemy(EnemyHealth enemyHealth)
    {
    }

    public string Declaration()
    {
        return "Increase crit damage chance";
    }

    public void ResetEffect(PlayerStateMachine anyPlayerScript)
    {
       // anyPlayerScript.selectedCharacter.GetComponent<Shooter>().InfluenceCriticalChance(-increase);
    }

    public void SettleEffect(PlayerStateMachine anyPlayerScript)
    {
      //  anyPlayerScript.selectedCharacter.GetComponent<Shooter>().InfluenceCriticalChance(increase);
    }


    public void Upgrade()
    {}
}
