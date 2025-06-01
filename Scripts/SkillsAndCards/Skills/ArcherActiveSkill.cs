using MoreMountains.Feedbacks;
using System.Collections;
using UnityEngine;

public class ArcherActiveSkill : MonoBehaviour , IActiveSkill
{
    private PlayerStateMachine playerStateMachine;
    private Shooter shooter;
    private HapticUser hapticUser;
    [SerializeField] Projectile projectile;
    [SerializeField] GameObject areaVisual;
    private ObjectPooler<Projectile> projectilePooler;
    [SerializeField] MMF_Player shootFeedBacks;
    [SerializeField] MMF_Player startFeedbacks;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] float timeBeforeShootStart = 0.25f;
    [SerializeField] float timeInAir = 1f;
    [SerializeField] float projectileRotationZ = -45f;
    public int ShootCount;
    public int ArrowCount;
    public int DamagePerArrow;
    public int DamageOnArea;
    public float radius = 7f;

    private void Awake()
    {
        hapticUser = GetComponent<HapticUser>();
    }

    private void Start()
    {
        projectilePooler = new ObjectPooler<Projectile>();
        projectilePooler.InitializeObjectPooler(projectile, null, 100);
    }

    public void SetSkillData(int level, int abilityDamage)
    {
        this.DamagePerArrow = DamagePerArrow * level + abilityDamage;
    }

    public void SetPlayerScript(PlayerStateMachine stateMachine)
    {
        this.playerStateMachine = stateMachine;
        shooter = stateMachine.selectedCharacter.GetComponent<Shooter>();
        StartCoroutine(ArcherSkillRoutine());                 
    }

    private IEnumerator ArcherSkillRoutine()
    {
        GameObject area_visual = Instantiate(areaVisual, playerStateMachine.transform.position + Vector3.up * 0.2f, Quaternion.identity);
        startFeedbacks?.PlayFeedbacks();
        Vector3 contactPoint = GridManager.Instance.GetClosestGridOnLocation(PlayerHealth.Instance.transform.position).Position;
        CheckEnemy(contactPoint);

        CameraController.Instance.ChangeLookTarget(playerStateMachine.selectedCharacter.transform);
        CameraController.Instance.ChangeFollowTarget(playerStateMachine.selectedCharacter.transform);

        float lowBefore, highBefore;
        PlayStartHaptics(out lowBefore, out highBefore);
        yield return new WaitForSeconds(timeBeforeShootStart);


        int arrow_count = ArrowCount;
        float delayBetweenShoots = timeInAir / ShootCount;

        for (int i = 0; i < ShootCount; i++)
        {
            Vector3 shootDirection = GetShootDirection();
            shootFeedBacks?.PlayFeedbacks();
            arrow_count++;
            ShootAsCircle(shootDirection, arrow_count);

            yield return new WaitForSeconds(delayBetweenShoots);

            PlayContinuousHaptics(lowBefore, highBefore);
        }
        Destroy(area_visual);
        CameraController.Instance.ChangeLookTarget(playerStateMachine.transform);
        CameraController.Instance.ChangeFollowTarget(playerStateMachine.transform);
    }

    private void PlayStartHaptics(out float lowBefore, out float highBefore)
    {
        lowBefore = hapticUser.lowFrequency;
        highBefore = hapticUser.highFrequency;
        if (hapticUser)
        {
            hapticUser.lowFrequency = 0.4f;
            hapticUser.highFrequency = 0.6f;
            hapticUser.Play();
        }
    }

    private void PlayContinuousHaptics(float lowBefore, float highBefore)
    {
        if (hapticUser)
        {
            hapticUser.lowFrequency = lowBefore;
            hapticUser.highFrequency = highBefore;
            hapticUser.Play();
        }
    }

    private Vector3 GetShootDirection()
    {
        Vector3 shootDirection = Vector3.zero;
        if (playerStateMachine.GetClosestEnemy() != null)
        {
            shootDirection = playerStateMachine.GetClosestEnemy().transform.position - playerStateMachine.transform.position;
        }
        else
        {
            shootDirection = playerStateMachine.transform.forward;
        }
        shootDirection.y = 0f;
        return shootDirection;
    }

    private void CheckEnemy(Vector3 damageCenter)
    {
        Collider[] enemies = Physics.OverlapSphere(damageCenter, radius , enemyLayer);
        foreach (Collider c in enemies)
        {
            if(c.TryGetComponent(out EnemyHealth enemyHealth))
            {
                enemyHealth.TakeDamage(DamageOnArea , Vector3.zero , true);
            }
        }
    }

    private void ShootAsCircle(Vector3 firstDir , int arrowCount)
    {
        Vector3 height = Vector3.up * 1.8f;
        Vector3 spawnPosition = playerStateMachine.transform.position + height;
        Vector3 direction = Quaternion.AngleAxis(projectileRotationZ, Vector3.forward) * firstDir;

        float angle = 360 / arrowCount;

        for (int i = 0; i < arrowCount; i++)
        {
            Projectile arrow = projectilePooler.GetObject();
            arrow.transform.position = spawnPosition;
            Vector3 shootDir = Quaternion.AngleAxis(angle * i, Vector3.up) * direction;
            arrow.Activate();
            arrow.SetDirection(shootDir);
            arrow.SetDamage(DamagePerArrow);
        }
    }
}
