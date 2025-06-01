using UnityEngine;

public class DamageAreaOverTime : MonoBehaviour
{
    [Header("Damageables")]
    [SerializeField] bool GiveDamageToPlayer;
    [SerializeField] bool GiveDamageToEnemy;

    [SerializeField] int damageOverTime;
    [SerializeField] float coolDown;

    [Header("Disable")]
    [SerializeField] bool DisableByLifeTime = true;
    [SerializeField] float lifeTime = 5f;

    private float damageTimer;

    private void OnEnable()
    {
        GameStateManager.OnLevelFinished += Disable;
    }

    private void OnDisable()
    {
        GameStateManager.OnLevelFinished -= Disable;
    }

    private void Start()
    {
        if (DisableByLifeTime)
        {
            Destroy(gameObject, lifeTime);
        }
    }

    private void Disable(int obj)
    {
        Destroy(gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        if(Time.time < damageTimer) { return; }
        
        if (GiveDamageToPlayer)
        {
            if(other.TryGetComponent(out PlayerHealth playerHealth))
            {
                playerHealth.TakeDamage(damageOverTime);
                damageTimer = Time.time + coolDown;
            }    
        }
        if(GiveDamageToEnemy)
        {
            if (other.TryGetComponent(out EnemyHealth enemyHealth))
            {
                enemyHealth.TakeDamage(damageOverTime , Vector3.zero , false);
                damageTimer = Time.time + coolDown;
            }
        }
    }

}
