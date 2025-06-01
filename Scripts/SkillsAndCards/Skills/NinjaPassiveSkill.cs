using MoreMountains.Feedbacks;
using UnityEngine;

public class NinjaPassiveSkill : MonoBehaviour , IPassiveSkill
{
    PlayerStateMachine player;
    [SerializeField] float dashSpeed;
    [SerializeField] float dashDuration;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] int dashAreaDamage;
    [SerializeField] float damageRadius;
    [SerializeField] GameObject dashSplash;
    [SerializeField] MMF_Player dashFeedbacks;
    
    private float standardRollMoveSpeed;
    private float standardRollDuration;

    private int rolled;
    private int Slide_Hash = Animator.StringToHash("Dash");

    public void SetPlayer(PlayerStateMachine player)
    {
        this.player = player;
        standardRollMoveSpeed = player.RollStartSpeed;
        standardRollDuration = player.RollDuration;
        player.OnRoll += Dash;
    }

    private void Dash(RollState rollState)
    {
        rolled++;
        if(rolled % 1 == 0 )
        {
            GiveDamageOnArea();
            dashFeedbacks?.PlayFeedbacks();
            rollState.OnRollStateFinish += ResetDash;
            rollState.SetRollHash(Slide_Hash);
            player.RollStartSpeed = dashSpeed;
            player.RollDuration = dashDuration;
        }        
    }
    private void ResetDash()
    {
        player.RollStartSpeed = standardRollMoveSpeed;
        player.RollDuration = standardRollDuration;
        CreateSplash();
        GiveDamageOnArea();
    }

    private void GiveDamageOnArea()
    {
        Collider[] colls = Physics.OverlapSphere(player.transform.position, damageRadius, enemyLayer);
        foreach (var coll in colls)
        {
            if(coll.TryGetComponent(out EnemyHealth enemyHealth))
            {
                enemyHealth.TakeDamage(dashAreaDamage, Vector3.zero, isUnstoppableAttack: true);
            }
        }
    }

    private void CreateSplash()
    {
        GameObject splash = Instantiate(dashSplash, player.transform.position + Vector3.up * 0.2f, dashSplash.transform.rotation);
    }
}
