using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossAttackBehaviour : MonoBehaviour
{
    protected Boss boss;
    [SerializeField] protected bool useAnimation = true;
    public abstract void Attack();
    public virtual void ResetSequence()
    {
        boss.FinishAttack();
        HKDebugger.LogWorldText("Attack finished", boss.transform.position + Vector3.up * 10);
    }
    public virtual void SetBoss(Boss boss)
    {
        this.boss = boss;
    }
}
