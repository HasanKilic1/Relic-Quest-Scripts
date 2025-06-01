using MoreMountains.Feedbacks;
using System.Collections;
using UnityEngine;

public class FortGolemAnimController : MonoBehaviour
{
    [Header("Heavy Step")]
    [SerializeField] Transform StepPosition;
    [SerializeField] GameObject spikes;
    [SerializeField] float arcRadius = 5f; // The radius of the arc
    [SerializeField] float arcStartAngle = 0f; // The starting angle of the arc in degrees
    [SerializeField] float arcEndAngle = 90f; // The ending angle of the arc in degrees
    [SerializeField] int damage = 100;
    [SerializeField] MMF_Player spikeSpawnFb;

    [Header("Slash Attack")]
    [SerializeField] EnemyProjectile slashPrefab;
    [SerializeField] Transform slashSpawnPosition;
    [SerializeField] Vector3[] slashAngles;
    [SerializeField] int slashDamage;
    [SerializeField] MMF_Player slashFeedbacks;

    [Header("Explosion Attack")]
    [SerializeField] int explosionDamage;
    [SerializeField] float radius;

    Transform player;

    private int rotationIndex;

    private void Start()
    {
        player = PlayerController.Instance.transform;
    }

    #region SpikeAttack
    public void SpawnSpikes()
    {
        GameObject spike = Instantiate(spikes, StepPosition.position, Quaternion.identity);
        spike.transform.forward = StepPosition.forward;
        if(spikeSpawnFb != null) { spikeSpawnFb.PlayFeedbacks(); }

        StartCoroutine(TryGiveDamage());
    }

    private IEnumerator TryGiveDamage()
    {
        yield return new WaitForSeconds(0.1f);
        if (IsPlayerInArc())
        {
            PlayerHealth.Instance.TakeDamage(damage);
        }
    }

    bool IsPlayerInArc()
    {
        Vector3 toPlayer = player.position - StepPosition.position;

        float distanceToPlayer = toPlayer.magnitude;

        // Check if the player is within the radius
        if (distanceToPlayer > arcRadius)
        {
            return false;
        }

        // Calculate the angle between the arc center's forward direction and the player
        float angleToPlayer = Vector3.Angle(StepPosition.forward, toPlayer);

        // Check if the angle is within the specified arc range
        if (angleToPlayer >= arcStartAngle && angleToPlayer <= arcEndAngle)
        {
            return true;
        }

        return false;
    }
    #endregion

    #region SlashAttack
    public void SendSlashes()
    {
        Quaternion angle = Quaternion.Euler(slashAngles[rotationIndex]);
        EnemyProjectile slash = Instantiate(slashPrefab, slashSpawnPosition.position, angle);
        slash.SetDamage(slashDamage);
        slash.Shoot(transform.forward);
        rotationIndex++;
        if(slashFeedbacks != null)
        {
            slashFeedbacks.PlayFeedbacks();
        }
    }

    public void ResetSlashAttack() 
    {
        rotationIndex = 0; 
    }

    #endregion

    #region ExplosiveAttack

    public void Explode()
    {

    }

    #endregion

    void OnDrawGizmos()
    {
        if (StepPosition == null)
            return;

        Gizmos.color = Color.blue;

        Vector3 startDirection = Quaternion.Euler(0, arcStartAngle, 0) * StepPosition.forward;
        Vector3 endDirection = Quaternion.Euler(0, arcEndAngle, 0) * StepPosition.forward;

        // Draw the arc
        DrawArc(StepPosition.position, startDirection, arcEndAngle - arcStartAngle, arcRadius);
    }

    void DrawArc(Vector3 center, Vector3 startDirection, float arcAngle, float radius)
    {
        int segments = 30;
        float angleStep = arcAngle / segments;
        Vector3 previousPoint = center + startDirection * radius;

        for (int i = 1; i <= segments; i++)
        {
            float currentAngle = angleStep * i;
            Vector3 nextDirection = Quaternion.Euler(0, currentAngle, 0) * startDirection;
            Vector3 nextPoint = center + nextDirection * radius;

            Gizmos.DrawLine(previousPoint, nextPoint);
            previousPoint = nextPoint;
        }

        // Draw radius lines
        Gizmos.DrawLine(center, center + startDirection * radius);
        Gizmos.DrawLine(center, center + Quaternion.Euler(0, arcAngle, 0) * startDirection * radius);
    }
}
