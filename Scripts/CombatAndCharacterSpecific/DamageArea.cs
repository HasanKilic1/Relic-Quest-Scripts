using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageArea : MonoBehaviour
{
    [Header("Combat")]
    [SerializeField] float damageInterval;
    [SerializeField] int damagePerInterval;
    [SerializeField] float lifeTime;
    float damageTimer;
    [Header("Animation")]
    [SerializeField] bool isAnimated;
    [SerializeField] Vector3 startScale = Vector3.one / 2f;
    [SerializeField] Vector3 targetScale = Vector3.one;
    [SerializeField] float scalingTime;
    void Start()
    {
        Destroy(gameObject , lifeTime);
        StartCoroutine(ScalingAnimationRoutine());
        damageTimer = damageInterval;
    }

    private IEnumerator ScalingAnimationRoutine()
    {
        float t = 0f;        
        while (t <= scalingTime)
        {
            t+= Time.deltaTime;
            float normalizedT = t / scalingTime;
            transform.localScale = Vector3.Lerp(startScale, targetScale, normalizedT);
            yield return null;         
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.TryGetComponent<PlayerHealth>(out var player))
        {
            damageTimer += Time.deltaTime;
            if (damageTimer >= damageInterval)
            {
                damageTimer = 0f;
                Debug.Log("gave damac");
                player.TakeDamage(damagePerInterval);
            }
        }
        
    }
}
