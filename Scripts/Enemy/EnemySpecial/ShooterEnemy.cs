using MoreMountains.Feedbacks;
using System;
using System.Collections;
using UnityEngine;

public class ShooterEnemy : Enemy
{
    
    [SerializeField] bool createPool = true;
    [SerializeField] string POOL_NAME = "X_Shooter";
       
    [SerializeField] bool RANDOMIZE_PROPERTIES = false;
    [SerializeField] string attack_parameter_name = "Attack";
    [SerializeField] float attack_duration = 1f;
    [SerializeField] float time_before_shoot = 0.3f; 
    [SerializeField] ShootObjectType shootObject = ShootObjectType.Projectile;
    [SerializeField] ShootType shootType = ShootType.Singular;
    [SerializeField] EnemyBallistic shootableObject;

    [Header("If shoot type is multiple")]
    [SerializeField] int objectSpawnCount = 8;
    [Range(0,360)][SerializeField] float objectSpawnAngle = 180f;
    [SerializeField] float perSpawnDuration = 0.2f;
   
    [SerializeField] Transform shootPosition;
    [SerializeField] Vector3 spawnOffset;
    [SerializeField] MMF_Player attackFeedbacks;

    [Header("Trajectory randomness")]
    [SerializeField] bool useRandomTargetOffset;
    [SerializeField] Vector3 randomSensMin;
    [SerializeField] Vector3 randomSensMax;
    protected override void Awake()
    {
        base.Awake();    
    }
    
    protected override void Start()
    {
        base.Start();

        if (RANDOMIZE_PROPERTIES)
        {
            Randomize();
        }
        if(createPool)
        {
            SceneObjectPooler.Instance.CreatePool(POOL_NAME, shootableObject.gameObject, 20);
        }
        perSpawnDuration = attack_duration / objectSpawnCount;        
    }

    private void Randomize()
    {
        UnityEngine.Random.InitState(RandomSeeder.GetSeed());

        float rand = UnityEngine.Random.Range(-1f, 1f);
        if(rand < 0f)
        {
            shootObject = ShootObjectType.Projectile;
            shootType = ShootType.Singular;
        }
        else {
            shootObject = ShootObjectType.Trajectory;
            shootType = ShootType.Multiple;
        }

        objectSpawnCount = UnityEngine.Random.Range(1, 12);
        objectSpawnAngle = UnityEngine.Random.Range(0f, 360f);
        attackRange = UnityEngine.Random.Range(5f, 20f);
    }

    protected override void Update()
    {
        base.Update();
        HandleStates();

        if(currentState == EnemyState.Attack) { FaceToTarget(playerTargetedPosition.position); }
    }

    protected override void Attack()
    {        
        if(attackFeedbacks) attackFeedbacks.PlayFeedbacks();
        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        ResetAttackTimer();
        Stop();
        animator.SetTrigger(attack_parameter_name);
        FaceToTarget(playerTargetedPosition.position);

        yield return new WaitForSeconds(time_before_shoot);
        Shoot();

        yield return new WaitForSeconds(attack_duration);
        HandleAttackEnd();
    }
  
    private void Shoot()
    {
        FaceToTarget(PlayerHealth.Instance.transform.position, lookInstantly: true);
        if(shootType == ShootType.Singular)
        {
            ShootBallistics(transform.forward);
        }
        else
        {
            StartCoroutine(MultipleShootRoutine());
        }
    }

    private IEnumerator MultipleShootRoutine()
    {
        float rotateAngle = objectSpawnAngle / objectSpawnCount;
        Vector3 shootDirection = transform.forward;
        
        for (int i = 0; i < objectSpawnCount; i++)
        {
            int angleMultiplier = i % 2 == 0 ?  i :  -i; 

            shootDirection =  Quaternion.AngleAxis(rotateAngle * angleMultiplier, Vector3.up) * shootDirection;

            switch (shootObject)
            {
                case ShootObjectType.Projectile:

                    ShootBallistics(shootDirection);

                    break;

                case ShootObjectType.Trajectory:
                    
                    Vector3 targetPoint = transform.position + shootDirection * diffToPlayer.magnitude;
                    targetPoint.y = 0.3f;
                    if(useRandomTargetOffset)
                    {
                        targetPoint.x += UnityEngine.Random.Range(randomSensMin.x, randomSensMax.x);
                        targetPoint.z += UnityEngine.Random.Range(randomSensMin.z, randomSensMax.z);
                    }
                    ShootBallistics(targetPoint);

                    break;
            }
           
            yield return new WaitForSeconds(perSpawnDuration);
        }
    }

    private void ShootBallistics(Vector3 directionOrPoint)
    {
        EnemyBallistic shootable;
        if (createPool)
        {
            shootable = SceneObjectPooler.Instance.GetObjectFromPool(POOL_NAME, shootableObject.gameObject).GetComponent<EnemyBallistic>();
        }
        else shootable = Instantiate(shootableObject);

        if (shootable.TryGetComponent(out IPooledObject pooledObject))
        {
            pooledObject.Activate();
        }

        shootable.gameObject.transform.position = shootPosition.position + spawnOffset;       
        shootable.SetRange(attackRange);
        shootable.SetDamage((int)damage);
        shootable.Shoot(directionOrPoint);
        shootable.SetTarget(PlayerHealth.Instance.transform);
    }


    public float Damage => this.damage;
}
public enum ShootObjectType
{
    Projectile,
    Trajectory
}
public enum ShootType
{
    Singular,
    Multiple,
}