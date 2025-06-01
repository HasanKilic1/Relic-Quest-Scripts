using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HelperSword : MonoBehaviour , IRelic
{
    PlayerStateMachine playerStateMachine;
    [SerializeField] GameObject sword;
    private enum SwordState
    {
        Follow,
        Combat,
    }
    private SwordState state;

    [Header("Combat")]
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] private float damage;
    [SerializeField] private float distanceSensivity;
    [SerializeField] private float attackDuration;
    [SerializeField] private float coolDown;

    [Header("Movement")]
    [SerializeField] private float movementSpeedOnFollow;
    [SerializeField] private float movementSpeedOnCombat;
    [SerializeField] private Vector3[] followOffsets;
    private Vector3 currentFollowOffset;
    [SerializeField] private float followOffsetChangeInterval;
    [SerializeField] private float attackPositionHeight;
   
    private Transform combatTarget;
    private bool isAttacking;

    private float timeToChangeFollowOffset;
    private readonly float enemyFilteringInterval = 1f;
    private float timeElapsedFiltering;
    private float attackTime;
    [Header("Feedbacks")]
    [SerializeField] MMF_Player combatStartFeedbacks;
    [SerializeField] MMF_Player damageFeedbacks;

    [Header("Test - Gizmo")]
    [SerializeField] bool drawSwordGizmo;
    [SerializeField] Transform player;

    private void Start()
    {
        SettleEffect(PlayerController.Instance.GetComponent<PlayerStateMachine>());//Test
        TryChangeFollowOffset();
        state = SwordState.Follow;
    }

    private void Update()
    {
        if(state == SwordState.Follow)
        {
            FollowPlayer();
        }
        else
        {
            CombatUpdate();
        }
        sword.transform.Rotate(360f * Time.deltaTime * Vector3.up);
    }

 
    public void SetDamage(float damage)
    {
        this.damage = damage;
    }

    public void SettleEffect(PlayerStateMachine playerStateMachine)
    {
        this.playerStateMachine = playerStateMachine;
    }

    public void ResetEffect(PlayerStateMachine anyPlayerScript)
    {
        
    }  

    private void FollowPlayer()
    {
        sword.transform.position = Vector3.Lerp(sword.transform.position, playerStateMachine.transform.position + currentFollowOffset, movementSpeedOnFollow * Time.deltaTime);
        TryChangeFollowOffset();
        if (AttackIsValid)
        {
            LookForEnemy(out bool enemyFound);
            if (enemyFound)
            {
                state = SwordState.Combat;
            }
        }       
    }

    private void TryChangeFollowOffset()
    {
        if (Time.time > timeToChangeFollowOffset)
        {
            currentFollowOffset = followOffsets[Random.Range(0, followOffsets.Length)];
            timeToChangeFollowOffset = Time.time + followOffsetChangeInterval;
        }
    }

    private void LookForEnemy(out bool enemyFound)
    {
        timeElapsedFiltering += Time.deltaTime;
        enemyFound = false;
        if(timeElapsedFiltering > enemyFilteringInterval)
        {
            timeElapsedFiltering = 0f;
            var possibleEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            if(possibleEnemies.Length > 0)
            {
                Transform farthestEnemy = possibleEnemies.OrderByDescending(enemy => Vector3.Distance(enemy.transform.position, playerStateMachine.transform.position)).First().transform;
                enemyFound = farthestEnemy != null;
                if (enemyFound)
                {
                    combatTarget = farthestEnemy;
                }
            }            
        }
    }

    private void CombatUpdate()
    {
        if(combatTarget == null)
        {
            state = SwordState.Follow;
            return;
        }
        if(!isAttacking && combatTarget != null)
        {
            Vector3 followPosition = combatTarget.position + Vector3.up * 10f;
            sword.transform.position = Vector3.Lerp(sword.transform.position, followPosition, movementSpeedOnCombat * Time.deltaTime);
            if(Vector3.Distance(sword.transform.position , followPosition) < distanceSensivity)
            {
                StartCoroutine(AttackRoutine());
            }
        }
        
    }

    private IEnumerator AttackRoutine()
    {
        combatStartFeedbacks?.PlayFeedbacks();

        bool succeed = true;
        isAttacking = true;
        float elapsed = 0f;

        while(elapsed < attackDuration)
        {
            elapsed += Time.deltaTime;
            if(combatTarget == null)
            {
                succeed = false;
                break;
            }
            Vector3 targetPos = new(combatTarget.position.x, attackPositionHeight , combatTarget.transform.position.z);
            sword.transform.position = Vector3.Lerp(sword.transform.position, targetPos, elapsed / attackDuration);
            yield return null;
        }
        if(succeed)
        {
            CheckDamage();
            damageFeedbacks?.PlayFeedbacks();
        }
        isAttacking = false;
        state = SwordState.Follow;
        attackTime = Time.time + coolDown;
    }

    private void CheckDamage()
    {
        Collider[] colls = Physics.OverlapSphere(sword.transform.position, 5f, enemyLayer);
        foreach (var coll in colls)
        {
            if(coll.TryGetComponent(out EnemyHealth enemyHealth))
            {
                enemyHealth.TakeDamage((int) damage , Vector3.zero , isUnstoppableAttack:true);
            }
        }
    }

    private bool AttackIsValid => Time.time > attackTime;
    private void OnDrawGizmosSelected()
    {
        if (player && drawSwordGizmo)
        {
            Gizmos.color = Color.blue;
            if(followOffsets.Length > 0)
            {
                foreach (var ofs in followOffsets)
                {
                    Gizmos.DrawMesh(sword.GetComponent<MeshFilter>().sharedMesh , player.position + ofs , Quaternion.Euler(0 , 0 , 180) , sword.transform.localScale);
                }
            }
        }
    }
}
