using MoreMountains.Feedbacks;
using UnityEngine;

public class AbilityCardGiver : GiftResource
{
    Transform targetPosition;
    [SerializeField] float startSpeed;
    
    [SerializeField] MMF_Player onStartFeedbacks;
    [SerializeField] MMF_Player onStopFeedBacks;
    [SerializeField] MMF_Player inactivationFeedbacks;

    [SerializeField] private float flyAcceleration = 50f;
    [SerializeField] private float flySpeed = 25f;
    PlayerStateMachine playerStateMachine;
    private bool isMoving = true;
    private bool isFlying = false;

    private float timeToInactivate;
    private void Start()
    {        
       
    }  

    void Update()
    {
        if (isMoving && targetPosition != null)
        {
            Vector3 diff = targetPosition.position - transform.position;
            diff.y = 0f;
            float distance = Vector3.Distance(targetPosition.position, transform.position);
            float speed = startSpeed * (distance - 1.5f);
            transform.forward = diff.normalized;
            transform.position += diff.normalized * speed * Time.deltaTime;        
            if(distance < 2f)
            {
                isMoving = false;
                HandleOnStop();
            }
        }
        if (isFlying)
        {
            flySpeed += flyAcceleration * Time.deltaTime; 
            transform.position += Vector3.up * flySpeed * Time.deltaTime;
           
        }
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        isMoving = true;
    }

    private void HandleOnStop()
    {
        onStopFeedBacks.PlayFeedbacks();
        playerStateMachine.inputClosed = false;
    }

    private void Fly()
    {
        inactivationFeedbacks?.PlayFeedbacks();
        isFlying = true;
        Destroy(gameObject , 5f);
        ChangeCameraLook(PlayerHealth.Instance.transform);
    }
    private void ChangeCameraLook(Transform target)
    {
        CameraController.Instance.ChangeLookTarget(target);
    }

    public void SetTargetPosition(Transform target)
    {
        targetPosition = target;
    }

    public override void Initialize()
    {
        playerStateMachine = PlayerController.Instance.GetComponent<PlayerStateMachine>();
        playerStateMachine.inputClosed = true;
        ChangeCameraLook(transform);
        onStartFeedbacks.PlayFeedbacks();
    }

    public override void Give()
    {
        
    }
}
