using UnityEngine;

public class BouncingProjectileEffect : MonoBehaviour , IRelic
{
    public int BounceCount = 2;
    Shooter shooter;
    public RelicSO RelicSO { get; set; }
    [Range(-100 , 0)][SerializeField] int effectToDamage = -50;
    private void Start() // Test
    {
        shooter = PlayerController.Instance.GetComponent<PlayerStateMachine>().selectedCharacter.GetComponent<Shooter>();
        shooter.OnShoot += SetProjectileBouncing;        
    }

    private void SetProjectileBouncing(Projectile projectile)
    {
        float decreasedDamage = projectile.GetDamage * (100 + effectToDamage) / 100f;
        projectile.CanBounce = true;
        projectile.BounceCount = BounceCount;
        projectile.SetDamage((int)decreasedDamage);
    }

    public void ResetEffect(PlayerStateMachine stateMachine)
    {
    }

    public void SettleEffect(PlayerStateMachine stateMachine)
    {
        stateMachine.selectedCharacter.GetComponent<Shooter>().OnShoot += SetProjectileBouncing;
    }
}

