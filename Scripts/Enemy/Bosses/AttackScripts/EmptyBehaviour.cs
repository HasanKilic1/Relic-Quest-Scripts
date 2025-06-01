using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyBehaviour : BossAttackBehaviour
{
    [SerializeField] private string parameterName;
    [SerializeField] private AnimatorControllerParameterType parameterType;
    [SerializeField] private float attackDuration;

    public override void Attack()
    {
        if(parameterType == AnimatorControllerParameterType.Trigger) {
            boss.Animator.SetTrigger(parameterName);
        }
        else if(parameterType == AnimatorControllerParameterType.Bool)
        {
            boss.Animator.SetBool(parameterName, true);
        }
        Invoke(nameof(ResetSequence), attackDuration);
    }

    public override void ResetSequence()
    {
        base.ResetSequence();
    }

    public override void SetBoss(Boss boss)
    {
        base.SetBoss(boss);
    }
    
}
