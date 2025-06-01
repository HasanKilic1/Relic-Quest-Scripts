using UnityEngine;

public class CircularMover : MonoBehaviour
{
    public enum PathDirection
    {
        Right,
        Left
    }
    [SerializeField] Transform mover;
    [Header("Point generation")]
    [SerializeField] PathDirection pathDirection;
    [SerializeField] Vector3 center;
    [SerializeField] float radius = 15f;

    private int nextPoint;
    public Vector3[] path;
    public bool isStopped;
    [SerializeField] float speed = 3f;
    [Header("Gizmo")]
    public int drawCount = 30;

    private void Awake()
    {
        path = new Vector3[drawCount];
        Vector3 firstPoint = center + radius * Vector3.forward;
        Vector3 angleMultiplier;

        angleMultiplier = GetAngleMultiplier();

        float angle = 360f / drawCount;
        for (int i = 0; i < drawCount; i++)
        {
            Vector3 point = Quaternion.AngleAxis(angle * i, angleMultiplier) * firstPoint;
            path[i] = point;
        }
        transform.position = path[0];
        nextPoint = 1;
    }

    private Vector3 GetAngleMultiplier()
    {
        Vector3 angleMultiplier;
        if (pathDirection == PathDirection.Right)
        {
            angleMultiplier = Vector3.up;
        }
        else
        {
            angleMultiplier = -Vector3.up;
        }

        return angleMultiplier;
    }

    private void Update()
    {
        if (!isStopped) 
        {
            FollowPath();
        }
    }
    private void FollowPath()
    {
        Vector3 nextPath = path[nextPoint];
        Vector3 diff = nextPath - transform.position;
        transform.position += speed * Time.deltaTime * diff.normalized;
        if (Vector3.Distance(transform.position, nextPath) < 0.5f)
        {
            nextPoint++;
            if (nextPoint > path.Length - 1)
            {
                nextPoint = 0;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 firstPoint = center + radius * Vector3.forward;
        float angle = 360f / drawCount;
        for (int i = 0; i < drawCount; i++)
        {
            Vector3 point = Quaternion.AngleAxis(angle * i, Vector3.up) * firstPoint;
            Gizmos.DrawWireSphere(point, 0.5f);
        }
    }
}
