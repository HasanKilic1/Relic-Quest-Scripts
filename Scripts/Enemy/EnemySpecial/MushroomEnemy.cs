using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomEnemy : Enemy
{
    
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        if (isFreezed) { return; }
        HandleStates();       
    }

    protected override void Attack()
    {
        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        Stop();
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.3f);
        if (canGiveDamageToPlayer)
        {
            PlayerHealth.Instance.TakeDamage((int)damage);
        }
        yield return new WaitForSeconds(timeBeforeAttack - 0.3f);
        ResetAttackTimer();
        HandleAttackEnd();
    }

    private bool canGiveDamageToPlayer => Vector3.Angle(transform.forward, diffToPlayer) < 45f && playerInSight;

   

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

       
    }
}

