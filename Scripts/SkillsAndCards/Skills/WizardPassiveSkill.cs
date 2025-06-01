using MoreMountains.Feedbacks;
using System.Collections;
using UnityEngine;

public class WizardPassiveSkill : MonoBehaviour , IPassiveSkill
{
    PlayerStateMachine playerStateMachine;
    [SerializeField] int maxEnergy;
    [SerializeField] int requiredEnergyPerShot;
    [Range(0,100)][SerializeField] float rechargeRatioPerSec;
    [SerializeField] WorldProgressBar energyBarPrefab;
    [SerializeField] float radius;
    [SerializeField] int damage;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] MMF_Player skillFeedbacks;
    MMF_InstantiateObject instantiateObject;

    WorldProgressBar energyBar;
    [SerializeField] float currentEnergy;
    private bool isCharging = false;
    private float chargeTimer;
    private void OnEnable()
    {
        Projectile.OnAnyProjectileCollision += GiveDamageOnArea;
    }
   
    private void OnDisable()
    {
        Projectile.OnAnyProjectileCollision -= GiveDamageOnArea;
    }

    private void Update()
    {
        chargeTimer += Time.deltaTime;
        if(chargeTimer > 1f && currentEnergy < maxEnergy)
        {
            chargeTimer = 0f;
            currentEnergy += (maxEnergy * rechargeRatioPerSec / 100);
            currentEnergy = Mathf.Min(currentEnergy, maxEnergy);
            energyBar.UpdateBar((int)currentEnergy, maxEnergy);
        }
        playerStateMachine.CanAttack = currentEnergy >= maxEnergy * 30f / 100f;
    }

    public void SetPlayer(PlayerStateMachine player)
    {
        playerStateMachine = player;
        player.selectedCharacter.GetComponent<Shooter>().OnShoot += SpendEnergy;

        currentEnergy = maxEnergy;
        energyBar = Instantiate(energyBarPrefab);
        energyBar.Setup(player.transform);
        instantiateObject = skillFeedbacks.GetFeedbackOfType<MMF_InstantiateObject>();
    }
    private void GiveDamageOnArea(EnemyHealth enemy)
    {
        Collider[] colls = new Collider[5];
        int result = Physics.OverlapSphereNonAlloc(enemy.GetTargetedPos().position, radius,colls ,enemyLayer);
        foreach (var coll in colls)
        {
            if(coll != null && coll.TryGetComponent(out EnemyHealth enemyHealth))
            {
                if (enemyHealth == enemy) continue;
                enemyHealth.TakeDamage(damage, Vector3.zero, isUnstoppableAttack: true);
            }
            
        }
        instantiateObject.TargetPosition = enemy.GetTargetedPos().position;
        if(skillFeedbacks != null)
        {
            skillFeedbacks.PlayFeedbacks();
        }
    }

    private void SpendEnergy(Projectile projectile)
    {
        if (isCharging) return;
        currentEnergy -= requiredEnergyPerShot;
        energyBar.UpdateBar((int)currentEnergy, maxEnergy);
        if (currentEnergy <= 0)
        {
            currentEnergy = 0;
            return;
        }
    }

}
