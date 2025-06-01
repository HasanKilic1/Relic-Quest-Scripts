using MoreMountains.Feedbacks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour , IPooledObject
{
    public static event Action<EnemyHealth> OnAnyProjectileCollision;
    public UnityEvent<EnemyHealth> OnThisProjectileCollision;
    
    [SerializeField] GameObject hitVfx;
    [SerializeField] TrailRenderer trail;
    [SerializeField] int damage;
    [SerializeField] float speed;
    [SerializeField] MMF_Player collisionFeedbacks;
    [SerializeField] float lifeTime = 6f;
    [SerializeField] bool Y_0 = false;
    public bool Penetrative = false;
    public bool CanBounce = false;
    public int BounceCount = 3;
    private int bounced = 0;

    private float timeElapsed;

    Transform target;
    Vector3 direction;
    private bool isHoming = false;
    
    public bool isOwnedByPlayer;
    public bool isPooled = false;
    private float healthRegenerationRatio;
    private float criticalDamageChance;


    void Update()
    {
        Movement();
        timeElapsed += Time.deltaTime;
        if(timeElapsed > lifeTime)
        {
            timeElapsed = 0f;
            HandleLifeEnd();
        }
    }

    private void Movement()
    {
        if (isHoming)
        {
            if (target != null)
            {
                Vector3 diff = target.position - transform.position;
                transform.forward = diff.normalized;
            }
            transform.position += speed * Time.deltaTime * transform.forward;
        }
        else
        {
            transform.forward = direction.normalized;
            if (Y_0) direction.y = 0; 
            transform.position += speed * Time.deltaTime * direction.normalized;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<IContactProvider>() != null && !Penetrative)
        {
            HandleLifeEnd();
            return;
        }
        if (other.TryGetComponent(out EnemyHealth enemy))
        {
            GiveDamage(enemy);            
            TryRegenerateHealth();            
            HandleCollisionOnContact(enemy.GetTargetedPos().position);
            
            if(CanBounce)
            {
                bounced++;
                Bounce(enemy);
            }
        }
        
    }

    #region CollisionWithEnemy
    private void GiveDamage(EnemyHealth enemy)
    {
        if (UnityEngine.Random.Range(0, 100f) <= criticalDamageChance)
        {
            enemy.TakeDamage(damage * 2, hitDirection: transform.forward);
        }
        else
        {
            enemy.TakeDamage(damage, hitDirection: transform.forward);
        }
        OnAnyProjectileCollision?.Invoke(enemy);
        OnThisProjectileCollision?.Invoke(enemy);
    }

    private void HandleCollisionOnContact(Vector3 contactPos)
    {
        if(hitVfx != null)
        {
            GameObject vfx = Instantiate(hitVfx, contactPos, Quaternion.identity);
            Destroy(vfx, 1.5f);
        }
       
        collisionFeedbacks?.PlayFeedbacks();  
        if(CanBounce || Penetrative)
        {
            return;            
        }
        HandleLifeEnd();
    }

    private void Bounce(EnemyHealth current)
    {   
        if(bounced > BounceCount) { HandleLifeEnd(); }

        Vector3 directionToClosest = Vector3.zero;
        List<EnemyHealth> allEnemies = FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None).ToList();
        if(current != null)
        {
            allEnemies.Remove(current);
        }

        if(allEnemies.Count > 0)
        {
            float closestDistance = 10000f;
            foreach (var enemy in allEnemies)
            {
                float distanceToThis = Vector3.Distance(transform.position, enemy.transform.position);
                if (distanceToThis < closestDistance)
                {
                    closestDistance = distanceToThis;
                    directionToClosest = enemy.GetTargetedPos().position - transform.position;
                }
            }
            SetDirection(directionToClosest);
        }
        else
        {
            if (!Penetrative)
            {
                HandleLifeEnd();
            }
        }
    }

    private void TryRegenerateHealth()
    {
        if (isOwnedByPlayer)
        {
            float regeneration = damage * healthRegenerationRatio / 100f;
            PlayerHealth.Instance.IncreaseHealth((int)regeneration);
            AchievementManager.Instance.EffectQuestByType(QuestType.RegenerateHealth, (int)regeneration);
        }
    }

    private void HandleLifeEnd()
    {
        if (isPooled)
        {
            Deactivate();
        }
        else Destroy(gameObject);
    }
    #endregion

    #region Getters and Setters 

    public int GetDamage => this.damage;
    public float GetSpeed => speed;
    public void SetDamage(int damage) => this.damage = damage;
   
    public void SetCriticalDamageChance(float criticalDamageChance) => this.criticalDamageChance = criticalDamageChance;  

    public void SetTarget(Transform target) => this.target = target;
    public void SetDirection(Vector3 direction) => this.direction = direction;

    public void SetRegenerationRatio(float regenerationRatio) => this.healthRegenerationRatio = regenerationRatio;
    
    public void SetSpeed(float speed) => this.speed = speed;

    #endregion

    #region ObjectPool
    public void Initialize()
    {
        isPooled = true;
        Deactivate();
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        if (trail)
        {
            trail.Clear();
            trail.enabled = true;
        }
    }

    public void Deactivate()
    {
        bounced = 0;
        gameObject.SetActive(false);
        if(trail != null) 
        { 
            trail.Clear();
            trail.enabled = false;
        }
    }

    #endregion

}
