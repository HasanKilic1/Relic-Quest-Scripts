using UnityEngine;

public class SkeletonAnim : MonoBehaviour
{
    Skeleton skeleton;
    [SerializeField] EnemyProjectile axe;
    private void Awake()
    {
        skeleton = GetComponentInParent<Skeleton>();
    }
    public void ThrowAxe()
    {
        EnemyProjectile axeObj = Instantiate(axe , skeleton.ShootPos.position , Quaternion.identity).GetComponent<EnemyProjectile>();
        axeObj.Shoot(skeleton.shootDir.normalized);
        axeObj.SetDamage((int)skeleton.Damage);
    }
}
