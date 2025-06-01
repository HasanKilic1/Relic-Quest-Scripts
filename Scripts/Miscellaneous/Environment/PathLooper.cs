#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class PathLooper : MonoBehaviour
{
    public Vector3[] waypoints; // Array of waypoints for the saw to follow
    public float duration = 2.0f; // Duration to move from one waypoint to the next
    public AnimationCurve movementCurve = AnimationCurve.Linear(0, 0, 1, 1); // Animation curve for movement

    private int currentWaypointIndex = 0; // Index of the current waypoint the saw is moving towards
    private bool movingForward = true;    // Direction of movement along the waypoints
    private float timeElapsed = 0.0f;     // Time elapsed since the saw started moving towards the current waypoint
    public bool ContinuePath = true;
    void Update()
    {
        if (waypoints.Length == 0)
            return;
        if (!ContinuePath) return;
        timeElapsed += Time.deltaTime;
        float t = timeElapsed / duration;
        t = Mathf.Clamp01(t);

        float curveValue = movementCurve.Evaluate(t);
        Vector3 startPosition = waypoints[currentWaypointIndex];
        Vector3 endPosition = movingForward ? waypoints[currentWaypointIndex + 1] : waypoints[currentWaypointIndex - 1];
        transform.position = Vector3.Lerp(startPosition, endPosition, curveValue);
        
        if (t >= 1.0f)
        {

            timeElapsed = 0.0f;

            if (movingForward)
            {
                currentWaypointIndex++;
                if (currentWaypointIndex >= waypoints.Length - 1)
                {
                    currentWaypointIndex = waypoints.Length - 1;
                    movingForward = false;
                }
            }
            else
            {
                currentWaypointIndex--;
                if (currentWaypointIndex <= 0)
                {
                    currentWaypointIndex = 0;
                    movingForward = true;
                }
            }

            if (movingForward)
            {
                if (currentWaypointIndex + 1 < waypoints.Length)
                {
                    transform.LookAt(waypoints[currentWaypointIndex + 1]);
                }
            }
            else
            {
                if (currentWaypointIndex - 1 >= 0)
                {
                    transform.LookAt(waypoints[currentWaypointIndex - 1]);
                }
            }
        }
    }
    void OnDrawGizmosSelected()
    {
        if (waypoints.Length == 0)
            return;

        for (int i = 0; i < waypoints.Length; i++)
        {
            // Set gizmo color
            if (i == 0)
            {
                Gizmos.color = Color.green; // First waypoint
            }
            else if (i == waypoints.Length - 1)
            {
                Gizmos.color = Color.red; // Last waypoint
            }
            else
            {
                Gizmos.color = Color.white; // Intermediate waypoints
            }

            Gizmos.DrawSphere(waypoints[i], 0.2f);

#if UNITY_EDITOR
            GUIStyle style = new GUIStyle();
            style.fontSize = 40;
            style.normal.textColor = Color.blue;
            Handles.Label(waypoints[i] + Vector3.up * 0.3f, i.ToString(), style);
#endif
        }

        // Draw lines between waypoints
        Gizmos.color = Color.blue;
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            Gizmos.DrawLine(waypoints[i], waypoints[i + 1]);
        }
    }
}
