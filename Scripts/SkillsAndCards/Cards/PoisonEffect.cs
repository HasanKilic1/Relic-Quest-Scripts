using System.Collections;
using UnityEngine;

public class PoisonEffect : ProjectileEffect, IRelic
{
    [SerializeField] float damageZoneRadius = 6f;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] GameObject poisonCloudPrefab;
    private int totalShot;
    private bool poisonAreaActive = false;
    public RelicSO RelicSO { get; set; }

    private void OnEnable()
    {
        Projectile.OnAnyProjectileCollision += ApplyEffectToEnemy;
    }

    private void OnDisable()
    {
        Projectile.OnAnyProjectileCollision -= ApplyEffectToEnemy;
    }

    public string Declaration() => "One of your every three shot leaves a poisonous area";

    public void SettleEffect(PlayerStateMachine anyPlayerScript)
    {}
    public void ResetEffect(PlayerStateMachine anyPlayerScript)
    {}

    public override void ApplyEffectToEnemy(EnemyHealth enemyHealth)
    {
        EnemyVisualizer enemyVisualizer = enemyHealth.GetComponent<EnemyVisualizer>();

        if (enemyVisualizer == null || enemyHealth.GetCurrentHealth() <= 0 || poisonAreaActive) { return; }

        if (totalShot % 3 == 0)
        {
            enemyVisualizer.Influence(influenceToEnemyMovementSpeed, influenceToEnemyAnimatorSpeed, influenceDuration);
            Vector3 pos = new(enemyHealth.transform.position.x, 0.6f, enemyHealth.transform.position.z);

            StartCoroutine(PoisonCloud(pos));
        }
        totalShot++;
    }

    private IEnumerator PoisonCloud(Vector3 damageZone)
    {
        poisonAreaActive = true;
        GameObject poisonCloud = Instantiate(poisonCloudPrefab, damageZone, Quaternion.Euler(90,0,0));
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < 4; i++)
        {
            GiveDamageOnArea(damageZone, damageZoneRadius, enemyLayer , out bool enemyFound);
            if (enemyFound)
            {
                //
            }
            yield return new WaitForSeconds(0.5f);
        }
        Destroy(poisonCloud);
        poisonAreaActive = false;
    }

}
