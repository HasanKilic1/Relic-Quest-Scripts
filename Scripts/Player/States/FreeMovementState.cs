using System;
using UnityEngine;

public class FreeMovementState : State
{
    private static int SPEED_HASH = Animator.StringToHash("Speed");
    private static int MOVEMENT_TREE_HASH = Animator.StringToHash("MovementBlendTree");
    float currentSpeed = 0f;

    public FreeMovementState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {}

    public override void Enter()
    {
        stateMachine.animator.CrossFadeInFixedTime(MOVEMENT_TREE_HASH, 0.15f , 0);
    }
    public override void Tick(float deltaTime)
    {
        if (stateMachine.inputClosed)
        {
            stateMachine.GetIntoNewState(new IdleState(stateMachine));
            return;
        }
        MoveCameraRelative(deltaTime);
        if (CanAttack)
        {
            stateMachine.GetIntoNewState(new AttackingState(stateMachine));
        }
        if (stateMachine.movementVector.magnitude == 0f)
        {
            stateMachine.GetIntoNewState(new IdleState(stateMachine));
        }
    }

    public override void Exit()
    {}

    protected void MoveCameraRelative(float deltaTime)
    {
        float x = stateMachine.movementVector.x;
        float z = stateMachine.movementVector.z;

        Vector3 forward = stateMachine.mainCamera.transform.forward;
        Vector3 right = stateMachine.mainCamera.transform.right;

        forward.y = 0f;
        right.y = 0f;

        Vector3 forwardRelative = z * forward;
        Vector3 rightRelative = x * right;
        Vector3 movementVector = forwardRelative + rightRelative;

        if (movementVector.magnitude > 0f)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, stateMachine.maxSpeed, stateMachine.acceleration * deltaTime);
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0f, stateMachine.acceleration * 2f * deltaTime);
        }

        
        stateMachine.transform.forward = Vector3.Slerp(stateMachine.transform.forward, movementVector.normalized, deltaTime * stateMachine.rotationSensivity);
        stateMachine.selectedCharacter.transform.forward = stateMachine.transform.forward;
        stateMachine.characterController.Move(deltaTime * currentSpeed * movementVector.normalized);
        stateMachine.animator.SetFloat(SPEED_HASH, currentSpeed / stateMachine.maxSpeed);
    }

}
