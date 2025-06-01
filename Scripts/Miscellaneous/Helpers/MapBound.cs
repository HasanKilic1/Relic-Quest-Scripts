using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBound : MonoBehaviour
{
    public static MapBound Instance {  get; private set; }

    [SerializeField] Collider[] boundColliders;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);
    }
    
    public bool Check(Vector3 position)
    {
        foreach (var collider in boundColliders)
        {
            if (collider.bounds.Contains(position))
            {
                return true;
            }
        }
        return false;
    }

    public Vector3 GetClosestPointInBounds(Vector3 position)
    {
        if (Check(position))
        {
            return position;
        }

        Vector3 closestPoint = boundColliders[0].ClosestPoint(position);
        float closestDistanceSqr = (closestPoint - position).sqrMagnitude;

        for (int i = 1; i < boundColliders.Length; i++)
        {
            Vector3 point = boundColliders[i].ClosestPoint(position);
            float distanceSqr = (point - position).sqrMagnitude;

            if (distanceSqr < closestDistanceSqr)
            {
                closestPoint = point;
                closestDistanceSqr = distanceSqr;
            }
        }

        return closestPoint;
    }
}
