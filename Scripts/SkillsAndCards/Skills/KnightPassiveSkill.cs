using MoreMountains.Feedbacks;
using UnityEngine;

public class KnightPassiveSkill : MonoBehaviour, IPassiveSkill
{
    PlayerStateMachine player;
    Shooter shooter;
    PlayerHealth playerHealth;
    [SerializeField] float fillEnergyPerSecond;
    [SerializeField] float maxEnergy;
    [SerializeField] float spendEnergyPerShot;
    [SerializeField] float resetCooldown;
    [SerializeField] WorldProgressBar worldProgressBarPrefab;
    [SerializeField] MMF_Player shieldBreakFeedbacks;
    private bool isCharging = false;
    WorldProgressBar shieldBar;
    float currentEnergy;

    private void Awake()
    {
        currentEnergy = maxEnergy;
    }

    private void Update()
    {
        if(isCharging)
        {
            FillEnergy();
        }
    }

    public void SetPlayer(PlayerStateMachine player)
    {
        this.player = player;
        Transform toFollow = player.selectedCharacter.transform;
        shooter = player.selectedCharacter.GetComponent<Shooter>();
        playerHealth = player.GetComponent<PlayerHealth>();
        shooter.OnShoot += ApplyPenetration;
        
        shieldBar = Instantiate(worldProgressBarPrefab);
        shieldBar.Setup(toFollow);
        UpdateEnergyBar();
    }

    private void ApplyPenetration(Projectile projectile)
    {
        if (isCharging) return; //Don't apply penetration and don't interrupt charging
        projectile.Penetrative = true;

        currentEnergy -= spendEnergyPerShot;
        if(currentEnergy <= 0)
        {
            currentEnergy = 0;
            isCharging = true;
        }
        UpdateEnergyBar();
    }

    private void FillEnergy()
    {
        currentEnergy += fillEnergyPerSecond * Time.deltaTime;
        
        if(currentEnergy >= maxEnergy)
        {
            currentEnergy = maxEnergy;
            isCharging = false;
        }
        UpdateEnergyBar();
    }
    private void UpdateEnergyBar()
    {
        shieldBar.gameObject.SetActive(true);
        shieldBar.UpdateBar((int)currentEnergy, (int)maxEnergy);
    }

    private void HideBar()
    {
        shieldBar.gameObject.SetActive(false);
    }
   
}
