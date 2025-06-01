using System.Collections;
using UnityEngine;

public class Worm : Enemy
{
    EnemyHealth enemyHealth;
    [SerializeField] Transform shootPos;
    [SerializeField] Trajectory trajectory;
    [SerializeField] float timeBeforeShoot;
    [SerializeField] float diveInDuration = 1f;
    [SerializeField] float getOutDuration = 1f;
    [SerializeField] float waitUnderGround = 2f;
    private bool isChangingLocation = false;   
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
        FaceToTarget(playerTargetedPosition.position);
        if (canAttack && !isChangingLocation)
        {
            Attack();
        }        
    }

    protected override void Attack()
    {   
        StartCoroutine(AttackRoutine());       
    }
    private IEnumerator AttackRoutine()
    {
        ResetAttackTimer();
        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(timeBeforeShoot);

        Vector3 randomPos = GetRandomPosNearPlayer();
        Trajectory _trajectory = Instantiate(trajectory, shootPos.position, Quaternion.identity);
        _trajectory.SetDamage(damage);
        _trajectory.Shoot(randomPos);
        yield return new WaitForSeconds(2f);
        ChangeLocation();
        HandleAttackEnd();
    }

    private void ChangeLocation()
    {
        StartCoroutine(DiveInGoOutRoutine());
    }

    private IEnumerator DiveInGoOutRoutine()
    {   
        isChangingLocation = true;
        animator.SetTrigger("DiveIn");
        yield return new WaitForSeconds(diveInDuration);

        enemyHealth.GetHealthBar().gameObject.SetActive(false);
        isTargetable = false;

        yield return new WaitForSeconds(waitUnderGround);

        animator.SetTrigger("GetOut");
        ChangePosition();

        yield return new WaitForSeconds(getOutDuration);

        
        enemyHealth.GetHealthBar().gameObject.SetActive(true);
        isChangingLocation = false;
        isTargetable = true;
    }
    private void ChangePosition()
    {
        Vector3 randomPosition = GridManager.Instance.GetEmptyPositions()[Random.Range(0, GridManager.Instance.GetEmptyPositions().Count)];
        transform.position = randomPosition;
    }
    private Vector3 GetRandomPosNearPlayer()
    {
        float randomDiff = Random.Range(-2f, 2f);
        Vector3 randomPosition = PlayerHealth.Instance.transform.position + new Vector3(randomDiff, 0f, randomDiff);
        return randomPosition;
    }

}
