using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Events;

public class AttackRelic : MonoBehaviour, IRelic
{
    [SerializeField] private float increasePerLevel = 50f;
    [SerializeField] private int level = 1;
    [SerializeField] private float locationChangeInterval = 5f;
    public UnityEvent OnPlayerEnter;
    public UnityEvent OnPlayerLeave;
    [SerializeField] MMF_Player enterFeedbacks;
    [SerializeField] MMF_Player exitFeedbacks;
    Vector3 destination;
    float movementStartTimer;
    private bool isChangingLocation = false;

    private void Update()
    {
        if(ShouldStartLocationChange)
        {
            StartLocationChange();
        }
        TryMove();
    }
    private void StartLocationChange()
    {
        isChangingLocation = true;
        float radius = UnityEngine.Random.Range(3f, 7f);
        destination = NavMeshManager.Instance.GetRandomPositionWithinRadius(transform.position , radius);
        destination.y = transform.position.y;
    }

    private void TryMove()
    {
        if(isChangingLocation)
        {
            transform.position = Vector3.Lerp(transform.position, destination, 1f * Time.deltaTime);
            if(Vector3.Distance(transform.position, destination) <= 1f )
            {
                Stop();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerController playerController))
        {
            playerController.InfluenceAttribute(AttributeType.Damage, increasePerLevel * level);
            OnPlayerEnter?.Invoke();
            enterFeedbacks?.PlayFeedbacks();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerController playerController))
        {
            playerController.InfluenceAttribute(AttributeType.Damage, -increasePerLevel * level);
            OnPlayerLeave?.Invoke();
            exitFeedbacks?.PlayFeedbacks();
        }
    }

    private void Stop()
    {
        isChangingLocation = false;
        movementStartTimer = Time.time + locationChangeInterval;
    }

    public void ResetEffect(PlayerStateMachine stateMachine)
    {}

    public void SettleEffect(PlayerStateMachine stateMachine)
    {}

    private bool ShouldStartLocationChange => Time.time > movementStartTimer && !isChangingLocation;

    private void OnDestroy()
    {
        OnPlayerEnter.RemoveAllListeners();
        OnPlayerLeave.RemoveAllListeners();
    }

}
