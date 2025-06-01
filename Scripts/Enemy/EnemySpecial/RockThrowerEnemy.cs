using System.Collections;
using UnityEngine;

public class RockThrowerEnemy : Enemy
{
    private bool alreadyAttacking = false;
    protected override void Awake()
    {
        base.Awake();        
    }
    protected override void Start()
    {
        base.Start();
        this.GetComponentInChildren<BulletHell>().OnShootingStarted += MakeAttackingTrue;
        this.GetComponentInChildren<BulletHell>().OnShootingFinished += MakeAttackingFalse;
    }
    protected override void Update()
    {
        base.Update();

        if(isFreezed) { return; }
        HandleStates();        
    }
    protected override void Attack()
    {
        if (alreadyAttacking) { return; }
        StartCoroutine(AttackRoutine());
    }
  
    private IEnumerator AttackRoutine()
    {
        Stop();
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(1);
        ResetAttackTimer();
        HandleAttackEnd();
    }
    private void MakeAttackingTrue()
    {
        alreadyAttacking = true;
    }

    private void MakeAttackingFalse()
    {
        alreadyAttacking = false;
    }

    public float Damage => this.damage;
    public float Range => this.attackRange;
}
