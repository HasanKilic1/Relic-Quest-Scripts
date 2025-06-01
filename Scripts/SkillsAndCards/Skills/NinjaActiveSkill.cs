using MoreMountains.Feedbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HapticUser))]
public class NinjaActiveSkill : MonoBehaviour , IActiveSkill
{
    HapticUser hapticUser;
    [Header("Combat")]
    [SerializeField] GameObject vfx;
    [SerializeField] int pointCount;
    [SerializeField] float pointRadius;
    [SerializeField] float damageRadius;
    [SerializeField] float tourDuration;
    [SerializeField] AnimationCurve moveCurve = AnimationCurve.EaseInOut(0,0,1,1);
    [SerializeField] Transform cameraLook;

    [Header("Data")]
    [SerializeField] int damagePerLevel;
    [SerializeField] int baseDamage;
    [SerializeField] PlayerStateMachine playerStateMachine;
    int currentDamage;

    [Header("Feedbacks")]
    [SerializeField] MMF_Player reachfeedbacks;
    public LayerMask EnemyLayer;

    private void Awake()
    {
        hapticUser = GetComponent<HapticUser>();
        cameraLook.SetParent(null);
    }
    public void SetSkillData(int level , int abilityDamage)
    {
        currentDamage += baseDamage + level * damagePerLevel + abilityDamage;
    }

    public void SetPlayerScript(PlayerStateMachine stateMachine)
    {
        playerStateMachine = stateMachine;
        playerStateMachine.GetRangeIndicator().gameObject.SetActive(false);
        StartCoroutine(AttackRoutine());        
    }

    private IEnumerator AttackRoutine()
    {
        List<Vector3> points = GetTriangleCorners(playerStateMachine.GetClosestEnemy().transform.position);
        float pointReachDuration = tourDuration / points.Count;
        Vector3 playerStart = playerStateMachine.transform.position;
        Vector3 firstTarget = points[0];
        Vector3 closest = playerStateMachine.GetClosestEnemy().transform.position;

        Vector3 vfxPos = new Vector3(closest.x, playerStateMachine.transform.position.y, closest.z);
        HandleVisual(vfxPos);
        cameraLook.transform.position = vfxPos + Vector3.up * 2f;
        ChangeCameraFollow(cameraLook);

        float t0 = 0f;

        while (t0 < 0.2f) // MoveToFirstPosition
        {
            t0 = MovePosition(0.2f, playerStart, firstTarget, t0);
            yield return null;
        }

        CheckDamage(closest);

        for (int j = 1; j < points.Count; j++)
        {
            Vector3 start = playerStateMachine.transform.position;
            Vector3 end = points[j];
            playerStateMachine.transform.forward = (end - start).normalized;
            float t = 0f;

            while (t < pointReachDuration)
            {
                t = MovePosition(pointReachDuration, start, end, t);
                yield return null;
            }
            reachfeedbacks?.PlayFeedbacks();
            if (hapticUser != null) hapticUser.Play();
        }

        yield return null;

        t0 = 0f;
        while (t0 < 0.3f) // MoveToFirstPosition
        {
            t0 = MovePosition(0.2f, playerStateMachine.transform.position, playerStart, t0);
            yield return null;
        }

        CameraController.Instance.ReturnNormal();
        playerStateMachine.GetRangeIndicator().gameObject.SetActive(true);
        //playerStateMachine.transform.position = playerStart;
    }

    private void CheckDamage(Vector3 center)
    {
        Collider[] colls = Physics.OverlapSphere(center, damageRadius, EnemyLayer);
        foreach (var collider in colls)
        {
            if(collider.TryGetComponent(out EnemyHealth enemyHealth))
            {
                enemyHealth.TakeDamage(currentDamage , Vector3.zero , isUnstoppableAttack:true);
            }
        }
    }

    private void HandleVisual(Vector3 center)
    {
        GameObject effect = Instantiate(vfx, center, vfx.transform.rotation);
    }

    private void ChangeCameraFollow(Transform look)
    {
        CameraController.Instance.ChangeFollowTarget(look);
        CameraController.Instance.ChangeLookTarget(look);
    }

    private float MovePosition(float pointReachDuration, Vector3 start, Vector3 end, float t)
    {
        t += Time.deltaTime;
        float evaluated = moveCurve.Evaluate(t / pointReachDuration);
        playerStateMachine.transform.position = Vector3.Lerp(start, end, evaluated);
        playerStateMachine.transform.forward = (end - start).normalized;
        return t;
    }

    private List<Vector3> GetTriangleCorners(Vector3 center)
    {
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < 3; i++)
        {
            float angle = i * Mathf.PI * 2 / 3; // Divide the circle into 3 equal parts
            Vector3 corner = new Vector3(
                center.x + pointRadius * Mathf.Cos(angle),
                center.y,
                center.z + pointRadius * Mathf.Sin(angle)
            );
            points.Add(corner);
        }

        return points;

    }
/*
    private void OnDrawGizmosSelected()
    {
        if (vfx != null) {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(vfx.transform.position, damageRadius);
            Gizmos.color = Color.yellow;
            foreach (var point in Points())
            {
                Gizmos.DrawWireSphere(point, 0.5f);
            }
        }
    }
  */
}
