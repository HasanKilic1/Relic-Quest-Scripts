using System.Collections;
using UnityEngine;

public class NagarWizard : Enemy
{
    [SerializeField] EnemyBallistic ballistic;
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
        yield return new WaitForSeconds(timeBeforeAttack);
        ResetAttackTimer();
        HandleAttackEnd();
    }
}
