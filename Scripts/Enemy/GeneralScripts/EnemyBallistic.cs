using UnityEngine;

public abstract class EnemyBallistic : MonoBehaviour
{
    protected Transform target;
    public abstract void Shoot(Vector3 pointOrDirection);
    public virtual void SetTarget(Transform target)
    {
        this.target = target;
    }
    public abstract void SetRange(float range);
    public abstract void SetDamage(float damage);
}
