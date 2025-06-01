using MoreMountains.Feedbacks;
using Unity.VisualScripting;
using UnityEngine;

public class Obstacle : MonoBehaviour , IGridObject , IContactProvider
{
    public enum MotionType
    {
        Static,
        Kinetic
    }

    public enum DisableType
    {
        GoUp,
        GoDown,
        Destroy
    }

    public MotionType motionType = MotionType.Kinetic;
    public DisableType disableType = DisableType.GoUp;
    [SerializeField] private float waitBeforeDisable = 1f;
    [Header("Only Moveable Obstacles")]
    [SerializeField] private float heightDiffAtStart;
    [SerializeField] float speed = 15f;
    [SerializeField] float acceleration = 10f;
    [SerializeField] bool useDeclaredPosition;
    private bool isMoving = true;
    private bool isDisabled = false;
    private float totalMovedSinceEnabled = 0f;
    private float totalMovedSinceDisabled = 0f;

    [SerializeField] MMF_Player approachFeedbacks;
    [SerializeField] MMF_Player disableFeedbacks;
    [SerializeField] GameObject targetAreaPrefab;

    [SerializeField] Vector3 targetPosition;
    WorldGrid grid;
    GameObject targetArea;

    private void Start()
    {
        if(useDeclaredPosition) { }
    }
    public void SetGrid(WorldGrid grid)
    {
        if (useDeclaredPosition) { this.grid = GridManager.Instance.GetClosestGridOnLocation(targetPosition); }
        this.grid = grid;        
    }
    public void SetPosition(Vector3 pos)
    {
        if(useDeclaredPosition) { return; }
        if(motionType == MotionType.Static)
        {
            transform.position = pos;
        }
        else
        {
            transform.position = pos + heightDiffAtStart * Vector3.up;
            targetPosition = pos;
            if(targetArea != null)
            {
                targetArea = Instantiate(targetAreaPrefab, pos + Vector3.up * 0.2f, Quaternion.identity);
            }
        }
    }

    private void Update()
    {
        if(motionType == MotionType.Kinetic)
        {
            if (isMoving)
            {
                GoTargetPos();
            }
        }
        if (isDisabled)
        {
            DisabledUpdate();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            Disable();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(isMoving && collision.collider.TryGetComponent(out PlayerHealth playerHealth))
        {
            playerHealth.TakeDamage((int)playerHealth.GetMaxHealth);
        }        
    }

    private void GoTargetPos()
    {
        Vector3 posBefore = transform.position;
        Vector3 diff = targetPosition - transform.position;
        transform.position += speed * Time.deltaTime * diff.normalized;
        speed += acceleration * Time.deltaTime; 
        Vector3 posNow = transform.position;
        float moved = Vector3.Distance(posBefore, posNow);
        totalMovedSinceEnabled += moved;
        if(totalMovedSinceEnabled > heightDiffAtStart)
        {
            Stop();
        }
    }

    private void Stop()
    {
        speed = 0f;
        isMoving = false;
        approachFeedbacks?.PlayFeedbacks();
        if (targetArea)
        {
            Destroy(targetArea);
        }
    }

    public void Disable()
    {
        disableFeedbacks?.PlayFeedbacks();
        Invoke(nameof(PermitDisable), waitBeforeDisable);       
    }

    private void PermitDisable()
    {
        isDisabled = true;
        grid.Clear();
        GridManager.Instance.RemoveObjectFromList(this);
    }

    private void DisabledUpdate()
    {
               
        if(disableType == DisableType.GoUp)
        {
            Vector3 positionA = transform.position;
            transform.position += Vector3.up * 50f * Time.deltaTime;
            Vector3 positionB = transform.position;
            totalMovedSinceDisabled += Vector3.Distance(positionA, positionB);
            if (totalMovedSinceDisabled > 50f)
            {
                Destroy(gameObject);
            }
            return;
        }
        else if(disableType == DisableType.GoDown)
        {
            Vector3 positionA1 = transform.position;
            transform.position -= Vector3.up * 50f * Time.deltaTime;
            Vector3 positionB1 = transform.position;
            totalMovedSinceDisabled += Vector3.Distance(positionA1, positionB1);
            if (totalMovedSinceDisabled > 50f)
            {
                Destroy(gameObject);
            }
            
            return;
        }
        else
        {
            Destroy(gameObject);
        }

        
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(targetPosition, 1f);
    }
}
