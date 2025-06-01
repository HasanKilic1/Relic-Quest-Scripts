using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Events;

public class EnemyContactShield : MonoBehaviour,IContactProvider
{
    public UnityEvent OnShieldBroken;
    public UnityEvent OnProjectileContact;
    [SerializeField] WorldProgressBar worldProgressBar;
    [SerializeField] private int health = 100;
    [SerializeField] MMF_Player shieldBrokenFeedbacks;
    private int maxHealth;
    private void Start()
    {
        maxHealth = health;
    }
    private void OnValidate()
    {
        if (GetComponent<BoxCollider>()!= null && !GetComponent<BoxCollider>().isTrigger)
        {
            Debug.LogWarning("You should make the collider as triggered to make shield work correctly! " + this.gameObject.name);
        }
        if(GetComponent<SphereCollider>() != null && !GetComponent<SphereCollider>().isTrigger)
        {
            Debug.LogWarning("You should make the collider as triggered to make shield work correctly! " + this.gameObject.name);
        }
        if (GetComponent<CapsuleCollider>() != null && !GetComponent<CapsuleCollider>().isTrigger)
        {
            Debug.LogWarning("You should make the collider as triggered to make shield work correctly! " + this.gameObject.name);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Projectile projectile))
        {
            health -= projectile.GetDamage;
            if(worldProgressBar != null) worldProgressBar.UpdateBar(health, maxHealth);

            if(health <= 0)
            {
                OnShieldBroken?.Invoke();

                if(shieldBrokenFeedbacks != null)
                {
                    shieldBrokenFeedbacks.PlayFeedbacks();
                }
                Destroy(gameObject);
            }
            OnProjectileContact?.Invoke();
        }
    }
    
}
