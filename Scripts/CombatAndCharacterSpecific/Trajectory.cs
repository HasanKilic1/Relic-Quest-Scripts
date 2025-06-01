using System;
using UnityEngine;

public class Trajectory : EnemyBallistic , IPooledObject
{
    public event Action OnExplode;
    [SerializeField] private float reachTime = 2f;
    [SerializeField] private float speed;
    [SerializeField] private float radius;
    [SerializeField] private float damage;
    [SerializeField] private float heightMultiplier;
    [SerializeField] private AnimationCurve Y_Curve;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] private float approachSensivity = 0.5f;
    [SerializeField] GameObject explosionVfx;
    [SerializeField] GameObject damageZonePrefab;
    GameObject damageZone;
    private float startTime;
    private float journeyLength;
    Vector3 startPoint;
    Vector3 targetPoint;
    private bool isStopped = false;
    private bool pooled = false;

    void Update()
    {
        Move();
    }

    private void Move()
    {
        if (targetPoint != null && !isStopped)
        {
            // based on speed
           // float distCovered = (Time.time - startTime) * speed;
           // float fractionOfJourney = distCovered / journeyLength;
           
            // based on Time
            float fractionOfJourney = (Time.time - startTime) / reachTime;
            Vector3 currentPosition = Vector3.Lerp(startPoint, targetPoint, fractionOfJourney);
            float height = Y_Curve.Evaluate(fractionOfJourney) * heightMultiplier;

            transform.position = new Vector3(currentPosition.x, currentPosition.y + height, currentPosition.z);

            if (Vector3.Distance(transform.position, targetPoint) < approachSensivity)
            {
                HandleReach();
            }
        }
    }

    private void HandleReach()
    {
        isStopped = true;
        if(explosionVfx != null)
        {
            Instantiate(explosionVfx, GridManager.Instance.GetClosestGridOnLocation(transform.position).Position, explosionVfx.transform.rotation);
        }
        Explode();
    }

    private void Explode()
    {
        Collider[] results = Physics.OverlapSphere(transform.position, radius, playerLayer);
        foreach (Collider col in results)
        {
            if (col.TryGetComponent(out PlayerHealth playerHealth)) playerHealth.TakeDamage((int)damage);
        }
        OnExplode?.Invoke();
        EndLife();
    }

    public override void SetDamage(float damage)
    {
        this.damage = (int)damage;
    }

    public override void SetRange(float range)
    {}

    public override void Shoot(Vector3 point)
    {
        isStopped = false;
        startPoint = transform.position;
        targetPoint = GridManager.Instance.GetClosestGridOnLocation(point).Position;
        journeyLength = Vector3.Distance(startPoint, targetPoint);
        startTime = Time.time;
        if(damageZonePrefab != null)
        {
            damageZone = Instantiate(damageZonePrefab, targetPoint + Vector3.up * 0.5f, damageZonePrefab.transform.rotation);
        }
    }

    public void Initialize()
    {
        pooled = true;
        Deactivate();
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    private void EndLife()
    {
        if(damageZone != null) { Destroy(damageZone); }
        if (pooled) { Deactivate(); }
        else Destroy(gameObject);
    }
}
