using UnityEngine;
using UnityEngine.Events;

public class PathMovement : MonoBehaviour
{
    [SerializeField] public UnityEvent OnFinish;
    Transform follower;
    public Vector3[] path;
    public bool overrideRotation = true;
    [Header("Movement")]

    public float rotationSpeed = 5f;
    public float followSpeed = 10f;
    public bool accelerate;
    public float acceleration = 10f;
    public float speedMin;
    public float speedMax;

    int targetPathIndex = 1;
    Vector3 nextPoint;
    bool followPath = false;
    [Header("Gizmo")]
    [SerializeField] bool drawPathGizmo = true;

    private void Update()
    {
        if (followPath)
        {
            Follow();
        }
    }

    public void StartFollow(Transform follower) 
    {
        this.follower = follower;
        followPath = true;
        follower.transform.position = path[0];
        nextPoint = path[targetPathIndex];
    }

    private void Follow()
    {
        if(followPath)
        {
            Move();

            if (Vector3.Distance(follower.transform.position, nextPoint) < 0.1f)
            {
                targetPathIndex++;
                if (targetPathIndex > path.Length - 1)
                {
                    followPath = false;
                    OnFinish?.Invoke();
                    return;
                }
                else
                {
                    nextPoint = path[targetPathIndex];
                }
            }
        }
    }

    private void Move()
    {
        if (accelerate)
        {
            followSpeed += acceleration;
            followSpeed = Mathf.Max(followSpeed, speedMin);
            followSpeed = Mathf.Min(followSpeed, speedMax);
        }
        Vector3 diff = nextPoint - follower.transform.position;
        follower.transform.position += diff.normalized * Time.deltaTime * followSpeed;

        if (overrideRotation)
        {
            Vector3 orientation = diff;
            orientation.y = 0f;
            follower.transform.forward = Vector3.Slerp(follower.transform.forward, diff.normalized, rotationSpeed * Time.deltaTime);
        }
    }

    public Vector3 GetPathPosition(int index)
    {
        return path[index];
    }

    public void SetPathPosition(Vector3 position , int index)
    {
        path[index] = position;
    }

    private void OnDestroy()
    {
        OnFinish.RemoveAllListeners();
    }

    private void OnDrawGizmosSelected()
    {
        if (drawPathGizmo)
        {
            if(path.Length > 0)
            {
                for (int i = 0; i < path.Length; i++)
                {
                    if (i + 1 < path.Length)
                    {
                        if(i == 0)
                        {
                            Gizmos.color = Color.green;
                        }
                        else if(i < path.Length)
                        {
                            Gizmos.color = Color.yellow;
                        }
                        Gizmos.DrawWireSphere(path[i], 1f);
                        Gizmos.DrawLine(path[i], path[i + 1]);                        
                    }
                }
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(path[path.Length - 1], 1f);
            }           
        }
    }
}
