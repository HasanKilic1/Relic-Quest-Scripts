using MoreMountains.Feedbacks;
using System;
using UnityEngine;

[RequireComponent(typeof(Character))]
[RequireComponent(typeof(HapticUser))]
public class Shooter : MonoBehaviour
{
    private Character character;
    private HapticUser hapticUser;
    public event Action<Projectile> OnShoot;
    private ObjectPooler<Projectile> projectilePooler;
    [Header("Standard Combat")]
    [SerializeField] private PlayerStateMachine playerStateMachine;
    [SerializeField] private Projectile projectile;
    [SerializeField] private Transform shootPosition;
    [SerializeField] private MMF_Player shootFeedBacks;

    [SerializeField] PlayerHealth playerHealth;

    private void Awake()
    {
        character = GetComponent<Character>();
        hapticUser = GetComponent<HapticUser>();
    }

    private void Start()
    {
        projectilePooler = new ObjectPooler<Projectile>();
        projectilePooler.InitializeObjectPooler(projectile, SceneObjectPooler.Instance.transform, 100);
    }

    public void ShootAnimEvent()
    {
        if(playerStateMachine.GetClosestEnemy() == null) return;
       
        InitProjectile(out Projectile projectile);
        hapticUser.Play();
    }

    public void InitProjectile(out Projectile projectile)
    {
        Vector3 diffToEnemy = playerStateMachine.GetClosestEnemy().GetTargetedPos().position - shootPosition.position;
        Projectile _projectile = projectilePooler.GetObject();
        _projectile.transform.position = shootPosition.position;
        _projectile.Activate();
        _projectile.SetDamage((int)character.GetDamage);
        _projectile.SetCriticalDamageChance(character.GetCritChance);
        _projectile.SetRegenerationRatio(character.GetLifeSteal);
        //_projectile.SetTarget(playerStateMachine.GetClosestEnemy().targetedPosition);
        _projectile.SetDirection(diffToEnemy.normalized);
        
        OnShoot?.Invoke(_projectile);
        shootFeedBacks.PlayFeedbacks();
        projectile = _projectile;
    }
  
    public Transform ShootPosition => this.shootPosition;

}
