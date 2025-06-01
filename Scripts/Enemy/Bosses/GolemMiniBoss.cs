using MoreMountains.Feedbacks;
using System.Collections;
using UnityEngine;

public class GolemMiniBoss : Boss , IShooterBoss
{
    public Transform PunchPosition;
    [SerializeField] private Transform ShootPosition;
    [SerializeField] private AnimationCurve backwardMovementCurve;
    [SerializeField] private float backwardMovementDuration;
    [SerializeField] private float backwardMovementDistance;
    [SerializeField] private MMF_Player dangerFeedbacks;

    private bool alreadyMoving = false;
    private void OnEnable()
    {
        RangedAttackBehaviour.OnShoot += MoveBW;
    }

    private void OnDisable()
    {
        RangedAttackBehaviour.OnShoot -= MoveBW;
    }

    private void MoveBW(Boss boss)
    {
        if(alreadyMoving && boss != this) { return; }
        StartCoroutine(BackwardMoveRoutine());
    }

    private IEnumerator BackwardMoveRoutine()
    {
        float elapsed = 0f;
        alreadyMoving = true;
       
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = transform.position - transform.forward * backwardMovementDistance;
        while (elapsed < backwardMovementDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / backwardMovementDuration;
            float evaluated = backwardMovementCurve.Evaluate(t);

            Vector3 newPosition = Vector3.Lerp(startPosition, targetPosition, evaluated);
            this.agent.Warp(newPosition);

            yield return null;
        }

        alreadyMoving = false;
    }
    protected override void Awake()
    {
        base.Awake();     
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Attack()
    {
        base.Attack();
        dangerFeedbacks?.PlayFeedbacks();
    }

    public void PlayMeleeAttackFeedbacks()
    {

    }

    public Transform GetShootPos()
    {
        return ShootPosition;
    }
}
