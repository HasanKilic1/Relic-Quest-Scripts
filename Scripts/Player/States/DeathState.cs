using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathState : State
{
    private readonly string DEATH_ANIM_HASH = "Death";
    public DeathState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {}

    public override void Enter()
    {
        stateMachine.animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        stateMachine.animator.CrossFadeInFixedTime(DEATH_ANIM_HASH , 0.2f);
        stateMachine.inputClosed = true;
    }

    public override void Tick(float deltaTime)
    {
    }

    public override void Exit()
    {
        stateMachine.animator.updateMode = AnimatorUpdateMode.Normal;
        stateMachine.inputClosed = false;
    }

    
}
