using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LizardEnemy : Enemy
{
    [SerializeField] Transform sword;
    private int run_hash = Animator.StringToHash("Run");
    private int attack1_hash = Animator.StringToHash("Attack1");
    private int attack2_hash = Animator.StringToHash("Attack2");
    float runValue = 0f;
    float stdMovementSpeed;
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
        stdMovementSpeed = movementSpeed;
    }

    
    protected override void Update()
    {
        base.Update();        
        HandleStates();
        RunToPlayer();
    }

    private void RunToPlayer()
    {
        if (currentState == EnemyState.Chase)
        {
            if (checkRun)
            {
                runValue += Time.deltaTime * 10f;
                animator.SetFloat(run_hash, runValue);
                movementSpeed = 9f;
            }
            else
            {
                movementSpeed = stdMovementSpeed;
                runValue -= Time.deltaTime * 3f;
                animator.SetFloat(run_hash, runValue);
            }
            runValue = Mathf.Clamp01(runValue);
        }

        else if (currentState == EnemyState.PositionChange)
        {
            animator.SetFloat(run_hash, 0f);
            movementSpeed = stdMovementSpeed;
        }
    }

    protected override void Attack()
    {
        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        Stop();
        PlayRandomAttackAnim();
        animator.SetFloat(run_hash, 0f);

        yield return new WaitForSeconds(timeBeforeAttack);


        TryGiveDamageToPlayer();
        ResetAttackTimer();
        HandleAttackEnd();
    }
    private void PlayRandomAttackAnim()
    {
        if(Random.Range(-1f , 1f) < 0f)
        {
            animator.SetTrigger(attack1_hash);
        }
        else 
        { 
            animator.SetTrigger(attack2_hash);
        }
    }
    private void TryGiveDamageToPlayer()
    {
        if (Vector3.Angle(transform.forward, PlayerHealth.Instance.transform.position - transform.position) > 45) return;

        Collider[] colls = Physics.OverlapSphere(transform.position, attackRange);
        foreach (Collider coll in colls)
        {
            if(coll.TryGetComponent(out PlayerHealth playerHealth))
            {
                playerHealth.TakeDamage((int)damage);
            }
        }
    }
    private bool checkRun => distanceToPlayer > attackRange * 1.7f;
}
