using MoreMountains.Feedbacks;
using UnityEngine;

public class ElectricityAbility : ProjectileEffect, IRelic
{
    PlayerStateMachine playerStateMachine;
    [SerializeField] float damageZoneRadius = 6f;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] MMF_Player feedbacks;
    MMF_InstantiateObject instantiateObject;
    private int totalShot;

    public RelicSO RelicSO { get; set;}

    private void OnEnable()
    {
        Projectile.OnAnyProjectileCollision += ApplyEffectToEnemy;
    }
    private void OnDisable()
    {
        Projectile.OnAnyProjectileCollision -= ApplyEffectToEnemy;
    }

    private void Awake()
    {
        instantiateObject = feedbacks.GetFeedbackOfType<MMF_InstantiateObject>();
    }

    public string Declaration() => "Your hits shocks your enemies";

    public void SettleEffect(PlayerStateMachine anyPlayerScript)
    {
        playerStateMachine = anyPlayerScript;
    }

    public override void ApplyEffectToEnemy(EnemyHealth enemyHealth)
    {
        EnemyVisualizer enemyVisualizer = enemyHealth.GetComponent<EnemyVisualizer>();
        totalShot++;

        if (enemyVisualizer == null || enemyHealth.GetCurrentHealth() <= 0) { return; }
        if (totalShot % 3 == 0)
        {
            enemyVisualizer.Influence(influenceToEnemyMovementSpeed , influenceToEnemyAnimatorSpeed , influenceDuration);
            Vector3 pos = new(enemyHealth.transform.position.x , 1.5f , enemyHealth.transform.position.z);
            instantiateObject.TargetPosition = pos;
            feedbacks.PlayFeedbacks();
            GiveDamageOnArea(enemyHealth.transform.position , damageZoneRadius , enemyLayer , out bool enemyFound);
        }                
    }

}
