using System.Linq;
using UnityEngine;

public class BouncingProjectile : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] float speed = 20f;
    [SerializeField] GameObject hitVfx;
    [SerializeField] int totalBounce = 5;
    private int bounced = 0;
    EnemyHealth currentTargetedEnemy;
    Vector3 moveDirection;
    void Update()
    {
        if (currentTargetedEnemy == null) 
        { 
            ChangeMoveDirectionToOtherEnemy(); 
        }
        moveDirection = currentTargetedEnemy.transform.position - transform.position;
        moveDirection.y = 0;
        transform.position += moveDirection.normalized * Time.deltaTime * speed;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out EnemyHealth enemy))
        {
            enemy.TakeDamage(damage , Vector3.zero);
            bounced++;
            CheckBounce();
            ChangeMoveDirectionToOtherEnemy();           
            GameObject vfx = Instantiate(hitVfx, transform.position, Quaternion.identity);
            Destroy(vfx, 1.5f);
        }

    }

    private void ChangeMoveDirectionToOtherEnemy()
    {
        EnemyHealth[] enemies = GameObject.FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None);
        var validTargets = enemies.ToList();

        if (currentTargetedEnemy != null && enemies.Contains(currentTargetedEnemy))
        {
            validTargets.Remove(currentTargetedEnemy);
        }
        if (validTargets.Count == 0)
        {
            Destroy(gameObject);
            return;
        }

        float dist = 100000;
        for (int i = 0; i < validTargets.Count; i++)
        {
            if (Vector3.Distance(transform.position, validTargets[i].transform.position) < dist)
            {
                currentTargetedEnemy = validTargets[i];
                SetNewTarget(currentTargetedEnemy);
                dist = Vector3.Distance(transform.position, validTargets[i].transform.position);
            }
        }

        if(currentTargetedEnemy == null)
        {
            Destroy(gameObject);
        }
        else SetNewTarget(currentTargetedEnemy);
    }

    private void CheckBounce()
    {
        if (bounced >= totalBounce) { Destroy(gameObject); }
    }

    public void SetNewTarget(EnemyHealth enemy)
    {
        currentTargetedEnemy = enemy;
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

}
