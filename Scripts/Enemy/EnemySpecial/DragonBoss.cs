using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonBoss : Enemy
{
    [Header("Attack 1")]
    [SerializeField] EnemyProjectile flameBallProjectile;
    [SerializeField] Transform shootPos;
    [Header("Attack 2")]
    [SerializeField] GameObject fireArea;
    private float lastAreaHeight = 0.5f;
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
        HandleStates();
    }
    protected override void Attack()
    {
        animator.SetBool("isMoving", false);
        agent.isStopped = true;
        if(UnityEngine.Random.Range(-1f,1f) < 0)
        {
            StartCoroutine(FlameAttackRoutine());
        }
        else StartCoroutine(FlameBallAttackRoutine());  
    }

    private IEnumerator FlameAttackRoutine()
    {
        animator.SetBool("isAttack1", true);
        int projectileCount = 6;
        yield return new WaitForSeconds(0.5f);
        //Vector3 diffToPlayer = PlayerHealth.Instance.transform.position - shootPos.position;

        for (int i = 0; i < projectileCount; i++)
        {
            Vector3 dir = shootPos.forward;
            dir.y = 0f;
            EnemyProjectile projectile = Instantiate(flameBallProjectile, shootPos.position , Quaternion.identity);            
            projectile.Shoot(dir);
            projectile.SetManeuverable(true);
            yield return new WaitForSeconds(0.3f);
        }

        animator.SetBool("isAttack1", false);
        ResetAttackTimer();
    }

    private IEnumerator FlameBallAttackRoutine()
    {
        animator.SetBool("isAttack2", true);
        int areaCount = 5;
        for (int i = 0; i < areaCount; i++)
        {   
            GameObject area = Instantiate(fireArea , GetRandomPointNearPlayer() , Quaternion.identity);
            yield return new WaitForSeconds(timeBeforeAttack / areaCount);
        }
        animator.SetBool("isAttack2", false);
        ResetAttackTimer();
    }
    private Vector3 GetRandomPointNearPlayer()
    {
        Vector3 playerPos = PlayerHealth.Instance.transform.position;
        float max = 6f; 
        float randX = UnityEngine.Random.Range(-max, max);
        float randZ = UnityEngine.Random.Range(-max , max);

        if (randX > -max/3f && randX < max/3f) randX *= 2;
        if (randZ > -max/3f && randZ < max/3f) randZ *= 2;
        lastAreaHeight += 0.03f;
        return new Vector3(playerPos.x + randX , lastAreaHeight , playerPos.z + randZ);
    }
   
}
