using System;
using System.Collections;
using UnityEngine;

public class BulletHell : MonoBehaviour
{
    RockThrowerEnemy rockThrowerEnemy;
    public event Action OnShootingStarted;
    public event Action OnShootingFinished;
    [SerializeField] EnemyProjectile projectile;
    [Range(0, 360)][SerializeField] float angleMax = 360;
    [SerializeField] int projectileCountPerWave = 4;
    [SerializeField] int waveCount = 3;
    [SerializeField] float timeBetweenProjectileWaves = 0.6f;
    private bool canShoot = true;
    Coroutine ProjectileRoutine;
    [SerializeField] Animator animator;

    private void Awake()
    {
        rockThrowerEnemy = GetComponentInParent<RockThrowerEnemy>();
    }
    public void ShootProjectiles()
    {
        if(ProjectileRoutine != null || !canShoot) { return; }
        StopAllCoroutines();
        ProjectileRoutine = StartCoroutine(ShootRoutine());
    }
  
    private IEnumerator ShootRoutine()
    {
        OnShootingStarted?.Invoke();
        int shooted = 0;
        float angle = angleMax / projectileCountPerWave;
        animator?.SetBool("isAttacking", true);
        canShoot = false;
        while(shooted < waveCount)
        {
            Vector3 dir = transform.forward;
            for (int i = 0; i < projectileCountPerWave; i++)
            {
                Vector3 shootPosition = transform.position + dir;
                EnemyProjectile spawned = Instantiate(projectile, shootPosition, Quaternion.identity);
                dir = Quaternion.AngleAxis(angle, Vector3.up) * dir;
                SetProjectile(dir, spawned);
            }
            shooted++;
            yield return new WaitForSeconds(timeBetweenProjectileWaves);
        }
        canShoot = true;
        OnShootingFinished?.Invoke();
        animator?.SetBool("isAttacking", false);
        ProjectileRoutine = null;
    }

    private void SetProjectile(Vector3 dir, EnemyProjectile spawned)
    {
        spawned.Shoot(dir);
        spawned.SetDamage((int)rockThrowerEnemy.Damage);
    }
}
