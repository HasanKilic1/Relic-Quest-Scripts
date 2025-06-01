using MoreMountains.Feedbacks;
using UnityEngine;

public class RedDemonAnimController : MonoBehaviour
{
    public Boss boss;
    private EnemyHealth enemyHealth;
    [Header("Melee Attack")]
    [SerializeField] int attack1Damage;
    [SerializeField] float attack1Radius;
    [SerializeField] GameObject splashVfx;
    [SerializeField] Transform splashPosition;
    [SerializeField] GameObject cleave;
    [SerializeField] Transform projectileSpawnPos;
    [SerializeField] MMF_Player splashFeedbacks;
    [SerializeField] MMF_Player cleaveFeedbacks;

    [Header("Rise Attack")]
    [SerializeField] int attack2Damage;
    [SerializeField] int attack2Radius;    
    [SerializeField] MMF_Player fallFeedbacks;
    
    [SerializeField] LayerMask playerLayer;

    [Header("Gizmos")]
    public bool ShowAttack2Gizmo = true;

    private void Awake()
    {
        enemyHealth = boss.GetComponent<EnemyHealth>();
    }
    #region Melee Attack
    public void CreateSplash()
    {
        GameObject splash = Instantiate(splashVfx , splashPosition.position , splashVfx.transform.rotation);
        if(splashFeedbacks != null)
        {
            splashFeedbacks.PlayFeedbacks();
        }
        CheckPositionInRadius(splashPosition.position , attack1Radius , attack1Damage);
    }

    public void SendProjectile()
    {
        if (cleaveFeedbacks != null) { cleaveFeedbacks.PlayFeedbacks(); }

        Vector3 dirToPlayer = PlayerController.Instance.transform.position - boss.transform.position;
        Quaternion rotation = Quaternion.LookRotation(dirToPlayer);
        
        Instantiate(cleave, projectileSpawnPos.position, rotation);
        cleave.transform.forward = projectileSpawnPos.forward;
        Ray ray = new Ray(projectileSpawnPos.position, dirToPlayer);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10f, playerLayer))
        {
            if (hit.collider.TryGetComponent(out PlayerHealth playerHealth)) playerHealth.TakeDamage(attack1Damage);
        }
    }

    #endregion

    #region Rise Attack

    public void MakeInvulnerable()
    {
        enemyHealth.SetVulnerable(false);
    }
    public void MakeVulnerable()
    {
        enemyHealth.SetVulnerable(true);
    }

    public void Rise()
    {
        CameraController.Instance.ChangeLookTarget(transform);
    }
    public void Fall()
    {
        CheckPositionInRadius(boss.transform.position , attack2Radius , attack2Damage);
        if(fallFeedbacks) fallFeedbacks.PlayFeedbacks();

        boss.isTargetable = true;
        CameraController.Instance.ChangeLookTarget(PlayerHealth.Instance.transform);
    }

    #endregion
    private void CheckPositionInRadius(Vector3 position , float radius, int damage)
    {
        Collider[] colls = Physics.OverlapSphere(position, radius , playerLayer);

        foreach (var collider in colls)
        {
            if(collider.TryGetComponent(out PlayerHealth playerHealth))
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (ShowAttack2Gizmo)
        {
            Gizmos.DrawWireSphere(transform.position, attack2Radius);
        }
    }
}
