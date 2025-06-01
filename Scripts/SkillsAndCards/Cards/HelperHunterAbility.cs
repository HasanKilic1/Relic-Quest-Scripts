using MoreMountains.Feedbacks;
using System.Collections;
using UnityEngine;

public class HelperHunterAbility : MonoBehaviour , IRelic
{
    PlayerStateMachine playerStateMachine;
    [SerializeField] LayerMask enemyLayer;
    [Range(0,100)][SerializeField] private int damageRatio = 33;
    [SerializeField] private float damageDelay;
    [SerializeField] private float radius;
    [SerializeField] Transform aimZone;
    [SerializeField] float followSpeed = 40f;
    [SerializeField] float coolDown = 5f;
    [SerializeField] GameObject shootVfx;
    [SerializeField] MMF_Player shootFeedbacks;
    Transform targetEnemy = null;
    MeshRenderer aimZoneMesh;
    private float timeElapsedFollowing;
    private bool isActivated = false;
    private float timeToActivate;
    float aimZoneYPos = 0.3f;
    public RelicSO RelicSO { get; set;}
    private void Awake()
    {
        aimZoneMesh = aimZone.GetComponent<MeshRenderer>();
    }
    void Start()//Test
    {
        SettleEffect(PlayerController.Instance.GetComponent<PlayerStateMachine>());
    }

    void Update()
    {
        if(Time.time > timeToActivate && !isActivated)
        {
            Activate();
        }
        if(isActivated)
        {
            GoClosestEnemy();
        }
        aimZoneMesh.enabled = CanFollow();
    }

    public void ResetEffect(PlayerStateMachine anyPlayerScript)
    {
        
    }

    public void SettleEffect(PlayerStateMachine stateMachine)
    {
        this.playerStateMachine = stateMachine;
    }

    private void Activate()
    {
        isActivated = true;
        aimZone.gameObject.SetActive(true);
        aimZone.position = playerStateMachine.transform.position;
        timeElapsedFollowing = 0f;
    }


    private void GoClosestEnemy()
    {
        if (CanFollow())
        {
            Vector3 targetPos = targetEnemy.transform.position;
            targetPos.y = aimZoneYPos;
            timeElapsedFollowing += Time.deltaTime;
            aimZone.position = Vector3.Lerp(aimZone.position,new Vector3(targetEnemy.position.x , aimZoneYPos , targetEnemy.position.z) , followSpeed * Time.deltaTime);

            if(timeElapsedFollowing > 5f)
            {
                Shoot();
            }
        }
    }

    private Transform GetClosestEnemy()
    {
        Transform closestEnemy = null;
        var allEnemies = FindObjectsOfType<EnemyHealth>();
        float closestDistance = 100000f;

        foreach (EnemyHealth enemy in allEnemies)
        {
            float distance = Vector3.Distance(enemy.transform.position, playerStateMachine.transform.position);
            if (distance < closestDistance)
            {
                closestEnemy = enemy.transform;
                closestDistance = distance;
            }
        }
        return closestEnemy;
    }

    private void Shoot()
    {
        shootFeedbacks?.PlayFeedbacks();
        Stop();
        StartCoroutine(ShootRoutine());
    }    

    private IEnumerator ShootRoutine()
    {
        GameObject vfx = Instantiate(shootVfx , aimZone.position , Quaternion.identity);
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(damageDelay);
            GiveDamageOnArea();
        }

    }
    protected void GiveDamageOnArea()
    {
        Collider[] colls = Physics.OverlapSphere(aimZone.position, radius, enemyLayer);
        foreach (Collider coll in colls)
        {
            if (coll.TryGetComponent(out EnemyHealth enemyHealth))
            {
                int damage = enemyHealth.GetMaxHealth() * damageRatio / 100;
                enemyHealth.TakeDamage(damage, Vector3.zero, isUnstoppableAttack: true);
            }
        }
    }
    private void Stop()
    {
        aimZone.gameObject.SetActive(false);
        isActivated = false;
        timeToActivate = Time.time + coolDown;
    }

    private bool CanFollow()
    {
        targetEnemy = GetClosestEnemy()?.transform;
        return targetEnemy != null && isActivated;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
