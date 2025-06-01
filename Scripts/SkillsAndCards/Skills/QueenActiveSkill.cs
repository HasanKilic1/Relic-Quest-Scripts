using MoreMountains.Feedbacks;
using System;
using System.Collections;
using UnityEngine;

public class QueenActiveSkill : MonoBehaviour , IActiveSkill
{    
    PlayerStateMachine playerStateMachine;
    Transform shootPosition;
    [Header("Projectile")]
    [SerializeField] Projectile projectile;
    Transform projectileSpawn;
    [SerializeField] AnimationCurve moveCurve;
    [SerializeField] private float moveDistance;
    [SerializeField] private float moveDuration;

    [Header("Explosion")]
    [SerializeField] private float explosionRadius;
    [SerializeField] private int damagePerLevel;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private GameObject explosionVfx;

    [Header("Regeneration")]
    [SerializeField] private GameObject regenerationVfx;
    [Range(0, 10)][SerializeField] private int regenHealthPerLevel;

    [Header("Timers")]
    [SerializeField] private float waitBeforeProjectile;
    [SerializeField] private float waitBeforeExplosion;
    [SerializeField] private float waitBeforeRegenerate;

    [Header("Feedbacks")]
    [SerializeField] private MMF_Player explosionFeedbacks;
    [SerializeField] private MMF_Player shootFeedbacks;
   
    private int currentDamage;
    private int currentRegeneration;

    public void SetPlayerScript(PlayerStateMachine stateMachine)
    {
        playerStateMachine = stateMachine;
        projectileSpawn = playerStateMachine.selectedCharacter.GetComponent<Shooter>().ShootPosition;
        Use();
    }

    public void SetSkillData(int level, int abilityDamage)
    {
        currentDamage = damagePerLevel * level + abilityDamage;
        currentRegeneration = regenHealthPerLevel * level;
    }

    private void Use()
    {
        GameObject expl = Instantiate(explosionVfx , playerStateMachine.transform.position + Vector3.up * 0.2f , explosionVfx.transform.rotation);
        Destroy(expl , 2f);
        Invoke(nameof(Shoot), waitBeforeProjectile);
        Invoke(nameof(CheckExplosion), waitBeforeExplosion);
        Invoke(nameof(Regenerate), waitBeforeRegenerate);
    }

    private void Shoot()
    {
        Projectile _projectile = Instantiate(projectile, projectileSpawn.position, Quaternion.identity);
        _projectile.SetDirection(projectileSpawn.forward);
        _projectile.SetDamage(currentDamage * 5);
        shootFeedbacks?.PlayFeedbacks();
        HapticManager.instance.Impulse(0.5f, 0.5f, 0.05f);
        StartCoroutine(MoveBack());
    }

    private void Regenerate()
    {
        GameObject vfx = Instantiate(regenerationVfx, playerStateMachine.transform.position + Vector3.up * 0.2f, regenerationVfx.transform.rotation);
        Destroy(vfx , 2f);

        int increase = (int)(PlayerHealth.Instance.GetMaxHealth * currentRegeneration / 100);
        PlayerHealth.Instance.IncreaseHealth(increase, playFeedbacks: true);
    }
    private void CheckExplosion()
    {
        Collider[] colls = Physics.OverlapSphere(playerStateMachine.transform.position, explosionRadius);
        foreach (var collider in colls)
        {
            if(collider.TryGetComponent(out EnemyHealth enemyHealth))
            {
                enemyHealth.TakeDamage(currentDamage, Vector3.zero, true);
            }
        }
        HapticManager.instance.Impulse(0.3f, 0.5f, 0.15f);
        explosionFeedbacks?.PlayFeedbacks();
    }

    private IEnumerator MoveBack()
    {
        Vector3 start = playerStateMachine.transform.position;
        Vector3 dest = playerStateMachine.transform.position - playerStateMachine.selectedCharacter.transform.forward.normalized * moveDistance;
        dest.y = start.y;
        CharacterController characterController = playerStateMachine.GetComponent<CharacterController>();

        yield return new WaitForSeconds(0.2f);
       
        float t = 0f;
        while (t < moveDuration)
        {
            t+= Time.deltaTime;
            float evaluated = moveCurve.Evaluate(t / moveDuration);
            Vector3 currentPos = Vector3.Lerp(start, dest, evaluated);

            Vector3 difference = currentPos - characterController.transform.position;
            characterController.Move(difference);
            yield return null;
        }
    }
}
