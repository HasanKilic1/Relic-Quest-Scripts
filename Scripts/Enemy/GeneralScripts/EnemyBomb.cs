using MoreMountains.Feedbacks;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class EnemyBomb : EnemyBallistic, IPooledObject
{
    public UnityEvent OnPlayerContact;
    [SerializeField] private float speed;
    [SerializeField] private float heightMultiplier;
    [SerializeField] private AnimationCurve Y_Curve;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] private int innerDamage = 25;
    [SerializeField] private int outerDamage = 10;
    [SerializeField] private float radiusInner = 2f;
    [SerializeField] private float radiusOuter = 6.5f;
    [SerializeField] private float explosionWaitDuration = 3f;
    [SerializeField] private float approachSensivity = 0.5f;
    [SerializeField] MMF_Player reachFeedbacks;
    [SerializeField] MMF_Player explosionFeedbacks;
    [SerializeField] GameObject objectToLeave;
    [SerializeField] Transform[] resetScalesAtActivation;

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

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Floor>() != null)
        {
            isStopped = true;
            HandleReach();
        }
    }

    public override void Shoot(Vector3 point)
    {
        isStopped = false;
        startPoint = transform.position;
        targetPoint = point;
        journeyLength = Vector3.Distance(startPoint, targetPoint);
        startTime = Time.time;

    }

    private void Move()
    {
        if (targetPoint != null && !isStopped)
        {
            float distCovered = (Time.time - startTime) * speed;
            float fractionOfJourney = distCovered / journeyLength;

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
        StartCoroutine(WaitUntilExplode());
    }

    private IEnumerator WaitUntilExplode()
    {
        reachFeedbacks?.PlayFeedbacks();

        yield return new WaitForSeconds(explosionWaitDuration);
        
        if(explosionFeedbacks != null) { explosionFeedbacks.PlayFeedbacks(); }
        CheckDamage();
        if(objectToLeave)
        {
            Instantiate(objectToLeave, transform.position, Quaternion.identity);
        }
    }

    private void CheckDamage()
    {
        Collider[] results = new Collider[10];
        Physics.OverlapSphereNonAlloc(transform.position, radiusInner, results , playerLayer);

        if(results.Length > 0)
        {
            foreach (Collider collider in results)
            {
                if (collider != null && collider.TryGetComponent(out PlayerHealth playerHealth))
                {                    
                    playerHealth.TakeDamage(innerDamage);                    
                    EndLife();
                    return;
                }
            }

            Physics.OverlapSphereNonAlloc(transform.position, radiusOuter, results, playerLayer);
            foreach (Collider collider in results)
            {
                if (collider != null && collider.TryGetComponent(out PlayerHealth playerHealth))
                {
                    playerHealth.TakeDamage(outerDamage);
                    OnPlayerContact?.Invoke();
                }
            }
        }                
       
        EndLife();
    }

    public override void SetDamage(float damage)
    {
        this.outerDamage = (int)damage;
    }

    public override void SetRange(float range)
    { }

   
    public void Initialize()
    {
        pooled = true;
        Deactivate();
    }

    public void Activate()
    {
        foreach(Transform t in resetScalesAtActivation)
        {
            t.gameObject.transform.localScale = Vector3.zero;
        }
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    private void EndLife()
    {
        if (pooled) { Deactivate(); }
        else Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radiusInner);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radiusOuter);
    }
}