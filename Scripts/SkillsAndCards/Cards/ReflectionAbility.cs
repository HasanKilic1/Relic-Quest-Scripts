using MoreMountains.Feedbacks;
using System;
using UnityEngine;

public class ReflectionAbility : MonoBehaviour , IRelic
{
    private static readonly string POOL_NAME = "REFLECTION ABILITY";
    PlayerStateMachine playerStateMachine;
    public RelicSO RelicSO { get; set; }

    [Range(0,100)][SerializeField] private int reflectedDamageRatioBase;
    [SerializeField] private float coolDown = 2f;
    [SerializeField] private GameObject HitVfx;
    [SerializeField] MMF_Player reflectionFeedbacks;
    private float validTime;

    private void OnEnable() => PlayerHealth.OnDamageTaken += Reflect;
    private void OnDisable() => PlayerHealth.OnDamageTaken -= Reflect;

    private void Start()//TEST
    {
        SettleEffect(PlayerHealth.Instance.GetComponent<PlayerStateMachine>());
        SceneObjectPooler.Instance.CreatePool(POOL_NAME, HitVfx, 5);
    }

    public void ResetEffect(PlayerStateMachine playerStateMachine)
    {
    }
    public void SettleEffect(PlayerStateMachine playerStateMachine)
    {

        this.playerStateMachine = playerStateMachine;
    }

    private void Reflect(float takenDamage)
    {
        if (!CanUse) return;
        reflectionFeedbacks?.PlayFeedbacks();
        validTime = Time.time + coolDown;
        int damage = (int)(takenDamage * reflectedDamageRatioBase / 100);
        EnemyHealth enemyHealth = GetClosestEnemy();
        if(enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage, Vector3.down, isUnstoppableAttack: true);
            // GameObject blast = Instantiate(HitVfx, enemyHealth.transform.position , HitVfx.transform.rotation);
            //Destroy(blast , 1f);
            GameObject blast = SceneObjectPooler.Instance.GetObjectFromPool(POOL_NAME, HitVfx);
            blast.transform.position = enemyHealth.transform.position + Vector3.up * 0.25f;           
        }
    }

    private EnemyHealth GetClosestEnemy()
    {
        EnemyHealth closestEnemy = null;
        var allEnemies = FindObjectsOfType<EnemyHealth>();
        float closestDistance = 100000f;

        foreach (EnemyHealth enemy in allEnemies)
        {
            float distance = Vector3.Distance(enemy.transform.position , playerStateMachine.transform.position);
            if(distance < closestDistance)
            {
                closestEnemy = enemy;
                closestDistance = distance; 
            }
        }
        return closestEnemy;
    }

    private bool CanUse => Time.time > validTime;

    private void OnDestroy()
    {
        
    }
}
