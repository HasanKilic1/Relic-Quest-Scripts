using MoreMountains.Feedbacks;
using UnityEngine;

public class FortGolemBoss : Boss , IStepperBoss
{
    [SerializeField] Transform StepPosition;
    [SerializeField] MMF_Player attackFeedbacks;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }
   
    protected override void Attack()
    {
        base.Attack();
        if(attackFeedbacks != null) { attackFeedbacks.PlayFeedbacks(); }
    }

    public Vector3 GetStepPosition()
    {
        return StepPosition.position;
    }
}
