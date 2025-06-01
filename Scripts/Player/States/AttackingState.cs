using UnityEngine;

public class AttackingState: State
{
    private static int SHOOTING_HASH = Animator.StringToHash("Shoot");
    public AttackingState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {}

    public override void Enter()
    {
        stateMachine.animator.CrossFadeInFixedTime(SHOOTING_HASH, 0.05f , 0);
    }
    public override void Tick(float deltaTime)
    {
        if (!CanAttack ||  stateMachine.GetClosestEnemy() == null)                                        
        {
            stateMachine.GetIntoNewState(new IdleState(stateMachine));
            return;
        }

        FaceClosestEnemy(deltaTime);
    }
    public override void Exit()
    {}

    private void FaceClosestEnemy(float deltaTime)
    {          
        Vector3 diff = stateMachine.GetClosestEnemy().transform.position - stateMachine.selectedCharacter.transform.position;
        diff.y = 0f;

        Quaternion targetRot = Quaternion.LookRotation(diff.normalized, Vector3.up);
        stateMachine.selectedCharacter.transform.rotation = Quaternion.Slerp(stateMachine.selectedCharacter.transform.rotation, targetRot, deltaTime * 10f);
    }  

}
