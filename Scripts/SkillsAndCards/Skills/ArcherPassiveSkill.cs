using UnityEngine;

public class ArcherPassiveSkill : MonoBehaviour , IPassiveSkill
{
    int shooted;
    PlayerStateMachine playerStateMachine;
    Shooter shooter;

    Projectile subscribedProjectile;
    public void SetPlayer(PlayerStateMachine stateMachine)
    {
        this.playerStateMachine = stateMachine;
        shooter = playerStateMachine.selectedCharacter.GetComponent<Shooter>();
        shooter.OnShoot += CheckShooted;
    }

    private void CheckShooted(Projectile projectile)
    {
        shooted++;
        if(shooted % 5 == 0)
        {
            subscribedProjectile = projectile;
            projectile.OnThisProjectileCollision.AddListener(GiveSecondaryDamage);
        }
    }

    private void GiveSecondaryDamage(EnemyHealth enemyHealth)
    {
        int halfDamage = subscribedProjectile.GetDamage / 2;
        enemyHealth.TakeDamage(halfDamage, Vector3.zero);        
        subscribedProjectile.OnThisProjectileCollision.RemoveListener(GiveSecondaryDamage);
    }
}
