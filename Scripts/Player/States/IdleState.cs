using UnityEngine;

public class IdleState : State
{
    private static int IDLE_ANIM_HASH = Animator.StringToHash("Idle");
    private float idleTimer = 0f;
    public IdleState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.animator.CrossFadeInFixedTime(IDLE_ANIM_HASH, 0.1f);
    }
    public override void Tick(float deltaTime)
    {
        idleTimer += deltaTime;
        if (idleTimer < 0.15f || stateMachine.inputClosed) return;
        
        if(stateMachine.movementVector.magnitude > 0)
        {
            stateMachine.GetIntoNewState(new FreeMovementState(stateMachine));
        }
        else
        {
            if (CanAttack)
            {
                stateMachine.GetIntoNewState(new AttackingState(stateMachine));
            }
        }
    }
    public override void Exit()
    {}

}
