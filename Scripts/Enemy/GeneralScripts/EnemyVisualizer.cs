using MoreMountains.Tools;
using System.Collections;
using UnityEngine;

public class EnemyVisualizer : MonoBehaviour
{
    Enemy enemy;
    private SkinnedMeshRenderer mesh;
    private Animator animator;
    private bool isUnderInfluence;

    private void Awake()
    {
        mesh = GetComponentInChildren<SkinnedMeshRenderer>();             
        animator = GetComponentInChildren<Animator>();
        enemy = GetComponent<Enemy>();
    }

    private void Start()
    {
        GameObject spawnVfx = SceneObjectPooler.Instance.GetEnemySpawnVfx();
        spawnVfx.transform.position = transform.position + Vector3.up * 0.15f;
    }

    public void Influence(float influenceToMovementSpeed = 0f, float influenceToAnimatorSpeed = 0f, float duration = 0f)
    {
        if (isUnderInfluence)
        {
            return;
        }

        if (animator.MMHasParameterOfType("GetHit", AnimatorControllerParameterType.Trigger))
        {
            animator.ResetTrigger("GetHit");
        }

        AnimatorControllerParameter[] parameters = animator.parameters;
        foreach (AnimatorControllerParameter parameter in parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Trigger)
            {
                animator.ResetTrigger(parameter.name);
            }
            if (parameter.type == AnimatorControllerParameterType.Bool)
            {
                animator.SetBool(parameter.name, false);
            }
        }
        StartCoroutine(InfluenceRoutine(influenceToMovementSpeed, influenceToAnimatorSpeed, duration));
    }

    private IEnumerator InfluenceRoutine(float influenceToMovementSpeed, float influenceToAnimatorSpeed, float duration)
    {
        isUnderInfluence = true;
        float movementSpeedBefore = enemy.movementSpeed;
        float animatorSpeedBefore = animator.speed;

        float newMovementSpeed = movementSpeedBefore * influenceToMovementSpeed / 100f;        
        float newAnimatorSpeed = animatorSpeedBefore + influenceToAnimatorSpeed;

        enemy.movementSpeed = newMovementSpeed;
        animator.speed = Mathf.Max(0f, newAnimatorSpeed);

        yield return new WaitForSeconds(duration);

        enemy.movementSpeed = movementSpeedBefore;
        animator.speed = 1f;
        isUnderInfluence = false;
    }

}
