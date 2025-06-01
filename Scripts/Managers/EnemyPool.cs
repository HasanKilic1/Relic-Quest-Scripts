using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance { get; private set; }
    private int enemyCountOfWave;

    private void Awake()
    {
        if(Instance == null) Instance = this;
        else { Destroy(gameObject); }
    }

    private void Update()
    {
        if (Keyboard.current.insertKey.wasPressedThisFrame) 
        {
            KillAll();
        }
    }
    public void DecreaseEnemyCount() 
    {
        enemyCountOfWave--;
      //  HKDebugger.LogWorldText($"Remaining enemy count : {enemyCountOfWave}",  PlayerHealth.Instance.transform.position + Vector3.up * 7f);
        if(enemyCountOfWave == 0)
        {
            EnemySpawnManager.Instance.EnterNextWave();
        }
    }
    public void KillAll()
    {
        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            enemy.GetComponent<EnemyHealth>().HandleDeath();
        }
    }
    public void SetMaxEnemyCountOfWave(int enemyCount) => enemyCountOfWave = enemyCount;
}
