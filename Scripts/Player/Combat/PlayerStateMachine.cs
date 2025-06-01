using System;
using System.Linq;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    public event Action<RollState> OnRoll;
    public Transform mainCamera{get; private set;}
    public CharacterController characterController {  get; private set;}
    [field: SerializeField] public GameObject selectedCharacter { get; set;}
    [field: SerializeField] public PlayerRangeIndicator rangeIndicatorPrefab;
    PlayerRangeIndicator indicator;
    [field: SerializeField] public Animator animator { get; private set; }
    [field: SerializeField] public float maxSpeed {  get; private set; }
    [field: SerializeField] public float rotationSensivity {  get; private set; }
    [field: SerializeField] public float acceleration { get; private set; }
    [field: SerializeField] public float RollStartSpeed { get; set; }
    [field: SerializeField] public float RollDuration { get; set; }
    [field: SerializeField] public float RollEndSpeed { get; set; }
    [field: SerializeField] public float RollCooldown { get; set; }

    [HideInInspector] public bool isRolling = false;
    [HideInInspector] public bool isAttacking = false;
    [HideInInspector] public bool inputClosed = false;

    private float validRollTime;
    public bool CanAttack = true;

    [SerializeField] float attackSpeed;
    [SerializeField] private float normalAttackRange;
    public Vector3 movementVector { get; private set; }

    [SerializeField] LayerMask enemyLayer;
    [SerializeField] LayerMask groundLayer;

    private State currentState;
    private readonly string SHOOT_ANIM_NAME = "Shoot";

    private void OnEnable()
    {
        InputReader.RollAction += Roll;
    }

    private void OnDestroy()
    {
        InputReader.RollAction -= Roll;
    }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();        
    }
    void Start()
    {
        mainCamera = Camera.main.transform;        
        animator = selectedCharacter?.GetComponent<Animator>();

        GetIntoNewState(new IdleState(this));

        indicator = Instantiate(rangeIndicatorPrefab , transform.position + Vector3.up * 0.05f ,Quaternion.Euler(90f,0f,0f));
    }

    
    void Update()
    {
        currentState.Tick(Time.deltaTime);

        indicator.SetRange(normalAttackRange);

        ReadMovementInput();
        ControlAttackAnimSpeed();
    }

    private void ReadMovementInput()
    {
        if (!inputClosed) 
        {            
            movementVector = new Vector3(InputReader.Instance.GetMovementVector().x, 0f, InputReader.Instance.GetMovementVector().y);
        }
    }
    public void Roll()
    {
        if (Time.time > validRollTime && !isRolling && !inputClosed)
        {
            RollState rollState = new(this);
            OnRoll?.Invoke(rollState);
            GetIntoNewState(rollState);

            validRollTime = Time.time + RollCooldown;
            InGameUI.Instance.EnterRollCooldown(RollCooldown);
        }
    }

    private void ControlAttackAnimSpeed()
    {
        AnimatorStateInfo stateInfo =animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName(SHOOT_ANIM_NAME))
        {
            animator.speed = attackSpeed;
        }
        else
        {
            animator.speed = 1f;
        }

    }

    public void GetIntoNewState(State newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public EnemyHealth GetClosestEnemy()
    {
        EnemyHealth closestEnemy = null;
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        var filteredEnemies = enemies.Where(enemy => Vector3.Distance(enemy.transform.position , transform.position) < normalAttackRange).ToList();
        float closestDistance = 10000f;
        for (int i = 0; i < filteredEnemies.Count; i++)
        {
            float currentDistance = Vector3.Distance(filteredEnemies[i].transform.position, transform.position);
            if (currentDistance < closestDistance)
            {
                closestDistance = currentDistance;
                closestEnemy = filteredEnemies[i].GetComponent<EnemyHealth>();  
            }
        }
        return closestEnemy;
    }

    public Vector3 GetCameraRelativeMovementVector()
    {
        float x = InputReader.Instance.GetMovementVector().x;
        float z = InputReader.Instance.GetMovementVector().y;

        Vector3 forward = mainCamera.transform.forward;
        Vector3 right = mainCamera.transform.right;

        forward.y = 0f;
        right.y = 0f;

        Vector3 forwardRelative = z * forward;
        Vector3 rightRelative = x * right;
        Vector3 cameraRelative = forwardRelative + rightRelative;
        return cameraRelative;
    }
    public void SetSelectedCharacter(GameObject character)
    {
        animator = character.GetComponent<Animator>();
    }

    public bool IsStopped() => movementVector.magnitude == 0f;

    public void SetAttackSpeed(float attackSpeed)
    {
        this.attackSpeed = attackSpeed;
    }
    public void SetRange(float range)
    {
        normalAttackRange = range;
    }

    public void InfluenceAttackSpeed(float influence)
    {
        attackSpeed += influence;
    }

    public void InfluenceRange(float influence)
    {
        this.normalAttackRange += influence;
    }

    public void InfluenceMovementSpeed(float increase)
    {
        maxSpeed += increase;
    }

    public float GetAttackSpeed => this.attackSpeed;

    public float GetRange => this.normalAttackRange;

    public float GetMovementSpeed => this.maxSpeed;

    public PlayerRangeIndicator GetRangeIndicator() => indicator;
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, normalAttackRange);
    }
}
