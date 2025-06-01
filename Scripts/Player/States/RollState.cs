using System;
using UnityEngine;

public class RollState : State
{
    public event Action OnRollStateFinish;
    private int ROLL_HASH = Animator.StringToHash("Roll");

    float timer = 0f;
    Vector3 rollDirection;
    public RollState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {}

    public override void Enter()
    {
        stateMachine.animator.CrossFadeInFixedTime(ROLL_HASH, 0.1f, 0);
        stateMachine.isRolling = true;
        stateMachine.GetComponent<PlayerHealth>().CanTakeDamage = false;

        float x = stateMachine.movementVector.x;
        float z = stateMachine.movementVector.z;

        Vector3 forward = stateMachine.mainCamera.transform.forward;
        Vector3 right = stateMachine.mainCamera.transform.right;

        forward.y = 0f;
        right.y = 0f;

        Vector3 forwardRelative = z * forward;
        Vector3 rightRelative = x * right;
        Vector3 movementVector = forwardRelative + rightRelative;

        stateMachine.selectedCharacter.transform.forward = movementVector;
        if(movementVector.magnitude == 0f)
        {
            movementVector = stateMachine.transform.forward;
        }
        rollDirection = movementVector.normalized;
    }

    public override void Tick(float deltaTime)
    {
        MoveWhileRolling(deltaTime);
    }

    private void MoveWhileRolling(float deltaTime , float overridedRollSpeed = 0f)
    {

        timer += deltaTime;
        float speedVelocity = 10f;
        float moveSpeed = Mathf.SmoothDamp(stateMachine.RollStartSpeed, stateMachine.RollEndSpeed, ref speedVelocity, stateMachine.RollDuration);
        stateMachine.characterController.Move(moveSpeed * deltaTime * rollDirection.normalized);
        Vector3 lookDir = rollDirection;  
        lookDir.y = 0f;
        stateMachine.selectedCharacter.transform.forward = lookDir.normalized;

        if (timer > stateMachine.RollDuration)
        {
            OnRollStateFinish?.Invoke();
            stateMachine.GetIntoNewState(new IdleState(stateMachine));
        }
    }

    public override void Exit()
    {
        stateMachine.GetComponent<PlayerHealth>().CanTakeDamage = true;
        stateMachine.isRolling = false;
        OnRollStateFinish = null;
    }
    
    public void SetRollHash(int hash)
    {
        ROLL_HASH = hash;
    }
}
