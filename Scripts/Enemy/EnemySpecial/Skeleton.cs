using System.Collections;
using UnityEngine;

public class Skeleton : Enemy
{
    [field: SerializeField] public Transform ShootPos {  get; private set; }
    [field: SerializeField] public GameObject hook { get; private set; }
    [HideInInspector] public Vector3 shootDir;
    private readonly string attackAnimName1 = "SkeletonAttack1";
    private readonly string attackAnimName2 = "SkeletonAttack2";
    EnemyHealth enemyHealth;
    protected override void Awake()
    {
        base.Awake();
        enemyHealth = GetComponent<EnemyHealth>();
    }
    protected override void Start()
    {
        base.Start();
       
    }
    protected override void Update()
    {        
        base.Update();
       
        if (isFreezed) { return; }

        bool isVulnerable = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == attackAnimName1
                            || animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == attackAnimName2; // this enemy becomes vulnerable only when attacking
        enemyHealth.SetVulnerable(isVulnerable);
        HandleStates();

    }
    protected override void Attack()
    {
        Stop();
        animator.SetTrigger("Attack");
        canFaceAnyTarget = false;
        FaceToTarget(playerTargetedPosition.position);

        StartCoroutine(AttackRoutine(timeBeforeAttack));
    }
 
    private IEnumerator AttackRoutine(float duration)
    {
        
        hook.gameObject.SetActive(true);
        Vector3 startPos = hook.transform.position;
        Vector3 startScale = hook.transform.localScale;
       
        Vector3 targetpos = startPos + (transform.forward * attackRange) / 2;
        shootDir = targetpos - startPos;
        float targetZScale = Vector3.Distance(startPos, targetpos) / 5;

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            ExtendHook(startPos, startScale, targetpos, targetZScale, t);
            yield return null;
        }

        canFaceAnyTarget = true;
        hook.transform.localPosition = Vector3.zero;
        hook.transform.localScale = startScale;
        hook.gameObject.SetActive(false);

        ResetAttackTimer();
        HandleAttackEnd();
    }

    private void ExtendHook(Vector3 startPos, Vector3 startScale, Vector3 targetpos, float targetZScale, float t)
    {
        hook.transform.position = Vector3.Lerp(startPos, targetpos, t);

        hook.transform.localScale = Vector3.Lerp(startScale,
                                    new Vector3(hook.transform.localScale.x,
                                    hook.transform.localScale.y, targetZScale), t);
    }

    public float Damage => this.damage;
    public float Range => this.attackRange;
}
