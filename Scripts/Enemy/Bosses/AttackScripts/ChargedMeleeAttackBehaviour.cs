using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargedMeleeAttackBehaviour : BossAttackBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private string animTriggerName = "Attack1";
    [SerializeField] private float attackDelay;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private GameObject attackVfx;
    [SerializeField] private Vector3 vfxOffset;

    [Header("Movement")]
    [SerializeField] private bool MOVE_BACKWARD;
    [SerializeField] private float backWardMoveDistance;
    [SerializeField] private float approachDurationToPlayer = 0.4f;
    [SerializeField] private AnimationCurve backwardMovementCurve;
    [SerializeField] private AnimationCurve approachMovementCurve;

    private bool damageGiven;

    public override void Attack()
    {
        if(useAnimation) boss.Animator.SetTrigger(animTriggerName);
        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        float elapsed1 = 0f;
        boss.overrideMovement = true;
        Vector3 targetPos = boss.transform.position - boss.transform.forward * backWardMoveDistance;
        Vector3 posBeforeMovement = boss.transform.position;

        while (elapsed1 < attackDelay)
        {
            if (MOVE_BACKWARD)
            {
                elapsed1 += Time.deltaTime;
                float t = elapsed1 / attackDelay;
                float evaluation = backwardMovementCurve.Evaluate(t);

                Vector3 newLocation = Vector3.Lerp(posBeforeMovement, targetPos, evaluation);
                boss.agent.Warp(newLocation);
                boss.LookPlayer();
            }

            yield return null;
        }

        float elapsed2 = 0f;

        Vector3 playerPos = PlayerHealth.Instance.transform.position;
        Vector3 targetPosition = playerPos; // + boss.DirToPlayer.normalized * 3f;
        Vector3 startPosition = boss.transform.position;

        while (elapsed2 < approachDurationToPlayer)
        {
            elapsed2 += Time.deltaTime;
            float t = elapsed2 / approachDurationToPlayer;
            float curveValue = approachMovementCurve.Evaluate(t);

            Vector3 newLocation = Vector3.Lerp(startPosition, targetPosition, curveValue);
            boss.agent.Warp(newLocation);

            if (!damageGiven)
            {
                CheckDamage();
            }
            yield return null;
        }

        if (attackVfx)
        {
            Instantiate(attackVfx, boss.transform.position + vfxOffset, attackVfx.transform.rotation);
        }
        damageGiven = false;

        ResetSequence();
    }

    private void CheckDamage()
    {
        Collider[] colls = Physics.OverlapSphere(boss.transform.position, 4f, playerLayer);
        foreach (Collider coll in colls)
        {
            if (coll.TryGetComponent(out PlayerHealth playerHealth))
            {
                playerHealth.TakeDamage(damage);
                damageGiven = true;
            }
        }
    }

    public override void SetBoss(Boss boss)
    {
        this.boss = boss;
    }

    public override void ResetSequence()
    {
        boss.overrideMovement = false;
        boss.FinishAttack();
    }
}
