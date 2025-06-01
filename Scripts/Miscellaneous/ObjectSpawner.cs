using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public void SpawnStoredPositions(List<GameObject> objects , List<Vector3> positions, bool spawnDelayed = false , float interval = 0.5f)
    {
        if(objects.Count == 0 || positions.Count == 0) return;
        if(spawnDelayed)
        {
            StartCoroutine(ObjectSpawnRoutine(objects, positions, interval));
        }
        else
        {
            for (int i = 0; i < objects.Count; i++)
            {
                GameObject obj = Instantiate(objects[i], positions[i], Quaternion.identity);
            }
        }
    }
    public void SpawnToValidGridPositions(List<GameObject> objects , out List<Vector3> placements,Vector3 offset, bool spawnDelayed = false, float interval = 0.5f)
    {        
        placements = new List<Vector3>();

        for (int i = 0;i < objects.Count;i++)
        {
            Vector3 validPos = GetValidPos();
            placements.Add(validPos);
            if (!spawnDelayed)
            {
                Instantiate(objects[i], validPos + offset, Quaternion.identity);
            }
            
        }
        if (spawnDelayed)
        {
            StartCoroutine(ObjectSpawnRoutine(objects , placements , interval));                
        }
    }

    private IEnumerator ObjectSpawnRoutine(List<GameObject> objects ,List<Vector3> positions , float interval)
    {
        if(objects.Count > 0)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                GameObject obj = Instantiate(objects[i], positions[i], Quaternion.identity);
                yield return new WaitForSeconds(interval);
            }
        }      
    }

    private Vector3 GetValidPos()
    {
        return GridManager.Instance.GetEmptyPositions()[UnityEngine.Random.Range(0, GridManager.Instance.GetEmptyPositions().Count)];
    }
}


