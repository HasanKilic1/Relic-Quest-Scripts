using MoreMountains.Feedbacks;
using UnityEngine;

public class Mjolnir : MonoBehaviour
{
    [SerializeField] MMF_Player collisionFeedBack;
    [SerializeField] MMF_Player exitFeedBack;
    [SerializeField] Transform collisionCenter;
    [SerializeField] LayerMask floorLayer;   
    [SerializeField] float speed = 20f;
    [SerializeField] float acceleration = 10f;
    bool collided = false;
    bool exitFeedBackPlayed = false;
    float timeToMoveUp;
    float timeElapsedMovingUp;

    public float damage;
    [SerializeField] private float damageRadius;
    [SerializeField] private LayerMask enemyLayer;

    void Update()
    {
        if (!collided)
        {
            MoveDown();
        }  
        if(collided && Time.time > timeToMoveUp)
        {
            MoveUp();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<Floor>() != null)
        {
            if (!collided)
            {
                Collide();
            }
        }        
    }
    private void MoveDown()
    {
        speed += acceleration * Time.deltaTime;
        transform.position += -Vector3.up * speed * Time.deltaTime;
    }

    private void MoveUp()
    {       
        if (!exitFeedBackPlayed)
        {
            exitFeedBack.PlayFeedbacks();
            exitFeedBackPlayed = true;
        }
        speed += acceleration * Time.deltaTime;
        transform.position += Vector3.up * speed * Time.deltaTime;

        timeElapsedMovingUp += Time.deltaTime;
        if(timeElapsedMovingUp > 5f)
        {
            Destroy(gameObject);
        }
    }

    private void Collide()
    {
        collided = true;
        collisionFeedBack.PlayFeedbacks();
        timeToMoveUp = Time.time + 4f;
        GiveDamage(transform.position);
    } 

    public void SetDamage(float damage)
    {
        this.damage = damage;
    }

    private void GiveDamage(Vector3 center)
    {
        Collider[] enemies = Physics.OverlapSphere(collisionCenter.position, damageRadius, enemyLayer);
        if (enemies.Length > 0)
        {
            foreach (Collider coll in enemies)
            {
                if (coll.TryGetComponent(out EnemyHealth enemy))
                {
                    enemy.TakeDamage((int)damage, Vector3.zero, isUnstoppableAttack: true);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(collisionCenter.position, damageRadius);
    }
}
