using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class OrcEnemy : Enemy
{
    [SerializeField] GameObject weaponTrail;
    [SerializeField] float attackStateTime = 6f;
    [SerializeField] float movementSpeedWhileAttacking = 5f;
    [SerializeField] float radiusOnAttack = 6f;
    [SerializeField] LayerMask playerLayer;
    float timePassedAttacking = 0;
    float directionChangeTimer = 0;
    private readonly int ATTACK_HASH = Animator.StringToHash("isAttacking");
    private readonly string AttackAnimName = "Attack02";
    Vector3 targetAttackLocation;
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
        HandleWeaponTrail();

        if (isAttacking)
        {
            MoveAndAttack();
        }
    }
    protected override void Attack()
    {
        isAttacking = true;
        animator.speed = 1f;
        animator.SetBool(ATTACK_HASH, true);
        SetNewAttackLocation();
        weaponTrail.SetActive(true);
        StartCoroutine(DamageCheck());
    }  

    private IEnumerator DamageCheck()
    {
        int checkCount = 20;
        float wait = attackStateTime / checkCount;
        for (int i = 0; i < checkCount; i++)
        {
            Collider[] colls = Physics.OverlapSphere(transform.position, radiusOnAttack, playerLayer);
            foreach (var coll in colls)
            {
                if(coll.TryGetComponent(out PlayerHealth playerHealth))
                {
                    playerHealth.TakeDamage((int)damage);
                }
            }
            yield return new WaitForSeconds(wait);
        }
    }

    private void MoveAndAttack()
    {
        agent.isStopped = false;
        agent.SetDestination(targetAttackLocation);
        agent.speed = movementSpeedWhileAttacking;
        
        timePassedAttacking += Time.deltaTime;
        directionChangeTimer += Time.deltaTime;

        if (directionChangeTimer > attackStateTime / 3f)
        {
            directionChangeTimer = 0f;
            //Change attack direction
            SetNewAttackLocation();

        }
        if (timePassedAttacking > attackStateTime)
        {
            EndAttack();
        }
    }

    private void EndAttack()
    {
        // end the attack                
        ResetAttackTimer();
        HandleAttackEnd();
        animator.SetBool(ATTACK_HASH, false);
        timePassedAttacking = 0f;
        weaponTrail.SetActive(false);
    }
    private void HandleWeaponTrail()
    {
        AnimatorClipInfo[] animatorClipInfos = animator.GetCurrentAnimatorClipInfo(0);
        weaponTrail.SetActive(animatorClipInfos[0].clip.name == AttackAnimName);
    }

    private void SetNewAttackLocation()
    {
        targetAttackLocation = NavMeshManager.Instance.FindClosestNavMeshPosition(PlayerHealth.Instance.transform.position);
        if(targetAttackLocation == null)
        {
            targetAttackLocation = transform.position;
        }
//        FaceToTarget(targetAttackLocation);
    }
     
}
