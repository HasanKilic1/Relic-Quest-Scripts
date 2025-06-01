using MoreMountains.Feedbacks;
using UnityEngine;

public class FreezeEffect : ProjectileEffect,  IRelic 
{
    [SerializeField] float damageZoneRadius = 6f;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] MMF_Player feedbacks;
    [SerializeField] PoolObject impactVfx;
    private ObjectPooler<PoolObject> impactVfxPool;
    private int totalShot;
    public RelicSO RelicSO { get; set; }  
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
        impactVfxPool = new ObjectPooler<PoolObject>();
        impactVfxPool.InitializeObjectPooler(impactVfx, transform, 10);
    }
    public string Declaration() => "Slow down any enemy you hit";
    public void ResetEffect(PlayerStateMachine player)
    {}

    public void SettleEffect(PlayerStateMachine player)
    {}

    public override void ApplyEffectToEnemy(EnemyHealth enemyHealth)
    {
        EnemyVisualizer enemyVisualizer = enemyHealth.GetComponent<EnemyVisualizer>();
        if (enemyVisualizer == null) { return; }
        if (totalShot % 3 == 0)
        {
            enemyVisualizer.Influence(influenceToEnemyMovementSpeed, influenceToEnemyAnimatorSpeed, influenceDuration);
            ShowVfx(enemyHealth);
            feedbacks.PlayFeedbacks();
            GiveDamageOnArea(enemyHealth.transform.position, damageZoneRadius, enemyLayer, out bool enemyFound);
        }
        totalShot++;
    }

    private void ShowVfx(EnemyHealth enemyHealth)
    {
        Vector3 pos = new(enemyHealth.transform.position.x, 1.5f, enemyHealth.transform.position.z);
        Transform vfx = impactVfxPool.GetObject().transform;
        vfx.transform.position = pos;
    }

}
