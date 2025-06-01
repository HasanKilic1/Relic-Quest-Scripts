using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    protected PlayerStateMachine stateMachine;
    public State(PlayerStateMachine playerStateMachine)
    {
        this.stateMachine = playerStateMachine;
    }

    public abstract void Enter();
    public abstract void Tick(float deltaTime); 
    public abstract void Exit();

    protected bool CanAttack => stateMachine.GetClosestEnemy() != null && stateMachine.IsStopped() && stateMachine.CanAttack;
}
