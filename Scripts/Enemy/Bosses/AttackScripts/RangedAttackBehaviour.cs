using System;
using System.Collections;
using UnityEngine;

public class RangedAttackBehaviour : BossAttackBehaviour
{
    public static event Action<Boss> OnShoot;
    [SerializeField] private string animTriggerName = "Ranged Attack";
    [SerializeField] private int damage;
    [SerializeField] private EnemyBallistic ballistic;
    [SerializeField] private float timeBeforeAttack;
    [SerializeField] private int shootLoop;
    [SerializeField] private float shootInterval;
    [SerializeField] private int ballisticCountEveryAttack;
    [SerializeField] private float timeBetweenBallistics;
    [SerializeField] private Vector3 shootPositionOffset;
    [SerializeField] private bool overrideBossMovement;
    [SerializeField] private ShootType shootType;
    [SerializeField] private BallisticTargetMode ballisticTargetMode;
    public bool FacePlayerWhileShooting = true;
    private bool isAttacking;

    private void OnValidate()
    {
        if (shootType == ShootType.Singular)
        {
            shootLoop = 1;
        }
    }
    private void Update()
    {
        if(FacePlayerWhileShooting && isAttacking)
        {
            boss.LookPlayer();
        }
    }
    public override void Attack()
    {
        if (isAttacking) { return; }
        if (useAnimation)
        {
            boss.Animator.SetTrigger(animTriggerName);
        }

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        yield return new WaitForSeconds(timeBeforeAttack);

        for (int i = 0; i < shootLoop; i++)
        {
            for (int j = 0; j < ballisticCountEveryAttack; j++)
            {
                yield return new WaitForSeconds(timeBetweenBallistics);
                ShootBallistic();

            }
            yield return new WaitForSeconds(shootInterval);
        }
        ResetSequence();
    }
    public override void SetBoss(Boss boss)
    {
        this.boss = boss;
        boss.overrideMovement = overrideBossMovement;
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    private void ShootBallistic()
    {
        Vector3 direction = ballisticTargetMode == BallisticTargetMode.Forward ? boss.transform.forward : boss.DirToPlayer;
        EnemyBallistic shootObj = Instantiate(ballistic, boss.transform.position + shootPositionOffset, Quaternion.identity);
        if (boss.TryGetComponent(out IShooterBoss shooterBoss))
        {
            shootObj.transform.position = shooterBoss.GetShootPos().position;
        }
        shootObj.SetDamage(damage);
        shootObj.SetRange(boss.CurrentPhase.Range);
        shootObj.Shoot(direction.normalized);

        OnShoot?.Invoke(boss);
    }

    public override void ResetSequence()
    {
        isAttacking = false;
        boss.FinishAttack();
    }
}
public enum BallisticTargetMode
{
    ToPlayer,
    Forward
}