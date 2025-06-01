using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveAreaAttack : BossAttackBehaviour
{
    public static event Action<Boss> OnExplosiveAttackFinished;
    [SerializeField] string anim_parameter;
    [SerializeField] AnimatorControllerParameterType parameterType;
    [SerializeField] List<ExplosionTargeter> targeters;

    public override void Attack()
    {
        boss.Stop();
        switch (parameterType)
        {
            case AnimatorControllerParameterType.Bool:
                boss.Animator.SetBool(name: anim_parameter, value: true);
                break;
            case AnimatorControllerParameterType.Trigger:
                boss.Animator.SetTrigger(name: anim_parameter);

                break;
        }
        StartCoroutine(SendExplosions());
    }

 
    public override void SetBoss(Boss boss)
    {
        this.boss = boss;
    }

    private IEnumerator SendExplosions()
    {
        for (int i = 0; i < targeters.Count; ++i)
        {
            float wait = targeters[i].WaitBeforeSpawn;
            targeters[i].Spawn(boss);
            yield return new WaitForSeconds(wait);                              
        }
        ResetSequence();
    }

    public override void ResetSequence()
    {
        if (parameterType == AnimatorControllerParameterType.Bool)
        {
            boss.Animator.SetBool(anim_parameter, false);
        }
        boss.FinishAttack();
        OnExplosiveAttackFinished?.Invoke(boss);
    }

}

[Serializable]
public struct ExplosionTargeter
{
    public enum SpawnOn
    {
        None,
        Player,
        Boss
    }
    
    [SerializeField] ExplosiveArea explosiveArea;
    [SerializeField] SpawnOn spawnOn;
    [SerializeField] Vector3 spawnOffset;
    [SerializeField] int spawnCount;
    [SerializeField] private bool randomizeOffset;
    [SerializeField] private float offsetMinMax;
    public float WaitBeforeSpawn;
   

    public void Spawn(Boss boss)
    {
        Vector3 pos = Vector3.zero;
        switch (spawnOn)
        {
            case SpawnOn.Player:
                pos = boss.player.transform.position;
                break;
            case SpawnOn.Boss:
                pos = boss.transform.position;
                break;
        }
        Vector3 posBeforeSpawn = pos;

        for(int i = 0; i < spawnCount; i++)
        {
            Vector3 spawnPos = posBeforeSpawn + GetSpawnOffset();
            ExplosiveArea area = GameObject.Instantiate(explosiveArea, spawnPos, explosiveArea.transform.rotation);
            area.FollowTo = boss.player.transform;
            area.Use();
        }
    }

    private Vector3 GetSpawnOffset()
    {
        if(!randomizeOffset) { return spawnOffset; }
        else
        {
            Vector3 randomOfs = new(GetRandomValue(),
                                    GetRandomValue(),
                                    GetRandomValue());
            return randomOfs;
        }
    }

    private float GetRandomValue() { return UnityEngine.Random.Range(-offsetMinMax, offsetMinMax); }
}
