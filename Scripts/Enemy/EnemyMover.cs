using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyMover : MonoBehaviour
{
    [Header("Standard Properties")]
    [SerializeField] NavMeshAgent Agent;
    [SerializeField] float patrolSpeed;
    [SerializeField] Animator animator;
    [SerializeField] MovementParameterType movementParameterType;
    [SerializeField] string movementParameterName = "isMoving";
    [SerializeField] private float stopSensivity;
    [Range(0f , 1f)][SerializeField] private float backwardSpeedMultiplier = 0.5f;
    Vector3 simpleMoveDestination;
    [Header("Events")]
    [SerializeField] UnityEvent OnMovementStarted;
    [SerializeField] UnityEvent OnMovementFinished;

    Transform player;
    private float lerpSpeed = 5f;
    public bool CartesianMovementPermitted = true;
    private Vector3 targetLocation;
    private void Start()
    {
        player = PlayerController.Instance.transform;
    }

    private void Update()
    {        
        if(Vector3.Distance(transform.position , simpleMoveDestination) < stopSensivity)
        {
            StopImmediately();
        }
    }

    public void SimpleMove(Vector3 location , float movementSpeed)
    {
        if(movementParameterType == MovementParameterType.Bool)
        {
            animator.SetBool(movementParameterName, true);           
        }
        Agent.isStopped = false;
        Agent.speed = movementSpeed;
        simpleMoveDestination = location;
        Agent.SetDestination(simpleMoveDestination);
    }

    public void CartesianLookerMove(Vector3 location , float speed)
    {
        if (!CartesianMovementPermitted)
        {
            SimpleMove(location , speed);
            return;
        }

        if (player == null) return;
        Agent.isStopped = false;
        Agent.speed = speed;
        Vector3 targetPosition = location;
        Agent.SetDestination(targetPosition);

        // Calculate direction to the player
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        // Rotate to face the player
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);

        // Calculate movement direction relative to the player
        Vector3 movementDirection = Agent.velocity;

        if (movementDirection.magnitude > 0.1f) 
        {
            movementDirection.Normalize();

            float forwardAmount = Vector3.Dot(transform.forward, movementDirection);
            float rightAmount = Vector3.Dot(transform.right, movementDirection);

            float currentForward = animator.GetFloat("Forward");
            float currentRight = animator.GetFloat("Right");

            animator.SetFloat("Forward", Mathf.Lerp(currentForward, forwardAmount, Time.deltaTime * lerpSpeed));
            animator.SetFloat("Right", Mathf.Lerp(currentRight, rightAmount, Time.deltaTime * lerpSpeed));
            
            animator.SetBool("isMoving", true);
            if(forwardAmount < -0.1f && Mathf.Abs(rightAmount) < 0.5f) { Agent.speed = speed / backwardSpeedMultiplier; }
        }
        else
        {
            animator.SetFloat("Forward", Mathf.Lerp(animator.GetFloat("Forward") , 0f , lerpSpeed * 3 * Time.deltaTime));
            animator.SetFloat("Forward", Mathf.Lerp(animator.GetFloat("Right"), 0f, lerpSpeed * 3 * Time.deltaTime));
            animator.SetBool("isMoving", false);
        }

        if(Vector3.Distance(transform.position, location) < stopSensivity)
        {
            StopImmediately();
        }
    }      

    public void StartPatrolling(float duration)
    {
        duration = Mathf.Max(1f, duration);
        StartCoroutine(PatrolRoutine(duration));
    }

    private IEnumerator PatrolRoutine(float duration)
    {
        yield return new WaitForSeconds(1f);
        Vector3 targetPos = GetValidPosition();

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            CartesianLookerMove(targetPos , patrolSpeed);
            if(Vector3.Distance(Agent.transform.position , targetPos) < stopSensivity)
            {
                targetPos = GetValidPosition();
            }

            yield return null;
        }
    }

    private Vector3 GetValidPosition()
    {
        return NavMeshManager.Instance.GetRandomPositionWithinRadius(transform.position, 25f);
    }

    public void StopImmediately()
    {
        Agent.isStopped = true;
        Agent.speed = 0;
        StopAllCoroutines();
        if (movementParameterType == MovementParameterType.Bool)
        {
            animator.SetBool(movementParameterName, false);
        }
        if (CartesianMovementPermitted)
        {
            animator.SetFloat("Forward", 0f);
            animator.SetFloat("Right", 0f);
        }
        OnMovementFinished?.Invoke();
    }

}
