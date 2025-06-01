using UnityEngine;

public class HomingMissile : EnemyBallistic , IPooledObject
{
    public bool ShootToPlayerAtStart;
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private bool randomizeCatch;
    [SerializeField] private float catchSensMin;
    [SerializeField] private float catchSensMax;
    [SerializeField] private float catchSensivity;
    [SerializeField] private float minDistanceToFollow =3f;

    [Header("Life End")]
    [SerializeField] private BallisticLifetimeEndType lifetimeEndType;
    [SerializeField] private float lifeTime = 10f;
    [SerializeField] private float range = 15f;
    [SerializeField] int damage = 5;
    private float totalMoved;
    private bool pooled;
    Vector3 direction;

    private void OnEnable()
    {
        GameStateManager.OnLevelFinished += OnLevelFinish;
    }

    private void OnDisable()
    {
        GameStateManager.OnLevelFinished -= OnLevelFinish;
    }
    private void OnLevelFinish(int obj)
    {
        EndLife();
    }

    void Start() // TEST
    {
        if (lifetimeEndType == BallisticLifetimeEndType.Duration)
        {
            Invoke(nameof(EndLife), lifeTime);
        }
        if(ShootToPlayerAtStart)
        {
            SetTarget(PlayerController.Instance.transform);
            Shoot(transform.forward);
        }
    }

    void Update()
    {
        if(target == null) { return; }
        Vector3 diff = target.position - transform.position;
        diff.y = 0f;
        if(diff.magnitude > minDistanceToFollow)
        {
            direction = Vector3.Lerp(direction, diff, catchSensivity * Time.deltaTime);
        }        
        transform.position += movementSpeed * Time.deltaTime * direction.normalized;
        CheckRange();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerHealth playerHealth))
        {
            playerHealth.TakeDamage(damage);
        }
    }

    private void CheckRange()
    {
        if(lifetimeEndType == BallisticLifetimeEndType.Range)
        {
            totalMoved += Time.deltaTime * movementSpeed;
            if (totalMoved > range)
            {
                EndLife();
            }
        }        
    }

    public override void Shoot(Vector3 pointOrDirection)
    {
        direction = pointOrDirection;
        transform.forward = direction;

        if (randomizeCatch)
        {
            catchSensivity = Random.Range(catchSensMin, catchSensMax);
        }
    }

    public override void SetRange(float range)
    {
        this.range = range;
    }

    public override void SetDamage(float damage)
    {
        this.damage = (int)damage;
    }

    public override void SetTarget(Transform target)
    {
        base.SetTarget(target);
    }

    public void Initialize()
    {
        pooled = true;
        Deactivate();
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        if(lifetimeEndType == BallisticLifetimeEndType.Duration)
        {
            Invoke(nameof(EndLife), lifeTime);
        }
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    private void EndLife()
    {
        if (pooled) Deactivate();
        else Destroy(gameObject);
    }
    
}
public enum BallisticLifetimeEndType
{
    Duration,
    Range,
    Infinite
}