using UnityEngine;

public class ResourceDropper : MonoBehaviour
{
    EnemyHealth enemyHealth;
    [Range(0 , 100)][SerializeField] int resourceDropChance = 100;
    [SerializeField] DroppableResourceType resourceType = DroppableResourceType.Coin;
    [SerializeField][Range(0, 100)] int coinDropChance = 90;
    [SerializeField] int coinMin = 10;
    [SerializeField] int coinMax = 20;
    [SerializeField][Range(0, 100)] int healthDropChance = 10;
    [Range(0, 100)][SerializeField] int healthMin = 3;
    [Range(0, 100)][SerializeField] int healthMax = 5;

    private void OnValidate()
    {
        healthDropChance = 100 - coinDropChance;
    }

    public void Drop()
    {
        bool canDrop = Random.Range(0, 100) < resourceDropChance;

        if(canDrop)
        {
            resourceType = Random.Range(0,100) < coinDropChance ? DroppableResourceType.Coin : DroppableResourceType.Health;

            int quantity = resourceType == DroppableResourceType.Coin ? Random.Range(coinMin, coinMax) : Random.Range(healthMin , healthMax);

            DroppableResource drop = SceneObjectPooler.Instance.GetDroppableResource();
            drop.SetupResource(resourceType, quantity);
            float dropHeight = 1f;
            drop.transform.position = new Vector3(transform.position.x, dropHeight, transform.position.z);
        }

    }
}
