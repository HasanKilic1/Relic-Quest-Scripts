using UnityEngine;

public class QueenPassiveSkill : MonoBehaviour , IPassiveSkill
{
    PlayerHealth playerHealth;
    [SerializeField] private int level;
    [SerializeField] private float regenerationByLevel;
    [SerializeField] GameObject markObjectPrefab;
    private GameObject markedEnemy;
    private GameObject markObject;
    private readonly int threshold = 7;
    private int totalContact = 0;
    private void OnEnable()
    {
        EnemyHealth.OnDamageTaken += TryRegenerateHealth;
    }

    private void OnDisable()
    {
        EnemyHealth.OnDamageTaken -= TryRegenerateHealth;
    }

    void Start()
    {
        markObject = Instantiate(markObjectPrefab);
    }

    private void Update()
    {
        if (CanMark())
        {
            MarkNewEnemy();
        }

        markObject.SetActive(markedEnemy != null);

        if (markedEnemy)
        {
            Vector3 targetPos = new Vector3(markedEnemy.transform.position.x, 0.3f, markedEnemy.transform.position.z);
            markObject.transform.position = Vector3.Lerp(markObject.transform.position , targetPos, 5f * Time.deltaTime);
        }
    }

    private void TryRegenerateHealth(EnemyHealth enemy)
    {
        totalContact++;
        if (totalContact >= threshold)
        {
            if (enemy == markedEnemy.GetComponent<EnemyHealth>())
            {                    
                int regen = (int)(playerHealth.GetMaxHealth * regenerationByLevel / 100);
                playerHealth.IncreaseHealth(regen, true);
                totalContact = 0;
            }            
        }       
    }

    public void SetPlayer(PlayerStateMachine player)
    {
        playerHealth = player.GetComponent<PlayerHealth>();
    }

    private void MarkNewEnemy()
    {
        GameObject[] all = GameObject.FindGameObjectsWithTag("Enemy");
        if(all.Length > 0)
        {
            int randIndex = Random.Range(0, all.Length);
            markedEnemy = all[randIndex];
        }
    }
    bool CanMark()
    {
        bool hasNoMarkedEnemy = markedEnemy == null;
        bool thereIsEnemy = GameObject.FindGameObjectsWithTag("Enemy").Length > 0;
        
        return hasNoMarkedEnemy && thereIsEnemy;
    }
}
