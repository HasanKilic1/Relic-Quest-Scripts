using System;
using System.Collections.Generic;
using UnityEngine;

public class ComboMeleeAttackBehaviour : BossAttackBehaviour
{
    [SerializeField] List<MeleeAttack> meleeAttackList;
    private MeleeAttack meleeAttack;
    private int currentMeleeAttackID;
    private bool isAttacking;
    private void Start()
    {
        foreach (var meleeAttack in meleeAttackList)
        {
            meleeAttack.OnFinish += FinishAttack;
        }
    }

    private void Update()
    {
        if(meleeAttack != null && isAttacking)
        {
            meleeAttack.Tick();
        }
    }

    public override void Attack()
    {
        currentMeleeAttackID = 0;
        boss.overrideMovement = true;
        isAttacking = true;

        meleeAttack = new MeleeAttack();
        meleeAttack = meleeAttackList[currentMeleeAttackID];
        meleeAttack.Enter(boss.Animator, boss);
    }

    private void FinishAttack()
    {
        currentMeleeAttackID++;
        if(currentMeleeAttackID >= meleeAttackList.Count)
        {
            ResetSequence();
        }
        else
        {
            meleeAttack = new MeleeAttack();
            meleeAttack = meleeAttackList[currentMeleeAttackID];
            meleeAttack.Enter(boss.Animator , boss);
        }
    }

    public override void ResetSequence()
    {
        meleeAttack = null;
        isAttacking = false;
        boss.overrideMovement = false;  
        boss.FinishAttack();
    }

    public override void SetBoss(Boss boss)
    {
        this.boss = boss;
    }
    
}

[Serializable]
public class MeleeAttack
{
    public event Action OnFinish;
    public bool CanTrigger;
    public string TriggerParameter;

    public float Duration;
    public bool CanMove;
    public float MoveDistance;
    public AnimationCurve MovementCurve;
    public Boss boss;
    private float speed;
    private float timeElapsed;
    private Vector3 startPosition;
    private Vector3 movePosition;
    public void SetAttacker(Boss agent) { boss = agent; }
    public void TriggerAnim(Animator animator) { animator.SetTrigger(TriggerParameter); }

    public void Enter(Animator animator , Boss attacker)
    {
        SetAttacker(attacker);
        if(CanTrigger) { animator.SetTrigger(TriggerParameter); }
        startPosition = attacker.transform.position;

        Vector3 diff = boss.player.transform.position - boss.transform.position;

        movePosition = boss.transform.position + boss.transform.forward * MoveDistance;
        movePosition.y = boss.transform.position.y;

        boss.LookPlayerInstantly();
    }

    public void Tick()
    {        
        if(boss != null)
        {
            
            timeElapsed += Time.deltaTime;
            float evaluation = timeElapsed / Duration;
            Vector3 newLocation = Vector3.Lerp(startPosition, movePosition, evaluation);
            newLocation = NavMeshManager.Instance.FindClosestNavMeshPosition(newLocation);
            boss.agent.Warp(newLocation);
            if (timeElapsed > Duration)
            {
                Exit();
            }
        }
    }

    public void Exit()
    {
        timeElapsed = 0;
        OnFinish?.Invoke();
    }
}
