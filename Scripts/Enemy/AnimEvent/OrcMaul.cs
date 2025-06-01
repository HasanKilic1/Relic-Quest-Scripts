using UnityEngine;

public class OrcMaul : MonoBehaviour
{
    private int damage;
    private bool isAttacking;
    private void OnTriggerEnter(Collider other)
    {
        if (!isAttacking) return;
        if(other.TryGetComponent<PlayerHealth>(out var health))
        {
            health.TakeDamage(damage);
        }
    }

    public void SetDamage(int damage) => this.damage = damage;
    public void SetIsAttacking(bool isAttacking) => this.isAttacking = isAttacking;
}
