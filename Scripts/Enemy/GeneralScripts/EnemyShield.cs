using MoreMountains.Feedbacks;
using UnityEngine;

public class EnemyShield : MonoBehaviour , IPooledObject
{
    [SerializeField] ParticleSystem shieldParticle;
    [SerializeField] Gradient shieldColorGradient;
    Transform toFollow;
    [SerializeField] Vector3 followOffset;
    [SerializeField] MMF_Player breakFeedbacks;
    private void Update()
    {
        if (toFollow != null)
        {
            transform.position = toFollow.position + followOffset;
        }
    }
    public void Activate()
    {
        gameObject.SetActive(true);
        UpdateShield(1, 1);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void Initialize()
    {
        gameObject.SetActive(false);
    }

    public void UpdateShield(int currentHealth , int maxHealth)
    {       
        Color particleColor = shieldColorGradient.Evaluate(currentHealth / maxHealth);
        var mainModule = shieldParticle.main;
        mainModule.startColor = particleColor;
        if(currentHealth <= 0)
        {
            breakFeedbacks?.PlayFeedbacks();
        }
    }

    public void SetUp(EnemyHealth enemyHealth , Vector3 followOffset)
    {
        this.followOffset = followOffset;
        toFollow = enemyHealth.transform;
    }
}
