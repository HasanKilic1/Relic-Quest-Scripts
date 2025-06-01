using MoreMountains.Feedbacks;
using System.Collections;
using UnityEngine;

public class QueenAnimatorController : MonoBehaviour
{
    private PlayerStateMachine stateMachine;
    private Shooter shooter;
    [SerializeField] private float slideTotalDuration;
    [SerializeField] private float slideSpeed;
    [SerializeField] private float projectileSpeed;
    [SerializeField] TrailRenderer[] trails;

    [SerializeField] MMF_Player slideFeedbacks;
    [SerializeField] MMF_Player shootFeedbacks;
    private bool shot;

    private void Awake()
    {
        stateMachine = GetComponentInParent<PlayerStateMachine>();
        shooter = GetComponent<Shooter>();
    }

    private void Start()
    {
        stateMachine.RollDuration = slideTotalDuration;
        stateMachine.RollStartSpeed = slideSpeed;
    }

    public void StartSlide()
    {
        if(slideFeedbacks != null) { slideFeedbacks.PlayFeedbacks(); }
     /*   foreach(TrailRenderer trailRenderer in trails)
        {
            trailRenderer.enabled = true;
            trailRenderer.Clear();
        }*/
        StartCoroutine(ShootWhileSlide());
    }

    private IEnumerator ShootWhileSlide()
    {
        float t = 0f;
        while (t < slideTotalDuration)
        {
            t+= Time.deltaTime;
            if (EnemyInAngle() && !shot)
            {
                shooter.InitProjectile(out Projectile projectile);
                projectile.SetSpeed(projectileSpeed);
                shootFeedbacks?.PlayFeedbacks();
                shot = true;
            }
            yield return null;
        }
        shot = false;

    /*    foreach (TrailRenderer trailRenderer in trails)
        {
            trailRenderer.Clear();
            trailRenderer.enabled = false;            
        }*/
        
    }

    private bool EnemyInAngle()
    {
        if (stateMachine.GetClosestEnemy())
        {
            Vector3 diff = stateMachine.GetClosestEnemy().transform.position - transform.position;
            diff.y = 0;
            Vector3 left = -transform.right;
            left.y = 0;
            float angle = Vector3.Angle(left, diff); 
            if(angle < 45f)
            {
               // HKDebugger.LogWorldText($"Angle : {angle}", transform.position + Vector3.up * 4f);
                return true;
            }
        }
        return false;
    }
}
