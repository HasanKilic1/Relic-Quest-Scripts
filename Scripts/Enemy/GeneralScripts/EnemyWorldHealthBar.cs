using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.UI;

public class EnemyWorldHealthBar : MonoBehaviour , IPooledObject
{
    [SerializeField] MMProgressBar progressBar;
    EnemyHealth toFollow;
    [SerializeField] Vector3 followOffset;
    [SerializeField] Image healthImage;
    [SerializeField] Color healthColor;

    private void Update()
    {
        Follow();
    }

    private void Follow()
    {
        if (toFollow != null)
        {
            transform.position = toFollow.GetTargetedPos().position + followOffset;
            if(toFollow.GetCurrentHealth() <= 0)
            {
                Inactivate();
            }
        }
    }
    public void Setup(EnemyHealth enemy)
    {
        toFollow = enemy;
    }

    public Vector3 SetFollowOffset(Vector3 offset) => followOffset = offset;
    public void UpdateHealthBar(int currentHealth , int maxHealth)
    {
        progressBar.UpdateBar(currentHealth, 0 , maxHealth);
    }

    public void Initialize()
    {
        Deactivate();
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {        
        gameObject.SetActive(false);
    }
   
    public void Inactivate()
    {
        toFollow = null;
        Deactivate();
    }
}
