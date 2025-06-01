using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(EnemyHealth))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyMover))]
public abstract class Boss : MonoBehaviour
{
    [SerializeField] bool OpenBossCam;
    public BossPhase CurrentPhase {  get; private set; }

    [SerializeField] protected List<BossPhase> bossPhases;
    public Transform TargetedPos;
    public Animator Animator;
    [HideInInspector] public NavMeshAgent agent {  get; protected set; }
    [HideInInspector] public EnemyMover mover {  get; protected set; }
    protected EnemyHealth enemyHealth;

    [HideInInspector] public Transform player;
    protected float validAttackTime;
    protected float timeElapsedInPhase;

    [HideInInspector] public bool overrideMovement;
    public bool isAttacking;
    protected bool isMoving;
    public bool isTargetable = true;
    protected bool canChase = true;
    public float attackFinishTime;
    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        mover = GetComponent<EnemyMover>();
        enemyHealth = GetComponent<EnemyHealth>();
    }

    protected virtual void Start()
    {
        player = PlayerController.Instance.transform;
        enemyHealth.OnHealthCurrencyChange += TryChangeByHealthCurrency;

        ChangePhase(bossPhases[0]);
        StartPatrol();
        if (OpenBossCam) CameraController.Instance.OpenBossCamera(this);
    }

    protected virtual void Update()
    {
        Move();
        TryChangeByDuration();
        if (CanAttack) { Attack(); }
        if (CurrentPhase.lookAlwaysPlayer) { LookPlayer(CurrentPhase.lookSensivity); }
    }

    #region PhaseTransition
    private void TryChangeByDuration()
    {
        if (!CurrentPhase.ChangePermitted) return;
        if (CurrentPhase != null && CurrentPhase.PhaseChangeMethod == PhaseChangeMethod.Duration)
        {
            timeElapsedInPhase += Time.deltaTime;
            if (timeElapsedInPhase > CurrentPhase.ChangeValue)
            {
                ChangeToNextPhase();
            }
        }
    }

    private void TryChangeByHealthCurrency(int currency)
    {
        if (!CurrentPhase.ChangePermitted) return;
        if(CurrentPhase != null && CurrentPhase.PhaseChangeMethod == PhaseChangeMethod.HealthCurrency)
        {
            if(currency < CurrentPhase.ChangeValue)
            {
                ChangeToNextPhase();
            }
        }
    }  

    protected void ChangePhase(BossPhase newPhase)
    {
        if(CurrentPhase != null)
        {
            CurrentPhase?.ExitPhase();
            if (CurrentPhase.ActiveAttackBehaviour)
            {
                Destroy(CurrentPhase.ActiveAttackBehaviour.gameObject);
            }
        }       
        CurrentPhase = newPhase;
        CurrentPhase.EnterPhase(this);
        timeElapsedInPhase = 0f;

        HKDebugger.LogWorldText($"Boss phase changed {CurrentPhase.PhaseName}", transform.position + Vector3.up * 10f);
    }

    private void ChangeToNextPhase()
    {
        int currentPhaseIndex = bossPhases.IndexOf(CurrentPhase);
        if (!CurrentPhase.ChangeRandomly)
        {            
            if (currentPhaseIndex == bossPhases.Count - 1) return;
            else currentPhaseIndex++;
        }
        else { currentPhaseIndex = UnityEngine.Random.Range(0, bossPhases.Count); }

        BossPhase nextPhase = bossPhases[currentPhaseIndex];
        ChangePhase(nextPhase);        
    }

    #endregion
    private void Move()
    {
        if (!isAttacking && !overrideMovement)
        {
            if(CurrentPhase.PhaseMovementType == MovementType.Chase)
            {
                ChasePlayer();
            }           
        }        
    }

    protected void ChasePlayer()
    {
        if(!canChase) return;

        
        if(DistanceBetweenPlayer > CurrentPhase.stopDistanceOnChase)
        {
            mover.CartesianLookerMove(player.position , CurrentPhase.MovementSpeed);
            isMoving = true;
        }
        else 
        { 
            if(isMoving)
            {
                Stop();
            }
        }

        if (CanAttack && isMoving)
        {
            Stop();
        }
    }

    public void Stop()
    {
        isMoving = false;
        mover.StopImmediately();
    }
    
    protected virtual void Attack()
    {       
        isAttacking = true;            
        CurrentPhase.Attack();
        mover.StopImmediately();
    }

    public void FinishAttack()
    {
        isAttacking = false;
        EnterCooldown();        
        StartPatrol();
    }

    private void EnterCooldown()
    {
        validAttackTime = Time.time + CurrentPhase.AttackCooldown;
    }

    private void StartPatrol()
    {
        if (CurrentPhase.patrolAfterAttack && !overrideMovement)
        {
            mover.StartPatrolling(CurrentPhase.AttackCooldown - 1f);
        }
    }

    protected float DistanceBetweenPlayer => Vector3.Distance(PlayerController.Instance.transform.position, transform.position);
    protected bool PlayerInSight => DistanceBetweenPlayer < CurrentPhase.Range;
    protected bool CanAttack => PlayerInSight && !isAttacking && Time.time > validAttackTime;
    public Vector3 DirToPlayer => PlayerController.Instance.transform.position - transform.position;
    public void LookPlayer(float rotationSpeed = 10f)
    {
        Vector3 diff = DirToPlayer;
        diff.y = 0f;
        transform.rotation = Quaternion.Slerp(transform.rotation , Quaternion.LookRotation(diff) , rotationSpeed * Time.deltaTime);
    }

    public void LookPlayerInstantly()
    {
        Vector3 diff = DirToPlayer;
        diff.y = 0f;
        transform.forward = diff;
    }

    public void LookDestination(Vector3 destination , float rotationSpeed = 10f)
    {
        Vector3 diffToDestination = destination - transform.position;
        diffToDestination.y = 0f;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(diffToDestination), rotationSpeed * Time.deltaTime);
    }
}


[Serializable]
public class BossPhase
{
    [Header("IBossAttack")]
    public BossAttackBehaviour AttackBehaviour;
    [HideInInspector] public BossAttackBehaviour ActiveAttackBehaviour { get; private set; } // To create an instance of scriptable object
    private Boss boss;
    [Header("Properties")]
    public string PhaseName;
    public Tier Tier;

    [Header("Phase Movement")]
    public MovementType PhaseMovementType;
    public bool lookAlwaysPlayer = true;
    public float lookSensivity;
    [Range(0.1f, 5f)] public float animatorSpeedOnMovement;
    public float MovementSpeed;
    public float stopDistanceOnChase = 2f;  

    [Header("Phase Combat")]    
    public float AttackCooldown;
    public float Range;
    public bool patrolAfterAttack = true;

    [Header("Auto Phase Change")]
    public bool ChangePermitted = true;
    public PhaseChangeMethod PhaseChangeMethod;
    public int ChangeValue;
    public bool ChangeRandomly;
    [Header("Events")]
    public UnityEvent OnPhaseEnter;
    public UnityEvent OnPhaseExit;
    
    public void Attack()
    {
       ActiveAttackBehaviour.Attack(); 
    }

    public void EnterPhase(Boss boss)
    {
        this.boss = boss;       
        ActiveAttackBehaviour = UnityEngine.Object.Instantiate(AttackBehaviour , boss.transform);
        ActiveAttackBehaviour.SetBoss(boss);
        boss.isAttacking = false;
        OnPhaseEnter?.Invoke();
    }

    public void ExitPhase() => OnPhaseExit?.Invoke();

}

public enum Tier
{
    T1,
    T2,
    T3,
    T4,
    T5
}
public enum MovementType
{
    None,
    Chase,   
}

public enum AttackParameterType
{
    Bool,
    Trigger
}

public enum MovementParameterType
{
    Bool,
    Float
}

public enum PhaseChangeMethod
{
    None,
    HealthCurrency,
    Duration
}