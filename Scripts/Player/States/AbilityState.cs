using System.Linq;
using UnityEngine;

public class AbilityState : State
{
    private MonoBehaviour ability;
    private static readonly int ABILITY_HASH = Animator.StringToHash("SpecialAttack");
    private Transform lookTarget;
    private SkillUser skillUser;
    public AbilityState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {}

    public override void Enter()
    {
        stateMachine.animator.CrossFadeInFixedTime(ABILITY_HASH, 0.15f, 0);
        skillUser = stateMachine.selectedCharacter.GetComponent<SkillUser>();
    }
    public override void Tick(float deltaTime)
    {        
        LookClosestTarget(deltaTime);
    }

    private void LookClosestTarget(float deltaTime)
    {
        if (skillUser.LookTargetWhileSkillPlays)
        {
            if (lookTarget == null && stateMachine != null && stateMachine.GetClosestEnemy() != null)
            {
                lookTarget = stateMachine.GetClosestEnemy().transform;
            }
            else if (stateMachine.GetClosestEnemy() == null)
            {
                AssignClosestEnemy();
            }
            
            if(lookTarget != null)
            {
                float rotationSens = 25f;

                Vector3 diffToTarget = (lookTarget.position - stateMachine.selectedCharacter.transform.position).normalized;
                diffToTarget.y = 0f;
                stateMachine.selectedCharacter.transform.rotation = Quaternion.Slerp(stateMachine.selectedCharacter.transform.rotation,
                                                                                     Quaternion.LookRotation(diffToTarget),
                                                                                     rotationSens * deltaTime);
            }           
        }        
    }

    private void AssignClosestEnemy()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if(enemies.Length > 0)
        {
            lookTarget = enemies.
                    OrderBy(e => Vector3.Distance(e.transform.position, stateMachine.transform.position)).
                    First().transform;
        }
       
    }

    public void SetAbility(MonoBehaviour ability , int level,int skillDamage)
    {
        (ability as IActiveSkill).SetSkillData(level , skillDamage);
        (ability as IActiveSkill).SetPlayerScript(this.stateMachine);
    }


    public override void Exit()
    {}
       
}
