using UnityEngine;
public abstract class ProjectileEffect : MonoBehaviour
{
    [SerializeField] protected int damage = 20;

    [Tooltip("0f means enemy will be stopped completely , 100f means enemy movement speed won't be effected")]
    [Range(0 , 100)][SerializeField] protected float influenceToEnemyMovementSpeed = 100f;
    [Range(-1 , 1)][SerializeField] protected float influenceToEnemyAnimatorSpeed = 0f;
    [SerializeField] protected float influenceDuration = 1.5f;

    protected float lastAppliedTime;
    protected float inactivationTime;
    public abstract void ApplyEffectToEnemy(EnemyHealth enemyHealth);
        
    protected void GiveDamageOnArea(Vector3 pos , float radius , LayerMask enemyLayer , out bool enemyFound)
    {
        Collider[] colls = Physics.OverlapSphere(pos, radius, enemyLayer);
        enemyFound = false;
        foreach (Collider coll in colls)
        {
            if (coll.TryGetComponent(out EnemyHealth enemyHealth))
            {
                enemyHealth.TakeDamage(damage, Vector3.zero, isUnstoppableAttack: true);
                enemyFound = true;
            }
        }
    }

    
}
