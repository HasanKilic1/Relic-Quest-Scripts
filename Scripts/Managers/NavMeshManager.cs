using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshSurface))]
public class NavMeshManager : MonoBehaviour
{
    public static NavMeshManager Instance { get; private set; }
    private NavMeshSurface NavMeshSurface;

    private void Awake()
    {
        if(Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
        NavMeshSurface = GetComponent<NavMeshSurface>();
    }

    public Vector3 GetRandomPositionWithinRadius(Vector3 origin, float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += origin;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return origin; 
    }

    public Vector3 FindClosestNavMeshPosition(Vector3 targetPosition, float maxDistance = 100f)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPosition, out hit, maxDistance, NavMesh.AllAreas))
        {
            return hit.position;
        }
        else
        {
            return targetPosition;
        }
    }

}
