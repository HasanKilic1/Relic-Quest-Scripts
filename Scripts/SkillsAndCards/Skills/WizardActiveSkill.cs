using MoreMountains.Feedbacks;
using System.Collections;
using UnityEngine;

public class WizardActiveSkill : MonoBehaviour , IActiveSkill
{
    [SerializeField] GameObject shieldPrefab;
    [SerializeField] GameObject shockWavePrefab;
    [SerializeField] int baseDamage = 10;
    int currentDamage;
    [SerializeField] float damageRadius = 30f;
    [SerializeField] float shockWaveSpawnDelay = 1.2f;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] MMF_Player feedBacks;
    GameObject shield;
    PlayerStateMachine playerStateMachine;

    private void OnEnable()
    {
        SkillUser.OnAbilityFinished += DestroyShield;
    }
    private void OnDisable()
    {
        SkillUser.OnAbilityFinished -= DestroyShield;
    }
    
    public void SetSkillData(int level, int abilityDamage)
    {
        currentDamage = level * baseDamage + abilityDamage;
    }

    public void SetPlayerScript(PlayerStateMachine stateMachine)
    {
        StartCoroutine(SkillRoutine(stateMachine));
        this.playerStateMachine = stateMachine;
    }

    private void Update()
    {
        if(shield != null && playerStateMachine != null)
        {
            shield.transform.position = this.playerStateMachine.selectedCharacter.transform.position;
        }
    }

    private IEnumerator SkillRoutine(PlayerStateMachine stateMachine)
    {
        shield = Instantiate(shieldPrefab , stateMachine.selectedCharacter.transform.position , Quaternion.identity);
        yield return new WaitForSeconds(shockWaveSpawnDelay);
        GameObject shockWave = Instantiate(shockWavePrefab, stateMachine.selectedCharacter.transform.position, Quaternion.Euler(-90,0,0));
        HapticManager.instance.Impulse(0.5f, 1f, 0.3f);
        CheckDamage(stateMachine);
        feedBacks.PlayFeedbacks();
    }

    private void CheckDamage(PlayerStateMachine playerStateMachine)
    {
        Collider[] enemyColls = Physics.OverlapSphere(playerStateMachine.transform.position, damageRadius , enemyLayer);
        foreach (Collider col in enemyColls)
        {
            if(col.TryGetComponent(out EnemyHealth enemyHealth))
            {
                Vector3 diff = enemyHealth.transform.position - playerStateMachine.transform.position;
                enemyHealth.TakeDamage(currentDamage, hitDirection:diff, isUnstoppableAttack:true);
            }
        }
    }

    private void DestroyShield()
    {
        Destroy(shield);
    }

}
