using MoreMountains.Feedbacks;
using UnityEngine;

public class ExplosiveObject : MonoBehaviour , IGridObject
{
    private PathMovement pathMovement;
    private WorldGrid grid;
    [SerializeField] Vector3 positionOffSet;
    [SerializeField] MMF_Player initializeFeedbacks;
    public bool GiveDamageToEnemy = true;
    public bool GiveDamageToPlayer = true;    

    [Header("Explosion Trigger")]
    public bool TriggerByPlayer = true;
    public bool TriggerByEnemy = false;    
    [SerializeField] float waitDurationAfterTrigger = 2f;
    [SerializeField] MMF_Player triggerFeedbacks;

    [Header("Explosion")]
    [SerializeField] int damageToPlayer;
    [SerializeField] int damageToEnemy;
    [SerializeField] float radius;
    [SerializeField] MMF_Player explosionFeedbacks;
    [SerializeField] float destroyDuration;
    [SerializeField] GameObject leaveObject;
    private bool isTriggered = false;
    private bool isExploded = false;
    [HideInInspector] public bool MotionFinished = false;
    float timeToExplode;
    private void Awake()
    {
        pathMovement = GetComponent<PathMovement>();
        pathMovement.OnFinish.AddListener(OnMotionFinished);
    }

    private void OnMotionFinished()
    {
        MotionFinished = true;
    }

    private void Start()
    {
        initializeFeedbacks?.PlayFeedbacks();
    }

    private void Update()
    {
        if(isTriggered && !isExploded && Time.time > timeToExplode)
        {
            Explode();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isTriggered && MotionFinished)
        {
            if (TriggerByPlayer && other.GetComponent<PlayerHealth>())
            {
                Trigger();
            }

            if (TriggerByEnemy && other.GetComponent<EnemyHealth>())
            {
                Trigger();
            }            
        }
        
    }

    private void Trigger()
    {
        isTriggered = true;
        timeToExplode = Time.time + waitDurationAfterTrigger;
        triggerFeedbacks?.PlayFeedbacks();        
    }

    private void Explode()
    {
        isExploded = true;
        explosionFeedbacks?.PlayFeedbacks();
        CheckDamageables();
        Invoke(nameof(Disable), destroyDuration);
        if(leaveObject != null) { Instantiate(leaveObject, transform.position, leaveObject.transform.rotation);}
    }

    private void CheckDamageables()
    {
        Collider[] damageables = Physics.OverlapSphere(transform.position, radius);

        foreach (var damageable in damageables)
        {
            if(GiveDamageToEnemy && damageable.TryGetComponent(out EnemyHealth enemyHealth))
            {
                enemyHealth.TakeDamage(damageToEnemy, Vector3.zero, isUnstoppableAttack: true);
            }
            if (GiveDamageToPlayer && damageable.TryGetComponent(out PlayerHealth playerHealth))
            {
                playerHealth.TakeDamage(damageToPlayer);
            }
        }
    }
   
    public void SetPosition(Vector3 position)
    {
        pathMovement.SetPathPosition(position + positionOffSet, 0);
        pathMovement.SetPathPosition(position, pathMovement.path.Length - 1);
        pathMovement.StartFollow(transform);
    }
    public void SetGrid(WorldGrid grid)
    {
        this.grid = grid;
    }

    public void Disable()
    {
        grid?.Clear();
        GridManager.Instance.RemoveObjectFromList(this);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, radius);
    }
    
}
