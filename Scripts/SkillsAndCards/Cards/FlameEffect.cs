using System.Collections;
using UnityEngine;

public class FlameEffect : ProjectileEffect, IRelic
{
    [SerializeField] float damageZoneRadius = 6f;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] float waitBeforeExplosionStart;
    [SerializeField] float timeBetweenExplosions;
    private int totalShot;
    private bool meteorRainActive = false;

    public RelicSO RelicSO { get; set;}

    private void OnEnable()
    {
        Projectile.OnAnyProjectileCollision += ApplyEffectToEnemy;
    }

    private void OnDisable()
    {
        Projectile.OnAnyProjectileCollision -= ApplyEffectToEnemy;
    }

    public string Declaration() => "Burn them all!";
       
    public void SettleEffect(PlayerStateMachine anyPlayerScript)
    {}
    public void ResetEffect(PlayerStateMachine anyPlayerScript)
    {}

    public override void ApplyEffectToEnemy(EnemyHealth enemyHealth)
    {
        EnemyVisualizer enemyVisualizer = enemyHealth.GetComponent<EnemyVisualizer>();
        if (enemyVisualizer == null || enemyHealth.GetCurrentHealth() <= 0 || meteorRainActive) { return; }
        if (totalShot % 7 == 0)
        {
            enemyVisualizer.Influence(influenceToEnemyMovementSpeed, influenceToEnemyAnimatorSpeed, influenceDuration);
            Vector3 pos = NavMeshManager.Instance.FindClosestNavMeshPosition(enemyHealth.transform.position) + Vector3.up * 0.25f;
            StartCoroutine(MeteorRainRoutine(pos));
        }
        totalShot++;
    }

    private IEnumerator MeteorRainRoutine(Vector3 damageZone)
    {
        meteorRainActive = true;
        GameObject meteorRain = Instantiate(explosionPrefab , damageZone , Quaternion.identity);
        yield return new WaitForSeconds(waitBeforeExplosionStart);
        for (int i = 0; i < 3; i++)
        {
            GiveDamageOnArea(damageZone, damageZoneRadius, enemyLayer , out bool enemyFound);
            if (enemyFound)
            {
                //
            }
            yield return new WaitForSeconds(timeBetweenExplosions);
        }        
        Destroy(meteorRain);
        meteorRainActive = false;
    }

    public void Upgrade()
    {}

    protected void OnDrawGizmosSelected()
    {

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, damageZoneRadius);
    }
}
