using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyVisualizer))]
[RequireComponent(typeof(EnemyHealth))]
[RequireComponent (typeof(ResourceDropper))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(MMScaleShaker))]
[RequireComponent(typeof(MMPositionShaker))]
[RequireComponent(typeof(MMRotationShaker))]
public abstract class Enemy : MonoBehaviour
{      
    protected EnemyState currentState;
    [SerializeField] public Animator animator;
    
    public bool isTargetable = true;
    [SerializeField] protected float timeBeforeAttack;
    protected NavMeshAgent agent;

    public Transform targetedPosition;    
    protected Transform playerTargetedPosition;
    [Header("Movement")]
    [SerializeField] public bool moveable = true;
    [SerializeField] public float movementSpeed;
    [SerializeField] protected float rotationSpeed;

    [Header("Combat")]
    [SerializeField] protected float damage;
    [SerializeField] protected float attackRange;
    [SerializeField] protected float attackInterval;
    protected bool isChasing;
    protected bool isAttacking;

    [Header("Chase")]   
    [Tooltip("Enable this if enemy is always chase the player")]
    [SerializeField] protected bool canAlwaysChasePlayer = false;
    [SerializeField] private float locationChangeTime = 4f;
    [SerializeField] private bool differentiateSpeedOnLocationChange = false;
    [SerializeField] private float locationChangeSpeed = 6f;
    [SerializeField] private float locationChangeApproachSensivity = 1f;
    private float locationChangeTimer = 0f;  
    Vector3 destinationPos;
    protected float attackTimer = 0f;
    protected bool canFaceAnyTarget = true;
    protected bool isFreezed = false;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = movementSpeed;
    }
    protected virtual void Start()
    {
        playerTargetedPosition = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>().TargetedPosition;
    }

    protected virtual void Update()
    {
        attackTimer -= Time.deltaTime;
    }  

    
    protected void HandleStates()
    {
        switch (currentState)
        {
            case EnemyState.PositionChange:
                ChangePosition();
                break;

            case EnemyState.Chase:
                ChasePlayer();
                break;
        }     
    }

    protected void ChasePlayer()
    {
        isChasing = true;
        if (distanceToPlayer <= attackRange)
        {
            Stop();
            FaceToTarget(playerTargetedPosition.position);
            if(canAttack)
            {
                EnterAttackState();
            }
            return;
        }
        
        Move(EnemyState.Chase);
        Vector3 destination = new Vector3(playerTargetedPosition.position.x, transform.position.y, playerTargetedPosition.position.z);
        agent.SetDestination(destination);
        FaceToTarget(destination);
    }

    private void EnterAttackState()
    {       
        isChasing = false;
        isAttacking = true;
        currentState = EnemyState.Attack;
        Attack();
        FaceToTarget(playerTargetedPosition.position);
    }

    protected void ResetAttackTimer()
    {
        attackTimer = attackInterval;
    }
    protected void HandleAttackEnd() //call this after ensuring attack state is over
    {
        isAttacking = false;        

        if(!canAlwaysChasePlayer) 
        { 
            currentState = EnemyState.PositionChange;
        } 
        else
        {
            currentState = EnemyState.Chase;
        }
    }
    private void ChangePosition()
    {
        if(!moveable) { return; }
        if(locationChangeTimer == 0f)
        {
            AssignNewRandomValidPosition();
        }
        if (locationChangeTimer > locationChangeTime) 
        {
            currentState = EnemyState.Chase;
            locationChangeTimer = 0f;
            return; 
        }

        isChasing = false;
        locationChangeTimer += Time.deltaTime;
        Move(EnemyState.PositionChange);
        agent.SetDestination(destinationPos);

        if (Vector3.Distance(destinationPos, transform.position) < locationChangeApproachSensivity)
        {
            currentState = EnemyState.Chase;
        }
    }
    public void GetIntoNewState(EnemyState enemyState)
    {
        currentState = enemyState;
    }

    private void AssignNewRandomValidPosition()
    {
        float searchDistance = 50f;
        destinationPos = NavMeshManager.Instance.GetRandomPositionWithinRadius(transform.position, searchDistance);
    }

   
    public void Stop()
    {
        agent.isStopped = true;
        agent.speed = 0f;
        animator.SetBool("isMoving", false);
    }
    protected void Move(EnemyState state)
    {
        if (!moveable) return;
        agent.isStopped = false;
        if(state == EnemyState.Chase)
        {
            agent.speed = movementSpeed;
        }
        if(state == EnemyState.PositionChange)
        {
            if(differentiateSpeedOnLocationChange)
            {
                agent.speed = locationChangeSpeed;
            }
            else agent.speed = movementSpeed;
        }
        animator.SetBool("isMoving", true);
    }


    public void FaceToTarget(Vector3 targetPosition , bool lookInstantly = false)
    {
        if (!canFaceAnyTarget) return;

        Vector3 diff = targetPosition - transform.position;
        diff.y = 0f;
        Quaternion lookRot = Quaternion.LookRotation(diff.normalized);
        if(lookInstantly)
        {
            transform.rotation = lookRot;
        }
        else transform.rotation = Quaternion.Slerp(transform.rotation , lookRot, Time.deltaTime * rotationSpeed);
    }
    protected Vector3 diffToPlayer => playerTargetedPosition.position - transform.position;
    protected float distanceToPlayer => Vector3.Distance(transform.position, playerTargetedPosition.position);
    protected bool canAttack => attackTimer <= 0f && !isAttacking;
    protected bool canAttackToPlayer => canAttack && playerInSight;
    protected bool playerInSight => distanceToPlayer < attackRange;

   
    protected abstract void Attack();

}
public enum EnemyState
{
    //Chase -> Attack -> Change Position -> Chase -> Attack
    Chase,
    Attack,
    PositionChange,
}