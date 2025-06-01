using MoreMountains.Feedbacks;
using System.Collections;
using UnityEngine;

public class SpiritDemonAnimController : MonoBehaviour
{
    [SerializeField] SpiritDemonBoss boss;
    [Header("Attack1")]
    [SerializeField] Transform shootPos;
    [SerializeField] EnemyProjectile projectile;
    [SerializeField] int projectileDamage;

    [Header("Attack2")]
    [SerializeField] MultipleShooter circle;
    [SerializeField] int circleCount;
    [SerializeField] float circleDistance;
    [SerializeField] float circleHeight;
    [SerializeField] int attack2Damage;

    [Header("Attack3")]
    [SerializeField] int attack3Damage;
    [SerializeField] float radius;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] AnimationCurve curve = AnimationCurve.Linear(0,0,1,1);
    [SerializeField] float stopDistance;
    [SerializeField] float movementDuration;
    [SerializeField] ParticleSystem dangerZonePrefab;
    [SerializeField] MMF_Player movementStartFeedbacks;
    [SerializeField] MMF_Player movementFinishFeedbacks;
    private ParticleSystem dangerZone;
    private Vector3 start;
    private Vector3 end;
    private void Awake()
    {
        dangerZone = Instantiate(dangerZonePrefab);
        dangerZone.Stop();
    }
    #region Attack1

    public void Attack1Shoot()
    {
        EnemyProjectile demonProjectile = Instantiate(projectile, shootPos.position, shootPos.rotation);
        demonProjectile.Shoot(boss.DirToPlayer.normalized);
        demonProjectile.SetDamage(projectileDamage);
    }

    #endregion

    #region Attack2

    public void ShowCircles()
    {
        for (int i = 0; i < GetCirclePositionsAroundPlayer().Length; i++)
        {
            MultipleShooter circleShooter = Instantiate(circle , Vector3.zero , Quaternion.identity);
            Vector3 startPoint = GetCirclePositionsAroundPlayer()[i];

            PositionAnimationer positionAnimationer = circleShooter.GetComponent<PositionAnimationer>();
            positionAnimationer.startPoint = startPoint;
            positionAnimationer.endPoint = GetCirclePositionsAroundPlayer()[i] + Vector3.up * circleHeight;
            positionAnimationer.Animate();

            circleShooter.SetDamage(attack2Damage);
            circleShooter.StartShootingDelayed(positionAnimationer.duration);
        }           
    }
    
    private Vector3[] GetCirclePositionsAroundPlayer()
    {
        Vector3[] positions = new Vector3[circleCount];
        float angle = 360f / circleCount;
        Vector3 firstPos = Vector3.zero + Vector3.forward * circleDistance;
        firstPos = Quaternion.AngleAxis(-45, Vector3.up) * firstPos;
        positions[0] = firstPos;
        for (int i = 1; i < circleCount; i++)
        {
            positions[i] = Quaternion.AngleAxis(angle * i, Vector3.up) * firstPos;
        }

        return positions;
    }
    #endregion

    #region Attack3
    private void ChooseDashPosition()
    {
        start = boss.transform.position;
        end = NavMeshManager.Instance.FindClosestNavMeshPosition(boss.player.position);

        if (dangerZone)
        {
            dangerZone.transform.position = end + Vector3.up * 0.3f;
            dangerZone.Play();
        }
    }
    public void StartMovement()
    {
        if(movementStartFeedbacks) movementStartFeedbacks.PlayFeedbacks();       

        StartCoroutine(MovementRoutine());
    }

    private IEnumerator MovementRoutine()
    {
        float t = 0f;
       
        while(t < movementDuration)
        {
            t+= Time.deltaTime;
            float evaluated = curve.Evaluate(t / movementDuration);
            Vector3 pos = Vector3.Lerp(start, end, evaluated);
            boss.agent.Warp(pos);
            yield return null;
        }

        CheckArea();

        if (movementFinishFeedbacks) movementFinishFeedbacks.PlayFeedbacks();
        if (dangerZone) dangerZone.Stop();
        
    }

    private void CheckArea()
    {
        Collider[] colls = Physics.OverlapSphere(boss.transform.position, radius, playerLayer);
        foreach (var coll in colls)
        {
            if(coll.TryGetComponent(out PlayerHealth playerHealth))
            {
                playerHealth.TakeDamage(attack3Damage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
    #endregion
}
