using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.UIElements;

public class RangedEnemyAnim : MonoBehaviour
{
    [SerializeField] RangedEnemy rangedEnemy;
    [Header("Do not leave this area null if ranged enemy uses spray attack!")]
    [SerializeField] GameObject sprayVfx;
    bool isSpraying = false;
    private readonly float timeBetweenSprayAttacks = 0.2f;
    float sprayTimer;
    private void Start()
    {
        if(sprayVfx != null) rangedEnemy.OnAttackFinished += ResetSpray;
    }

    private void Update()
    {
        if (isSpraying)
        {
            sprayTimer += Time.deltaTime;
            if(sprayTimer > timeBetweenSprayAttacks)
            {
                SendRaycast();
                sprayTimer = 0f;
            }
        }
    }

    private void SendRaycast()
    {
        Ray ray = new(rangedEnemy.ShootPos.position,rangedEnemy.ShootPos.forward);                          
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit , rangedEnemy.Range))
        {
            if(hit.collider.TryGetComponent(out PlayerHealth playerHealth))
            {
                playerHealth.TakeDamage((int)rangedEnemy.Damage);
            }
        }
    }

    public void SingleShoot()
    {
        EnemyProjectile enemyProjectile = Instantiate(rangedEnemy.GetProjectile, rangedEnemy.ShootPos.position, Quaternion.identity);
        enemyProjectile.SetDamage((int)rangedEnemy.Damage);
        enemyProjectile.Shoot(rangedEnemy.ProjectileDirection.normalized);
        enemyProjectile.SetRange(rangedEnemy.Range);
    }

    public void Spray()
    {
        isSpraying =true;
        sprayVfx.SetActive(true);
    }

    public void ResetSpray()
    {
        isSpraying = false;
        sprayVfx.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(rangedEnemy.ShootPos.position, rangedEnemy.ShootPos.forward * rangedEnemy.Range);
    }
}
