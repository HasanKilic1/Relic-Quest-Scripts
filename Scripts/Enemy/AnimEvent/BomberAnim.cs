using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BomberAnim : MonoBehaviour
{
    [SerializeField] Trajectory bomb;
    [SerializeField] Transform spawnPos;
    [SerializeField] GameObject bombVisual;
    ShooterEnemy bomberEnemy;
    private void Awake()
    {
        bomberEnemy = GetComponentInParent<ShooterEnemy>();
    }

    public void ShowVisual()
    {
        bombVisual?.SetActive(true);
    }
    public void HideVisual()
    {
        bombVisual?.SetActive(false);
    }

    public void ThrowBombToRandomPos()
    {
        float height = UnityEngine.Random.Range(0f, 0.2f);
        Trajectory trajectory = Instantiate(bomb, spawnPos.position, Quaternion.identity);
        trajectory.SetDamage(bomberEnemy.Damage);
        Vector3 targetedPoint = GetRandomPosNearPlayer();
        trajectory.Shoot(targetedPoint);
    }

    private Vector3 GetRandomPosNearPlayer()
    {
        Transform player = PlayerHealth.Instance.transform;
        return new Vector3(player.position.x + Random.Range(-5, 5), 0.1f, player.position.z + Random.Range(-5f, 5f));
    }

}
