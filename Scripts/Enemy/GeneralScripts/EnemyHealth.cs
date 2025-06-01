using System;
using UnityEngine;
using MoreMountains.Feedbacks;

public class EnemyHealth : MonoBehaviour
{
    public static event Action<float, float> OnBossHealthChanged;
    public static event Action<EnemyHealth> OnDamageTaken;
    public event Action<int> OnHealthCurrencyChange;

    private Boss boss;
    [SerializeField] bool isBossHealth;
    private Enemy enemy;
    private ResourceDropper dropper;
    private Animator animator;
    EnemyWorldHealthBar healthBar;
    [SerializeField] int maxHealth;
    [SerializeField] int addHealthPerLevel = 20;
    int currentHealth;
    [SerializeField] MMF_Player damageFeedBack;
    [SerializeField] MMF_Player deathFeedBack;
    private readonly string[] deathTexts = new string[] { "DEAD" , "WOW" , "INCREDIBLE" , "BRUTAL"};

    private bool vulnerable = true;
    public bool IsShielded;
    [SerializeField] Vector3 healthBarOffset = Vector3.up;
    [SerializeField] Vector3 shieldOffset = Vector3.up / 2;
    private EnemyShield shield;
    private int shieldMaxHealth;
    private int shieldHealth;
    private bool isDead;
    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        boss = GetComponent<Boss>();
        animator = GetComponentInChildren<Animator>();
        dropper = GetComponent<ResourceDropper>();
    }

    private void Start()
    {
        if (!isBossHealth)
        {
            maxHealth += addHealthPerLevel * GameStateManager.Instance.CurrentLevel;
        }

        currentHealth = maxHealth;        
        if (!isBossHealth)
        {
            CallHealthBar();
        }
        else
        {
            OnBossHealthChanged?.Invoke(currentHealth, maxHealth);
        }
        Invoke(nameof(CallShield), 0.1f);
    }
    private void Update()
    {
        if (isDead)
        {
            Destroy(gameObject);
        }
    }
    private void CallHealthBar()
    {
        healthBar = SceneObjectPooler.Instance.GetEnemyHealthBar();
        healthBar.Setup(this);
        healthBar.SetFollowOffset(healthBarOffset);
        healthBar.UpdateHealthBar(currentHealth, maxHealth);
    }

    private void CallShield()
    {
        if(IsShielded)
        {
            shieldMaxHealth = maxHealth / 2;
            shieldHealth = shieldMaxHealth;
            shield = SceneObjectPooler.Instance.GetEnemyShield();
            shield.SetUp(this , shieldOffset);
        }
    }

    public void TakeDamage(int damage , Vector3 hitDirection, bool isUnstoppableAttack = false)
    {
        if (!vulnerable && !isUnstoppableAttack) return;

        if(IsShielded)
        {
            shieldHealth -= damage;
            shield.UpdateShield(shieldHealth, shieldMaxHealth);
            if (shieldHealth <= 0)
            {
                shieldHealth = 0;
                IsShielded = false;                
                shield.Deactivate();
            }           
        }

        else
        {
            currentHealth -= damage;
            currentHealth = Mathf.Max(currentHealth, 0);
            PlayHitFeedBacks(hitDirection , damage);
            OnHealthCurrencyChange?.Invoke(currentHealth);
            OnDamageTaken?.Invoke(this);

            if (isBossHealth)
            {
                OnBossHealthChanged?.Invoke(currentHealth, maxHealth);
            }

            if (currentHealth <= 0)
            {     
                currentHealth = 0;
                HandleDeath();
                return;
            }
           
        }              
    }

    public void HandleDeath(bool playFeedbacks = true)
    {
        if (isDead) return;

        AchievementManager.Instance.EffectQuestByType(QuestType.DestroyEnemies, 1);
        if (healthBar)
        {
            healthBar.UpdateHealthBar(0, maxHealth);
            healthBar.Inactivate();
        }
        EnemyPool.Instance.DecreaseEnemyCount();
        
        if (playFeedbacks) PlayDeathFeedbacks();
        isDead = true;
        Destroy(gameObject);
    }

    private void PlayHitFeedBacks(Vector3 hitDirection,int damage)
    {
        if (damageFeedBack != null)
        {
            float randomizedDamage = (10 * damage);
            damageFeedBack.GetFeedbacksOfType<MMF_FloatingText>()[0].Value = randomizedDamage.ToString("0");
            damageFeedBack.PlayFeedbacks();
            if (healthBar)
            {
                healthBar?.UpdateHealthBar(currentHealth, maxHealth);
            }
        }

        var hitVfx = SceneObjectPooler.Instance.GetDamageVfx();
        hitVfx.transform.position = GetTargetedPos().position;
        hitVfx.transform.forward = hitDirection.normalized;
    }

    private void PlayDeathFeedbacks()
    {
        deathFeedBack.GetFeedbackOfType<MMF_FloatingText>().Value = deathTexts[UnityEngine.Random.Range(0, deathTexts.Length)];
        deathFeedBack.PlayFeedbacks();
        if(dropper) dropper.Drop();
        GameObject groundVisual = SceneObjectPooler.Instance.GetDeathGroundVisual();
        groundVisual.transform.position = NavMeshManager.Instance.FindClosestNavMeshPosition(transform.position , 3f) + Vector3.up * 0.3f;
    }

    public int GetCurrentHealth()
    {
        return this.currentHealth;
    }
    public int GetMaxHealth()
    { 
        return this.maxHealth; 
    }
    public Transform GetTargetedPos()
    {        
        if(enemy == null)
        {
            return boss.TargetedPos;
        }
        else return enemy.targetedPosition;
    }
    public EnemyWorldHealthBar GetHealthBar()
    {
        return healthBar;
    }
    public void SetVulnerable(bool isvulnerable)
    {
        this.vulnerable = isvulnerable;
    }

    public bool IsTargetable()
    {
        if(enemy != null)
        {
            return enemy.isTargetable;
        }
        if(boss != null)
        {
            return boss.isTargetable;
        }
        return false;
    }
    private void OnDestroy()
    {
        if (healthBar)
        {
            if (healthBar.gameObject.activeInHierarchy)
            {
                healthBar.Inactivate();
            }
        }        
    }
}
