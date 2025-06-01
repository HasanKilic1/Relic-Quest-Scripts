using MoreMountains.Feedbacks;
using UnityEngine;

public class DroppableResource : MonoBehaviour , IPooledObject
{    
    MMWiggle mmWiggle;
    DroppableResourceType resourceType;
    [SerializeField] GameObject coinBag;
    [SerializeField] GameObject healthPotion;
    [SerializeField] int quantity;

    [Header("Movement")]
    [SerializeField] float speed;
    
    bool canMoveToPlayer = false;
    bool resourceGiven = false;
    [SerializeField] MMF_Player GiveFeedbacks;
    [SerializeField] Gradient coinGradient;
    [SerializeField] Gradient healthPotionGradient;
    MMF_FloatingText floatingText;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerHealth playerHealth))
        {
            GiveResource();
        }
    }

    private void Update()
    {
        if (canMoveToPlayer)
        {
            Vector3 diff = PlayerHealth.Instance.transform.position + Vector3.up * 1.5f - transform.position;
            transform.position += diff.normalized * speed * Time.deltaTime;
            if(Vector3.Distance(transform.position, PlayerHealth.Instance.transform.position) < 2f)
            {
                GiveResource();
            }
        }
    }

    public void SetupResource(DroppableResourceType resourceType , int quantity)
    {
        if(resourceType == DroppableResourceType.Coin)
        {
            coinBag.SetActive(true);
            healthPotion.SetActive(false);
            floatingText.AnimateColorGradient = coinGradient;
        }
        else
        {
            coinBag.SetActive(false);
            healthPotion.SetActive(true);
            floatingText.AnimateColorGradient = healthPotionGradient;
        }
        this.quantity = quantity;
        floatingText.Value = string.Concat(resourceType.ToString().ToUpper() + $" +{quantity}");
    }

    private void StartMove()
    {
        mmWiggle.enabled = false;
        canMoveToPlayer = true;
    }
    
    private void GiveResource()
    {
        if(!resourceGiven)
        {
            switch (resourceType)
            {
                case DroppableResourceType.Coin:
                    EconomyManager.Instance.AddResource(ResourceType.Coin, quantity, true);
                    break;
                case DroppableResourceType.Health:
                    PlayerHealth.Instance.IncreaseHealth(quantity);
                    break;
            }
            GiveFeedbacks?.PlayFeedbacks();
            resourceGiven = true;
            Deactivate();
        }
                
    }

    public void Activate()
    {
        mmWiggle = GetComponent<MMWiggle>();
        floatingText = GiveFeedbacks.GetFeedbackOfType<MMF_FloatingText>();
        gameObject.SetActive(true);
        mmWiggle.enabled = true;
        resourceGiven = false;
        canMoveToPlayer = false;
        Invoke(nameof(StartMove), 3f);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void Initialize()
    {
        Deactivate();
    }
}
public enum DroppableResourceType
{
    Coin,
    Health,
}