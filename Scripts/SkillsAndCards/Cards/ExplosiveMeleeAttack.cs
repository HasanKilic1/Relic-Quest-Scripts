using MoreMountains.Feedbacks;
using System.Collections;
using UnityEngine;

public class ExplosiveMeleeAttack : MonoBehaviour , IRelic , IMeleeAttackCard
{
    Shooter shooter;
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] int damage;
    [SerializeField] float radius;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] float distanceBetweenExplosions = 4f;
    [SerializeField] int explosionCount = 5;
    [SerializeField] float timeBetweenExplosions = 1f;
    [SerializeField] float cooldown = 10f;
    [SerializeField] MMF_Player onExplosionStarted;
    [SerializeField] MMF_Player onExplosionContinued;
    public RelicSO RelicSO { get; set;}

    private bool isUsing = false;
    private float timeToUse;
    private void OnEnable()
    {
        Projectile.OnAnyProjectileCollision += SendExplosions;
    }

    private void OnDisable()
    {
        Projectile.OnAnyProjectileCollision -= SendExplosions;
    }


    public void ResetEffect(PlayerStateMachine anyPlayerScript)
    {}

    public void SettleEffect(PlayerStateMachine playerStateMachine)
    {
        shooter = playerStateMachine.selectedCharacter.GetComponent<Shooter>();        
    }

    private void SendExplosions(EnemyHealth enemyHealth)
    {
        if(isUsing || Time.time < timeToUse) { return; }
        Transform firstEnemy = enemyHealth.transform;
        Vector3 direction = -enemyHealth.GetTargetedPos().forward; //shooter.transform.forward;
        StartCoroutine(ExplosionRoutine(firstEnemy.position , direction));

        timeToUse = Time.time + cooldown;
    }

    private IEnumerator ExplosionRoutine(Vector3 startPosition ,  Vector3 direction)
    {
        onExplosionStarted?.PlayFeedbacks();
        for (int i = 0; i < explosionCount; i++)
        {   
            Vector3 location = startPosition + distanceBetweenExplosions * i * direction.normalized;
            GameObject explosion = Instantiate(explosionPrefab , location , Quaternion.identity);
            Destroy(explosion , 2.5f);
            GiveDamageOnArea(location);
            yield return new WaitForSeconds(timeBetweenExplosions);
            onExplosionContinued?.PlayFeedbacks();
        }
        isUsing = false;
    }

    private void GiveDamageOnArea(Vector3 pos)
    {
        Collider[] colls = Physics.OverlapSphere(pos, radius, enemyLayer);
        foreach (Collider coll in colls)
        {
            if (coll.TryGetComponent(out EnemyHealth enemyHealth))
            {
                enemyHealth.TakeDamage(damage, Vector3.zero, isUnstoppableAttack: true);
            }
        }
    }

}
