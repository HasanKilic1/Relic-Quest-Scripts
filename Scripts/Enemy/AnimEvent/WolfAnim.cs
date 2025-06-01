using MoreMountains.Feedbacks;
using UnityEngine;

public class WolfAnim : MonoBehaviour
{
    [SerializeField] WolfBoss wolf;
    [SerializeField] EnemyProjectile handProjectile;
    [SerializeField] Transform rightHand;
    [SerializeField] Transform leftHand;
    [SerializeField] MMF_Player shootFeedBacks;
   
    public void ShootFromRightHand()
    {
        SpawnProjectile(rightHand);
    }

    public void ShootFromLeftHand()
    {
        SpawnProjectile(leftHand);
    }

    private void SpawnProjectile(Transform pos)
    {
        EnemyProjectile project = Instantiate(handProjectile , pos.position , Quaternion.identity);
        project.Shoot(transform.forward);
        project.SetDamage((int)wolf.Damage);
        shootFeedBacks?.PlayFeedbacks();
    }
}
