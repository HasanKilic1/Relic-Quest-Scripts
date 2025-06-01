using UnityEngine;

public class EnemyInfluencer : MonoBehaviour
{
    [Range(0f, 100f)][SerializeField] private float influenceToMovementSpeed;
    [Range(-1f, 0f)][SerializeField] private float influenceToAnimatorSpeed;
    [SerializeField] private float influenceDuration;
    [SerializeField] bool influenceByCollision;
    [SerializeField] private float influenceRadius;
    [SerializeField] LayerMask enemyLayer;
    private void OnTriggerEnter(Collider other)
    {
        if (!influenceByCollision) return;

        if (other.TryGetComponent(out EnemyVisualizer visualizer))
        {
            visualizer.Influence(influenceToMovementSpeed , influenceToAnimatorSpeed);
            HKDebugger.LogWorldText("Move speed : " + influenceToMovementSpeed , visualizer.transform.position + Vector3.up * 5f);
        }
    }

    public void InfluenceEnemiesInArea(Vector3 center)
    {
        Collider[] enemies = Physics.OverlapSphere(center, influenceRadius);
        foreach (var coll in enemies)
        {
            if(coll.TryGetComponent(out EnemyVisualizer enemyVisualizer))
            {
                enemyVisualizer.Influence(influenceToMovementSpeed, influenceToAnimatorSpeed , influenceDuration);
            }
        }
    }
}
