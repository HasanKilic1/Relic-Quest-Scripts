using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WolfBoss : Enemy
{
    private enum WolfAttackType
    {
        Ranged,
        Bulldozer,
        None
    }

    private WolfAttackType attackType;
    private float attack3Time = 8f;
    private int ranged_Hash = Animator.StringToHash("Ranged"); // triggered attack
    private int bulldozer_Hash = Animator.StringToHash("isBulldozer"); // continuitive attack

    [SerializeField] EnemyProjectile rangedAttackProjectile;
    [SerializeField] GameObject damageAreaPrefab;    

    [Range(0,100)] [SerializeField] float attack1Chance = 50f;
    [Range(0, 100)][SerializeField] float attack2Chance = 50f;
    float timePassed;

    private void OnValidate()
    {
        attack2Chance = 100 - attack1Chance;
    }
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        ChooseAttack();
    }

    protected override void Update()
    {
        base.Update();
        if (isAttacking)
        {
            switch (attackType)
            {
                case WolfAttackType.Ranged:
                    Attack1Update();
                    break;              
                case WolfAttackType.Bulldozer:
                    Attack2Update();
                    break;
                case WolfAttackType.None:
                    break;
            }
        }
        else
        {
            HandleStates();
        }
    }

    protected override void Attack()
    {
        Stop();

        
        switch (attackType)
        {
            case WolfAttackType.Ranged:
                OnAttack1Started();
                break;
            
            case WolfAttackType.Bulldozer:
                OnAttack2Started();
                break;
            case WolfAttackType.None:
                break;
        }

        if (attackType == WolfAttackType.Bulldozer)
        {
            return;
        }
        else StartCoroutine(AttackRoutine(3f));
    }

    private IEnumerator AttackRoutine(float time)
    {
        yield return new WaitForSeconds(time);
        HandleAttackEnd();
        ChooseAttack();
    }

    private void ChooseAttack()
    {
        float rand = UnityEngine.Random.Range(0, 100f);

        if (rand < attack1Chance) attackType = WolfAttackType.Ranged; 
        else attackType = WolfAttackType.Bulldozer;
    }
    
    private void OnAttack1Started()
    {
        animator.SetTrigger(ranged_Hash);
        StartCoroutine(ProjecileRoutine());
    }

    private IEnumerator ProjecileRoutine()
    {
        float timeBetweenSpawns = 0.3f;

        List<GameObject> areas = new List<GameObject>();
        Vector3 playerLocation = PlayerHealth.Instance.transform.position;
        float radius = 8f;
        int projectileCount = 8;
        float angle = 360f / projectileCount;
        float height = 10f;

        Vector3 centerPoint = new Vector3(playerLocation.x, height, playerLocation.z);
        EnemyProjectile centerProjectile = Instantiate(rangedAttackProjectile, centerPoint, Quaternion.identity);
        centerProjectile.Shoot(-Vector3.up);

        GameObject centerArea = Instantiate(damageAreaPrefab, centerPoint - Vector3.up * height, Quaternion.identity);
        areas.Add(centerArea);

        Vector3 spawnPoint = centerPoint + radius * Vector3.forward;

        for (int i = 0; i < projectileCount; i++)
        {
            EnemyProjectile projectile = Instantiate(rangedAttackProjectile, spawnPoint, Quaternion.identity);
            GameObject damageArea = Instantiate(damageAreaPrefab, spawnPoint - Vector3.up * height, Quaternion.identity);
            areas.Add(damageArea);

            projectile.Shoot(-Vector3.up);
            
            Vector3 rotated = spawnPoint - centerPoint;
            rotated = Quaternion.AngleAxis(angle, Vector3.up) * rotated;
            spawnPoint = centerPoint + rotated;

            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        yield return null;

        foreach (var item in areas)
        {
            Destroy(item);
        }
        areas.Clear();
    }

   
    private void OnAttack2Started()
    {
        animator.SetBool(bulldozer_Hash, true);
    }

   
    private void Attack1Update()
    { }
  
    private void Attack2Update()
    {
        timePassed += Time.deltaTime;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(diffToPlayer.normalized),Time.deltaTime * 3f);
        
        if (timePassed > attack3Time)
        {
            timePassed = 0f;
            animator.SetBool(bulldozer_Hash, false);
            HandleAttackEnd();
            ChooseAttack();
        }
    }

    public float Damage => damage;
}
