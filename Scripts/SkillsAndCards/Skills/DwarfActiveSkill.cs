using System;
using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DwarfActiveSkill : MonoBehaviour , IActiveSkill
{
    public static event Action OnSkillStarted;
    public static event Action OnSkillFinished;
    public UnityEvent OnExplosion;
    Vector3 explosionPos;
    DwarfAnimController dwarf;
    PlayerStateMachine playerStateMachine;
    [SerializeField] private float waitBeforeFinish;
    [SerializeField] private int BaseDamage;
    [SerializeField] private float radius;
    [SerializeField] private float yHeight;
    [SerializeField] private AnimationCurve yMovementCurve;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] GameObject explosionVfx;
    [SerializeField] PoolObject meteorVfx;
    [SerializeField] private float meteorDistance;
    [SerializeField] private float waitBeforeMeteorRain;
    [SerializeField] private float meteorRadius;
    [SerializeField] private float timeBetweenMeteors;
    [SerializeField] private MMF_Player finishFeedbacks;
    [SerializeField] private MMF_Player meteorFeedbacks;

    private ObjectPooler<PoolObject> meteorPool;
    private int currentDamage;

    private void Start()
    {
        meteorPool = new ObjectPooler<PoolObject>();
        meteorPool.InitializeObjectPooler(meteorVfx, SceneObjectPooler.Instance.transform, 10);
    }

    public void SetPlayerScript(PlayerStateMachine stateMachine)
    {
        playerStateMachine = stateMachine;
        dwarf = stateMachine.selectedCharacter.GetComponent<DwarfAnimController>();
        Use();
    }

    public void SetSkillData(int level, int abilityDamage)
    {
        currentDamage = BaseDamage * level + abilityDamage;             
    }

    private void Use()
    {
        OnSkillStarted?.Invoke();
        StartCoroutine(SkillRoutine());
    }

    private IEnumerator SkillRoutine()
    {
        Vector3 startPos = playerStateMachine.transform.position;
        Vector3 endPos = playerStateMachine.transform.position + Vector3.up * yHeight;
        float t = 0f;
        while(t < waitBeforeFinish)
        {
            t += Time.deltaTime;
            float evaluated = yMovementCurve.Evaluate(t / waitBeforeFinish);
            playerStateMachine.transform.position = Vector3.Lerp(startPos, endPos, evaluated);
            yield return null;
        }
   //     yield return new WaitForSeconds(waitBeforeFinish);
        
        OnExplosion?.Invoke();
        CheckNearbyEnemies(dwarf.transform.position , radius , currentDamage);
        Instantiate(explosionVfx , dwarf.GetSplashPosition + Vector3.up * 1.5f , explosionVfx.transform.rotation);
        AdjustMeteorRainPositions(out List<Vector3> meteorPositions);
        finishFeedbacks?.PlayFeedbacks();

        yield return new WaitForSeconds(waitBeforeMeteorRain);
        
        for (int i = 0; i < meteorPositions.Count; i++)
        {            
            PoolObject meteor = meteorPool.GetObject();
            meteor.transform.position = meteorPositions[i];
            meteorFeedbacks?.PlayFeedbacks();
            HapticManager.instance.Impulse(0.1f,0.2f,0.2f);
            yield return new WaitForSeconds(timeBetweenMeteors);
            CheckNearbyEnemies(meteorPositions[i], meteorRadius , currentDamage / 4);
        }
        OnSkillFinished?.Invoke();
    }

    private void CheckNearbyEnemies(Vector3 pos , float radius , int damage)
    {
        Collider[] enemies = Physics.OverlapSphere(pos, radius, enemyLayer);

        foreach (var enemy in enemies)
        {
            if (enemy.TryGetComponent(out EnemyHealth enemyHealth))
            {
                enemyHealth.TakeDamage(currentDamage, Vector3.zero, isUnstoppableAttack: true);
            }
        }
    }

    private void AdjustMeteorRainPositions(out List<Vector3> meteorPositions)
    {
        meteorPositions = new List<Vector3>();
        Vector3 orientation = dwarf.transform.forward;
        
        for (int i = 0; i < 4; i++)
        {
            Quaternion rotate = Quaternion.AngleAxis(90 * i, Vector3.up);
            Vector3 point = rotate * orientation;
            point *= meteorDistance; 
            Vector3 spawnPoint = dwarf.transform.position + point;
            meteorPositions.Add(spawnPoint);
        }
    }
}