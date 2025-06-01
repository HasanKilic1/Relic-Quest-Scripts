using MoreMountains.Tools;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance;
    public static event Action OnPlayerDead;
    public static event Action<float> OnDamageTaken;
    public static event Action<float> OnHealthIncreased;
    [SerializeField] private UnityEvent OnDamage;
    [SerializeField] private UnityEvent OnRevive;

    private PlayerStateMachine playerStateMachine;
    private PlayerFeedbackController feedbackController;

    private float maxHealth;    
    private float currentHealth;
    public Transform TargetedPosition;
    private float defense;

    [SerializeField] MMProgressBar healthBar;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] GameObject shield;
    [SerializeField] GameObject reviveVfx;
    public bool CanTakeDamage = true;
    public bool isImmortal;
    private float invulnerabilityFinishTime;
    
    private void Awake()
    {
        if(Instance == null) Instance = this;
        playerStateMachine = GetComponent<PlayerStateMachine>();
        feedbackController = GetComponent<PlayerFeedbackController>();
    }

    public void TakeDamage(int damage)
    {
        if (isImmortal) return;
        if (!CanTakeDamage) return;

        float reducedDamage = damage - (damage * defense / 1000f);
        reducedDamage = Mathf.Max(0, reducedDamage);
        currentHealth -= reducedDamage;

        OnDamageTaken?.Invoke(reducedDamage);
        UpdateHealthBar();
        AchievementManager.Instance.EffectQuestByType(QuestType.TakenDamageFromEnemies, damage);

        if (currentHealth <= 0)
        {
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        CanTakeDamage = false;
        currentHealth = 1;

        OnPlayerDead?.Invoke();
        AchievementManager.Instance.EffectQuestByType(QuestType.TotalDeathCount, 1);

        playerStateMachine.inputClosed = true;
        playerStateMachine.GetIntoNewState(new DeathState(playerStateMachine));
    }

    public void Revive()
    {
        CanTakeDamage = false;
        currentHealth = maxHealth / 2;
        UpdateHealthBar();

        Invoke(nameof(MakeVulnerable) , 5f);
        shield.SetActive(true);

        playerStateMachine.inputClosed = false;
        playerStateMachine.GetIntoNewState(new ReviveState(playerStateMachine));
        
        OnRevive?.Invoke();
        
        Instantiate(reviveVfx , transform.position + Vector3.up * 0.5f , Quaternion.identity);
    }

    private void MakeVulnerable()
    {
        CanTakeDamage = true;
        shield.SetActive(false);
    }
    public void IncreaseHealth(int increase, bool playFeedbacks = false)
    {
        currentHealth += increase;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        UpdateHealthBar();
        OnHealthIncreased?.Invoke(increase);
        if (playFeedbacks)
        {
            feedbackController.PlayHealthIncraseFeedbacks(increase.ToString());
        }
    }


    public void SetStartHealth(float health) 
    {
        maxHealth = health;
        currentHealth = health;
        UpdateHealthBar(); 
    }

    public void InfluenceMaxHealth(float influence)
    {
        maxHealth += influence;
        currentHealth += influence;
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        healthBar.UpdateBar(currentHealth, 0f, maxHealth);
        UpdateText();
    }

    public void CloseHealthBar() => healthBar.gameObject.SetActive(false);
    public void OpenHealthBar() => healthBar.gameObject.SetActive(true);
    private void UpdateText()
    {
        healthText.text = ((int)currentHealth).ToString();
    }

    public void SetDefense(float defense)
    {
        this.defense = defense;
    }    
    public void InfluenceDefense(float influence)
    {
        defense += influence;
    }

    public int GetCurrentHealth() => (int)this.currentHealth;
    public float GetDefense => this.defense;
    public float GetMaxHealth => maxHealth;

    [ContextMenu("Kill Player")]
    public void KillPlayer()
    {
        HandleDeath();
    }

}
