using MoreMountains.Feedbacks;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MeleeEnemy : Enemy
{
    [SerializeField] UnityEvent OnContact;
    public enum DamageControlType
    {
        Animation,
        Physics,
    }

    public event Action OnAttackFinished;
    [Header("Melee enemy can use different Animation,Range and Duration values for different attacks")]

    [SerializeField] string[] attackAnimNames;
    [SerializeField] float[] rangeOnAttacks;
    [SerializeField] float[] durationOnAttacks;

    [Header("Control Damage Type")]
    [SerializeField] DamageControlType damageControlType = DamageControlType.Animation;

    [Header("When damageControlType = Physics")]
    [SerializeField] float delayForCheck = 0.5f;
    [SerializeField] Transform providedMeleeAttackPosition;
    [SerializeField] float radius = 3f;
    [SerializeField] MMF_Player attackFeedbacks;
    [SerializeField] MMF_Player attackFinishFeedbacks;


    private float currentRange;
    private int currentTrigger;
    private float currentAttackDuration;    

    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
        ChooseRandomAttack();
    }

    protected override void Update()
    {
        base.Update();
        if(isFreezed) { return; }
        HandleStates();       
    }
    protected override void Attack()
    {
        if(attackFeedbacks != null) { attackFeedbacks.PlayFeedbacks(); }
        StartCoroutine(AttackRoutine());            
    }

    private IEnumerator AttackRoutine()
    {
        Stop();
        animator.SetTrigger(currentTrigger);

        if(damageControlType == DamageControlType.Physics)
        {
            Invoke(nameof(CheckDamageByRadius), delayForCheck);
        }

        yield return new WaitForSeconds(currentAttackDuration);

        OnAttackFinished?.Invoke();
        ResetAttackTimer();
        if(attackFinishFeedbacks != null) { attackFinishFeedbacks.PlayFeedbacks(); }    

        yield return new WaitForSeconds(1f);

        HandleAttackEnd();
        ChooseRandomAttack();
    }

    private void ChooseRandomAttack()
    {
        int rand = UnityEngine.Random.Range(0, attackAnimNames.Length);

        currentTrigger = Animator.StringToHash(attackAnimNames[rand]);
        currentAttackDuration = durationOnAttacks[rand];
        currentRange = rangeOnAttacks[rand];

        this.attackRange = currentRange;
    }

    private void CheckDamageByRadius()
    {
        Collider[] colls = Physics.OverlapSphere(providedMeleeAttackPosition.position, radius);
        foreach (Collider coll in colls)
        {
            if(coll.TryGetComponent(out PlayerHealth playerHealth))
            {
                if (playerHealth.CanTakeDamage)
                {
                    playerHealth.TakeDamage(Damage);
                    OnContact?.Invoke();
                }                
            }
        }
    }

    public int Damage => (int)damage;

    private void OnDrawGizmosSelected()
    {
        if (providedMeleeAttackPosition != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(providedMeleeAttackPosition.position, radius);
        }
    }
}
