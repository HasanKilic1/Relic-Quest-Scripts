using System.Collections.Generic;
using UnityEngine;

public class SceneObjectPooler : MonoBehaviour
{
    public static SceneObjectPooler Instance{get; private set;}

    private Dictionary<string, List<GameObject>> pools;

    [SerializeField] PoolObject enemySpawnVfx;
    [SerializeField] PoolObject enemyDamageVfx;
    [SerializeField] EnemyWorldHealthBar enemyWorldHealthBar;
    [SerializeField] DroppableResource droppableResource;
    [SerializeField] EnemyShield enemyShield;
    [SerializeField] PoolObject EnemyDeathGroundVisual;
    private ObjectPooler<PoolObject> enemySpawnVfxPooler;
    private ObjectPooler<PoolObject> damageVfxPooler;
    private ObjectPooler<EnemyWorldHealthBar> worldHealthBarPooler;
    private ObjectPooler<DroppableResource> droppableResourcePooler;
    private ObjectPooler<EnemyShield> enemyShieldPooler;
    private ObjectPooler<PoolObject> deathGroundVisualPooler;
    private void Awake()
    {
        if(Instance == null) Instance = this;
        pools = new Dictionary<string, List<GameObject>>();

        enemySpawnVfxPooler = new ObjectPooler<PoolObject>();
        enemySpawnVfxPooler.InitializeObjectPooler(enemySpawnVfx, transform, 10);

        damageVfxPooler = new ObjectPooler<PoolObject>(); 
        damageVfxPooler.InitializeObjectPooler(enemyDamageVfx, this.transform, 15);

        worldHealthBarPooler = new ObjectPooler<EnemyWorldHealthBar>(); 
        worldHealthBarPooler.InitializeObjectPooler(enemyWorldHealthBar, null, 10);

        droppableResourcePooler = new ObjectPooler<DroppableResource>();
        droppableResourcePooler.InitializeObjectPooler(droppableResource , transform, 10);

        enemyShieldPooler = new ObjectPooler<EnemyShield>();
        enemyShieldPooler.InitializeObjectPooler(enemyShield, transform, 10);

        deathGroundVisualPooler = new ObjectPooler<PoolObject>();
        deathGroundVisualPooler.InitializeObjectPooler(EnemyDeathGroundVisual, transform, 10);
    }

    public GameObject GetEnemySpawnVfx()
    {
        PoolObject spawnVfx = enemySpawnVfxPooler.GetObject();
        spawnVfx.Activate();
        return spawnVfx.gameObject;
    }

    public GameObject GetDamageVfx()
    {
        PoolObject damageVfx = damageVfxPooler.GetObject();
        damageVfx.Activate();
        return damageVfx.gameObject;
    }

    public EnemyWorldHealthBar GetEnemyHealthBar()
    {
        EnemyWorldHealthBar healthBar = worldHealthBarPooler.GetObject();
        healthBar.Activate();
        return healthBar;
    }

    public DroppableResource GetDroppableResource()
    {
        DroppableResource droppableResource = droppableResourcePooler.GetObject();
        droppableResource.Activate();
        return droppableResource;
    }

    public EnemyShield GetEnemyShield()
    {
        EnemyShield enemyShield_ = enemyShieldPooler.GetObject();
        enemyShield_.Activate();
        return enemyShield_;
    }

    public GameObject GetDeathGroundVisual()
    {
        PoolObject visual = deathGroundVisualPooler.GetObject();
        visual.Activate();
        return visual.gameObject;
    }

    public void CreatePool(string poolName, GameObject prefab, int initialSize)
    {
        if (!pools.ContainsKey(poolName))
        {
            List<GameObject> objectPool = new List<GameObject>();

            for (int i = 0; i < initialSize; i++)
            {
                GameObject obj = Instantiate(prefab);
                if(obj.TryGetComponent(out IPooledObject pooledObject))
                {
                    pooledObject.Initialize();
                }
                obj.transform.parent = transform;
                obj.SetActive(false);
                objectPool.Add(obj);
            }

            pools.Add(poolName, objectPool);
        }
    }

    public GameObject GetObjectFromPool(string poolName, GameObject prefab)
    {
        if (!pools.ContainsKey(poolName))
        {
            CreatePool(poolName, prefab, 1);
        }

        List<GameObject> pool = pools[poolName];
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                if(obj.TryGetComponent(out IPooledObject pooledObject))
                {
                    pooledObject.Activate();
                }
                obj.SetActive(true);
                return obj;
            }
        }

        // If no inactive object is found, instantiate a new one
        GameObject newObj = Instantiate(prefab);
        newObj.SetActive(true);
        if(newObj.TryGetComponent(out IPooledObject pooledObject1))
        {
            pooledObject1.Activate();
        }
        pool.Add(newObj);
        return newObj;
    }

    public void ReturnObjectToPool(GameObject obj)
    {
        obj.SetActive(false);
    }
}
