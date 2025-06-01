using MoreMountains.Feedbacks;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ExplosiveArea : EnemyBallistic, IPooledObject
{
    private ScaleAnimationer scaleAnimationer;
    public UnityEvent OnPlayerContact;
    [Header("Follow Behaviour")]
    [SerializeField] bool isFollower;
    public Transform FollowTo;
    [SerializeField] float followSensivity;
    [SerializeField] float followDuration;
    [SerializeField] bool alsoUseFollowRange;
    [SerializeField] float followRange = 100f;
    [SerializeField] float positionY;
    [SerializeField] bool randomizeXZ = true;
    [SerializeField] float xRandomDiff;
    [SerializeField] float zRandomDiff;

    [Header("General Properties")]
    [SerializeField] float radius;
    [SerializeField] int damage;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] int explosionCount;
    [SerializeField] float waitBeforeFirstExplosion;
    [SerializeField] float waitBetweenEveryExplosion;
    [SerializeField] MMF_Player explosionFeedbacks;
    private bool isPooled;
    private float elapsedTimeAsFollower;
    private float totalMoved;
    private void Awake()
    {
        scaleAnimationer = GetComponent<ScaleAnimationer>();
    }

    private void Start()
    {
        transform.position = new Vector3(transform.position.x, positionY, transform.position.z);
    }

    private void Update()
    {
        if (isFollower && FollowTo)
        {
            Vector3 positionBefore = transform.position;
            Vector3 targetPos = new Vector3(FollowTo.position.x, positionY, FollowTo.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPos, followSensivity * Time.deltaTime);
            totalMoved = Vector3.Distance(transform.position, positionBefore);

            elapsedTimeAsFollower += Time.deltaTime;
            if (elapsedTimeAsFollower > followDuration || (alsoUseFollowRange && totalMoved > followRange))
            {
                isFollower = false;
                Explode();
            }
        }
    }
    public override void Shoot(Vector3 point)
    {
        if (randomizeXZ)
        {
            float xRandom = Random.Range(-xRandomDiff, xRandomDiff);
            float zRandom = Random.Range(-zRandomDiff, zRandomDiff);
            Vector3 diffRandom = new(xRandom, 0, zRandom);
            transform.position = point + diffRandom;
        }       
    }

    public override void SetTarget(Transform target)
    {
        base.SetTarget(target);
        FollowTo = target;
        isFollower = true;
        elapsedTimeAsFollower = 0f;
        totalMoved = 0f;
    }
    public override void SetRange(float range)
    {
        followRange = range;
    }

    public override void SetDamage(float damage)
    {
        this.damage = (int)damage;
    }


    public void Use()
    {
        if (!isFollower)
        {
            Explode();
        }
    }

    private void Explode()
    {
        StartCoroutine(ExplosionRoutine());
    }

    private IEnumerator ExplosionRoutine()
    {
        yield return new WaitForSeconds(waitBeforeFirstExplosion);

        for (int i = 0; i < explosionCount; ++i)
        {
            CheckArea();
            yield return new WaitForSeconds(waitBetweenEveryExplosion);
        }
        EndLife();
    }

    private void CheckArea()
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, radius, playerLayer);
        foreach (var coll in colls)
        {
            if (coll.TryGetComponent(out PlayerHealth playerHealth))
            {
                playerHealth.TakeDamage(damage);
                OnPlayerContact?.Invoke();
            }
        }

        if (explosionFeedbacks != null) { explosionFeedbacks.PlayFeedbacks(); }
    }

    public void SetDamage(int damage) => this.damage = damage;
    public float GetRadius() => this.radius;
    public void SetRadius(float radius) => this.radius = radius;
 
    public void Initialize()
    {
        isPooled = true;
        Deactivate();
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        if(scaleAnimationer != null) { scaleAnimationer.Perform(); }
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    private void EndLife()
    {
        if (isPooled) { Deactivate(); }
        else Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, radius);
    }

}
