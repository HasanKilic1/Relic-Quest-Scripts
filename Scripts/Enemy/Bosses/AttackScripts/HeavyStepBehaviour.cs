using System.Collections;
using System;
using UnityEngine;

public class HeavyStepBehaviour : BossAttackBehaviour
{
    public event Action OnStepUsed;
    Boss attackerBoss;
    IStepperBoss stepperBoss;
    [Header("Animation")]
    [SerializeField] string parameterName;
    [SerializeField] AnimatorControllerParameterType attackParameterType;

    [Header("Combat Attributes")]
    [SerializeField] bool controlCombat;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] public int Damage;
    [SerializeField] private float radius;

    [Header("Time elapsed until step finish")]

    [SerializeField] private float waitBeforeStepFinish;

    [Header("Total step attack duration(transitions included)")]
    [SerializeField] private float totalAttackDuration;

    [SerializeField] GameObject SpawnOnStep;
    

    public override void Attack()
    {
        if (useAnimation)
        {
            switch (attackParameterType)
            {
                case AnimatorControllerParameterType.Bool:
                    attackerBoss.Animator.SetBool(parameterName, true);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    attackerBoss.Animator.SetTrigger(parameterName);
                    break;
            }
        }
        

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        bool beforeAttack = attackerBoss.CurrentPhase.lookAlwaysPlayer;
  //      attackerBoss.currentPhase.lookAlwaysPlayer = false;

        yield return new WaitForSeconds(waitBeforeStepFinish);

        SpawnRelevantObject();
        ApplyAttack();
        OnStepUsed?.Invoke();
        float waitForFinish = totalAttackDuration - waitBeforeStepFinish;


        yield return new WaitForSeconds(waitForFinish);

 //       attackerBoss.currentPhase.lookAlwaysPlayer = beforeAttack;
        ResetSequence();

    }

    public override void ResetSequence()
    {
        attackerBoss.FinishAttack();
    }

    public override void SetBoss(Boss boss)
    {
        attackerBoss = boss;
        stepperBoss = boss.GetComponent<IStepperBoss>();
    }

    private void SpawnRelevantObject()
    {
        if (SpawnOnStep != null)
        {
            GameObject toSpawn = Instantiate(SpawnOnStep, stepperBoss.GetStepPosition(), SpawnOnStep.transform.rotation);
        }
    }

    private void ApplyAttack()
    {
        if (controlCombat)
        {
            Vector3 pos = stepperBoss.GetStepPosition();
            Collider[] colliders = Physics.OverlapSphere(pos, radius, playerLayer);
            foreach (var coll in colliders)
            {
                if (coll.TryGetComponent(out PlayerHealth playerHealth))
                {
                    playerHealth.TakeDamage(Damage);
                }
            }
        }
    }
}
