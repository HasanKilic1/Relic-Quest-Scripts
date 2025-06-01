using System.Linq;
using UnityEngine;

public class DwarfPassiveSkill : MonoBehaviour , IPassiveSkill
{
    // Reflect the enemy attacks when shield is active
    PlayerHealth playerHealth;
    PlayerStateMachine playerStateMachine;
    [SerializeField] private Projectile reflectedProjectile;
    [SerializeField] private int Level;
    [SerializeField] private int HealthPerLevel;
    [SerializeField] private int DamagePerLevel;
    [SerializeField] private int shootCooldown = 3;
    [SerializeField] private int shieldCooldown = 10;
    [SerializeField] GameObject shieldPrefab;
    private bool ShieldActive = true;
    private int currentShieldHealth;
    private float shootTimer;
    private GameObject shield;
    private ObjectPooler<Projectile> reflectedProjectilePooler;

    private void OnEnable()
    {
        PlayerHealth.OnDamageTaken += DecreaseShieldHealth;
        DwarfActiveSkill.OnSkillStarted += HideShieldVisualWhileSkillPlays;
        DwarfActiveSkill.OnSkillFinished += ShowShieldVisualOnSkillFinish;
    }

    private void OnDisable()
    {
        PlayerHealth.OnDamageTaken -= DecreaseShieldHealth;
        DwarfActiveSkill.OnSkillStarted -= HideShieldVisualWhileSkillPlays;
        DwarfActiveSkill.OnSkillFinished -= ShowShieldVisualOnSkillFinish;
    }

    
    private void Start()
    {
        reflectedProjectilePooler = new ObjectPooler<Projectile>();
        reflectedProjectilePooler.InitializeObjectPooler(reflectedProjectile, SceneObjectPooler.Instance.transform , 20);
    }

    private void Update()
    {
        if (CanShoot)
        {
            TrySendProjectile();
        }
    }

    public void SetPlayer(PlayerStateMachine player)
    {
        this.playerHealth = player.GetComponent<PlayerHealth>();
        playerStateMachine = player;
        shield = Instantiate(shieldPrefab, player.transform.position , shieldPrefab.transform.rotation);
        currentShieldHealth = Level * HealthPerLevel;
    }
    
    private void TrySendProjectile()
    {
        if (GetValidEnemy() != null)
        {
            Vector3 direction = GetValidEnemy().transform.position - playerStateMachine.transform.position;
            Vector3 spawnPosition = playerStateMachine.transform.position + direction.normalized * 2f; // add some range 
            spawnPosition.y = 4f;

            Projectile projectile = reflectedProjectilePooler.GetObject();
            projectile.transform.position = spawnPosition;
            projectile.SetDamage(Level * DamagePerLevel);
            projectile.SetDirection(direction.normalized);

            shootTimer = Time.time + shootCooldown;
        }
    }

    private Transform GetValidEnemy()
    {
        Transform closest = null;
        if(playerStateMachine.GetClosestEnemy()) closest = playerStateMachine.GetClosestEnemy().transform;
        else
        {
            var allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            var ordered = allEnemies.OrderBy(e => Vector3.Distance(e.transform.position , playerStateMachine.transform.position)).ToList();
            if(ordered.Count > 0)
            {
                closest = ordered[0].transform;
            }
        }
        return closest;
    }
    private void DecreaseShieldHealth(float decrease)
    {
        if (!ShieldActive) return;
        
        currentShieldHealth -= (int)decrease;
        playerHealth.IncreaseHealth((int)decrease);

        if(currentShieldHealth <= 1)
        {
            shield.SetActive(false);
            
            currentShieldHealth = 0;
            ShieldActive = false;
            Invoke(nameof(ShieldCooldown), shieldCooldown);
        }
    }
    private bool CanShoot => Time.time > shootTimer && ShieldActive;
    private void ShieldCooldown()
    {
        shield.SetActive(true);
        ShieldActive = true;
        currentShieldHealth = Level * HealthPerLevel;
    }

    private void ShowShieldVisualOnSkillFinish()
    {
        shield.SetActive(ShieldActive);
    }
    private void HideShieldVisualWhileSkillPlays()
    {
        shield.SetActive(false);
    }
}
