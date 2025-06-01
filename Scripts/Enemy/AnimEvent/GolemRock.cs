using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GolemRock : MonoBehaviour
{
    [SerializeField] MeleeEnemy golem;
    [SerializeField] GameObject cleave;
    [SerializeField] Transform cleaveShootPos;
    [SerializeField] SawTrap spikePrefab;
    [SerializeField] float durationBeforeSpike = 0.3f;
    [SerializeField] GameObject damageZone;
    public void SendCleave()
    {
        Vector3 shootPos = cleaveShootPos.position;
        Vector3 rayDir = PlayerHealth.Instance.transform.position - transform.position;
        rayDir.y = 0f;
        GameObject toShoot = Instantiate(cleave , cleaveShootPos.position , Quaternion.identity);
        toShoot.transform.rotation = Quaternion.FromToRotation(toShoot.transform.forward, rayDir);
        StartCoroutine(CleaveDamageRoutine(shootPos , rayDir));
    }

    private IEnumerator CleaveDamageRoutine(Vector3 cleaveStartPos , Vector3 cleaveStartDir)
    {
        yield return new WaitForSeconds(0.2f);

        Vector3 diffToPlayerFromCleaveStartPos = PlayerHealth.Instance.transform.position - cleaveStartPos;
        diffToPlayerFromCleaveStartPos.y = cleaveStartDir.y;
        float angleFromStart = Vector3.Angle(diffToPlayerFromCleaveStartPos, cleaveStartDir);

        if (Vector3.Distance(cleaveStartPos , PlayerHealth.Instance.transform.position) < 20f && angleFromStart < 17.5f)
        {
            PlayerHealth.Instance.TakeDamage(golem.Damage);
        }
    }

    public void ElevateTrap()
    {
        golem.FaceToTarget(PlayerHealth.Instance.TargetedPosition.position , lookInstantly:true);

        var posList = GetFormationRandomly();

        foreach (var pos in posList)
        {
            GameObject damageZoneIdentifier = Instantiate(damageZone, pos + Vector3.up * 0.5f, damageZone.transform.rotation);
            Destroy(damageZoneIdentifier , 1.2f);
        }       

        StartCoroutine(SpikeRoutine(posList));
    }

    private IEnumerator SpikeRoutine(List<Vector3> positions )
    {
        List<SawTrap> traps = new();
        for (int i = 0; i < positions.Count; i++)
        {
            SawTrap trap = Instantiate(spikePrefab);
            traps.Add(trap);
            trap.gameObject.SetActive(false);
        }
               
        yield return new WaitForSeconds(durationBeforeSpike);

        traps.ForEach(trap => trap.gameObject.SetActive(true));

        for (int i = 0;i < traps.Count; i++)
        {
       //     traps[i].Activate(positions[i]);    
        }
    }

    // returns a square formation of grid positions or just a single grid
    public List<Vector3> GetFormationRandomly()
    {
        List<Vector3> formation = new List<Vector3>();
        bool useSquare = UnityEngine.Random.Range(-1f, 1f) < 0f;
        
        if (useSquare)
        {
            int[] square_x_add = new int[] { -1, -1, +1, 1 };
            int[] square_z_add = new int[] { -1, 1, -1, 1 };

            WorldGrid playerGrid = GridManager.Instance.GetClosestGridOnLocation(PlayerHealth.Instance.transform.position);

            Vector3 playerGridPosition = playerGrid.Position;
            formation.Add(playerGridPosition);

            for (int i = 0; i < square_x_add.Length; i++)
            {
                int targetGridX = playerGrid.coordinateX + square_x_add[i];
                int targetGridZ = playerGrid.coordinateZ + square_z_add[i];
                if (GridManager.Instance.HasValidGridOnCoordinate(targetGridX, targetGridZ))
                {
                    formation.Add(GridManager.Instance.GetGridOfCoordinate(targetGridX, targetGridZ).Position);
                }
            }
        }
             
        if(formation.Count == 0)
        {
            var positionsNearPlayer = GridManager.Instance.GetEmptyPositions().Where(pos => Vector3.Distance(pos, PlayerHealth.Instance.transform.position) < 5f).
                                 ToList();

            Vector3 randomPos = positionsNearPlayer[UnityEngine.Random.Range(0, positionsNearPlayer.Count)];
            formation.Add(randomPos);
        } 
       
        return formation;
    }
}
