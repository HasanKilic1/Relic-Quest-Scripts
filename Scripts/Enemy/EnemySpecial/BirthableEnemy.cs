using MoreMountains.Feedbacks;
using System.Collections;
using UnityEngine;

public class BirthableEnemy : Enemy
{
    [SerializeField] GameObject copy;
    [SerializeField] LineRenderer laser;
    [SerializeField] Transform shootPos;
    [Range(0.5f, 1f)][SerializeField] float scaleMultiplier;
    private EnemyHealth health;
    private int currentBirthCount;
    private int maxBirthCount = 2;
    private bool canMove = true;
    [SerializeField] MMF_Player feedbacks;
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        health = GetComponent<EnemyHealth>();

        laser.enabled = false;
    }

    protected override void Update()
    {
        base.Update();
        if (isFreezed && !canMove) { return; }
        HandleStates();
       
    }
    public void SetBirthCount(int birthCount)
    {
        this.currentBirthCount = birthCount;
    }
    protected override void Attack()
    {
       StartCoroutine(AttackRoutine());
    }

   
    private void TryGiveBirth()
    {
        if(currentBirthCount < maxBirthCount)
        {
            Vector3 scale = transform.localScale * scaleMultiplier;
            currentBirthCount++;
            feedbacks.SkipToTheEnd();
            for (int i = 0; i < 2; i++)
            {
                BirthableEnemy breedingEnemy = Instantiate(copy, transform.position + transform.forward * i, Quaternion.identity).GetComponent<BirthableEnemy>();
                breedingEnemy.SetBirthCount(currentBirthCount);
                breedingEnemy.transform.localScale = scale;
            }                        
        }
        Destroy(gameObject);
    }
    private IEnumerator AttackRoutine()
    {
        animator.SetBool("isMoving", false);
        agent.isStopped = true;
        animator.SetBool("isAttacking" , true);
        canMove = false;
        float t = 0f;
        float damageTimer = 0f;
        float targetTime = 2f;
        

        yield return new WaitForSeconds(1f);

        while(t < targetTime)
        {
            canMove = false;
            t += Time.deltaTime;
            damageTimer += Time.deltaTime;  
           
            float normalizedT = t / targetTime;
            transform.forward = Vector3.Lerp(transform.forward, (playerTargetedPosition.position - transform.position).normalized, Time.deltaTime * 0.5f);
            Vector3 targetedPos = shootPos.position + transform.forward * attackRange / (normalizedT + 0.01f);
            Vector3 extendedPos = Vector3.Lerp(shootPos.position , targetedPos, normalizedT);      
            
            DrawLaser(shootPos.position , extendedPos);
            if (damageTimer > 0.5f)
            {
                damageTimer = 0f;
                TryGiveDamage(targetedPos);
            }
            yield return null;            
        }
        
        animator.SetBool("isAttacking", false);
        ResetAttackTimer();
        laser.enabled = false;
        canMove = true;
    }

    private void DrawLaser(Vector3 start  ,Vector3 end)
    {
        laser.enabled = true;
        Vector3 laserEndPos = end;
        laser.SetPosition(0, start);
        laser.SetPosition(1, end);     
    }

    private void TryGiveDamage(Vector3 targetPosition)
    {
        Vector3 diff = targetPosition - shootPos.position;
        Ray ray = new Ray(shootPos.position, diff);
        if(Physics.Raycast(ray , out RaycastHit hitInfo , attackRange)) 
        {
            if(hitInfo.collider.TryGetComponent<PlayerHealth>(out var health))
            {
                health.TakeDamage((int)damage);
            }
        }
    }
}
