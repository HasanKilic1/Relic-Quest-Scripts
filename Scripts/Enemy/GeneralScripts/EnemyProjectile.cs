using MoreMountains.Feedbacks;
using UnityEngine;

public class EnemyProjectile : EnemyBallistic, IPooledObject
{
    [SerializeField] int damage;
    [SerializeField] float speed;
    [SerializeField] float range = 10f;
    Vector3 moveDirection = Vector3.zero;
    Vector3 maneuverDirection = Vector3.zero;
    float totalPathTaken = 0f;
    [SerializeField] bool provideEnvironmentContacts = true;
    [SerializeField] bool isManeuaverable;
    [SerializeField] bool Y_0;
    float maneuverTime = 0f;
    [SerializeField] bool accelerated = false;
    [SerializeField] float acceleration = 10f;
    [SerializeField] MMF_Player collisionFeedbacks;

    private bool isPooled = false;
    private float damageCooldown = 0.5f;
    private float validDamageTime;
    private void Awake()
    {
        this.GetComponent<Collider>().isTrigger = true;
    }

    void Update()
    {
        Vector3 posBeforeMovement = transform.position;
        Move();
        Vector3 posAfterMovement = transform.position;
        totalPathTaken = Vector3.Magnitude(posBeforeMovement - posAfterMovement);
        if (totalPathTaken > range)
        {
            EndLife();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerHealth playerHealth))
        {
            if (CanGiveDamage)
            {
                playerHealth.TakeDamage(damage);
                validDamageTime = Time.time + damageCooldown;
            }
        }
        if(!provideEnvironmentContacts) { return; }
        if (other.GetComponent<Floor>() || other.GetComponent<Obstacle>())
        {
            collisionFeedbacks?.PlayFeedbacks();           
            EndLife();
        }
    }

    private void Move()
    {
        maneuverTime += Time.deltaTime;
        if (isManeuaverable)
        {
            moveDirection = Vector3.Lerp(moveDirection, maneuverDirection, Time.deltaTime * 10f);
            if (maneuverTime > 0.3f)
            {
                float angle = UnityEngine.Random.Range(-1f, 1f) < 0 ? 45 : -45;
                maneuverTime = 0f;
                maneuverDirection = Quaternion.AngleAxis(angle, Vector3.up) * moveDirection;
            }
        }

        if (accelerated) { speed += acceleration * Time.deltaTime; }

        transform.position += speed * Time.deltaTime * moveDirection.normalized;
    }

    private bool CanGiveDamage => Time.time > validDamageTime;

    public override void SetDamage(float damage)
    {
        this.damage = (int)damage;
    }
    public override void Shoot(Vector3 moveDirection)
    {
        this.moveDirection = moveDirection;
        if (Y_0) this.moveDirection.y = 0f;
        this.maneuverDirection = moveDirection;
        transform.forward = moveDirection.normalized;
        Invoke(nameof(EndLife), 10f);
    }

    public override void SetRange(float range)
    {
        this.range = range;
    }

    public void SetManeuverable(bool maneuverable)
    {
        this.isManeuaverable = maneuverable;
    }

    public void Initialize()
    {
        isPooled = true;
        Deactivate();
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        totalPathTaken = 0f;
        gameObject.SetActive(false);
    }

    private void EndLife()
    {
        if (isPooled) { Deactivate(); }
        else Destroy(gameObject);
    }
}
