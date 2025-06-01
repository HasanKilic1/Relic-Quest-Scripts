using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    public static EnemySpawnManager Instance;

    private ObjectSpawner objectSpawner;

    [SerializeField] private List<SpawnSO> spawnSOs;

    public SpawnSO spawnSO;
    public int MaxWave;
    public int CurrentWave;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        objectSpawner = GetComponent<ObjectSpawner>();
    }

    public void StartNewLevel()
    {
        foreach (var so in spawnSOs)
        {
            if (so.IsValid(GameStateManager.Instance.CurrentLevel))
            {
                spawnSO = so;
                MaxWave = spawnSO.GetWaveCount();
                break; 
            }
        }
        CurrentWave = 0;
        EnterNextWave();
    }

    public void EnterNextWave()
    {
        CurrentWave++;
        if (CurrentWave > MaxWave)
        {
            GameStateManager.Instance.FinishLevel();
        }
        else
        {
            Invoke(nameof(SpawnWave), 2.5f);
        }
        
    }

    private void SpawnWave()
    {
        List<GameObject> enemyList = spawnSO.GetEnemiesOfWave(CurrentWave);
        objectSpawner.SpawnToValidGridPositions(enemyList, out List<Vector3> placements, Vector3.zero);
        EnemyPool.Instance.SetMaxEnemyCountOfWave(enemyList.Count);
    }

}
