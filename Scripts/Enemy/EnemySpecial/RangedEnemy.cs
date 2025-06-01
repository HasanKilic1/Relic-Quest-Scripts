using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : Enemy
{
  
    public event Action OnAttackFinished;
    [SerializeField] EnemyProjectile projectile;
    [SerializeField] Transform shootPos;
    [SerializeField] bool isUsingMultipleAttacks = false;
    [Tooltip("Will use at index 0 as standard")]
    [SerializeField] List<string> attackAnimNames;
    [SerializeField] List<float> attackDurations;
    private int moveBool = Animator.StringToHash("isMoving");
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

        if (isFreezed) { return; }
        HandleStates();     
        
        if(isAttacking) FaceToTarget(playerTargetedPosition.position);
    }    
    protected override void Attack()
    {       
        StartCoroutine(AttackRoutine()); 
    }     

    private IEnumerator AttackRoutine()
    {
        Stop();
        SetRandomTrigger(out float attack_Duration);
                
        yield return new WaitForSeconds(attack_Duration);

        OnAttackFinished?.Invoke();
        ResetAttackTimer();

        yield return new WaitForSeconds(1f);

        HandleAttackEnd();
    }
    private void SetRandomTrigger(out float attack_Duration)
    {
        if (!isUsingMultipleAttacks) 
        { 
            animator.SetTrigger(attackAnimNames[0]);
            attack_Duration = attackDurations[0];
            return;
        }
        float chanceForPerAttack = 100 / attackAnimNames.Count;
        float random = UnityEngine.Random.Range(0, 100f);
        int selectedId = (int)(random / chanceForPerAttack);
        animator.SetTrigger(attackAnimNames[selectedId]);
        attack_Duration = attackDurations[selectedId];
    }


    public EnemyProjectile GetProjectile => projectile;
    public Transform ShootPos => shootPos;
    public float Damage => damage;
    public float Range => attackRange;
    public Vector3 ProjectileDirection => playerTargetedPosition.position - shootPos.position;

/*
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(shootPos.position , ShootPos.position + transform.forward * attackRange);
    }*/
}
