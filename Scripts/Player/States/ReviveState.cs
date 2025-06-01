using UnityEngine;

public class ReviveState : State
{
    private readonly string REVIVE_ANIM_HASH = "Revive";
    private float reviveTimer;
    public ReviveState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    { }

    public override void Enter()
    {
        stateMachine.animator.CrossFadeInFixedTime(REVIVE_ANIM_HASH, 0.2f);
        stateMachine.inputClosed = true;
    }

    public override void Tick(float deltaTime)
    {
        reviveTimer += deltaTime;
        if(reviveTimer > 0.6f)
        {
            stateMachine.GetIntoNewState(new IdleState(stateMachine));
        }
    }

    public override void Exit()
    {
        stateMachine.inputClosed = false;
    }
}
