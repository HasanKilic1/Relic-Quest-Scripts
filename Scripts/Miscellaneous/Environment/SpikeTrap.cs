using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PositionAnimationer))]
[RequireComponent(typeof(BoxCollider))]
public class SpikeTrap : MonoBehaviour , IGridObject
{
    WorldGrid grid;
    PositionAnimationer positionAnimationer;

    public int DamageOverTime = 25;
    public float damageInterval = 0.3f;
    private float damageTimer;

    private void Awake()
    {
        positionAnimationer = GetComponent<PositionAnimationer>();
    }

    private void OnTriggerStay(Collider other)
    {
        if(Time.time > damageTimer)
        {
            if(other.TryGetComponent(out PlayerHealth playerHealth))
            {
                playerHealth.TakeDamage(DamageOverTime);
                damageTimer = Time.time + damageInterval;
            }
        }
    }
    public void SetGrid(WorldGrid grid)
    {
        this.grid = grid;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position + Vector3.up * 25f;
        positionAnimationer.startPoint = transform.position;
        positionAnimationer.endPoint = position;
        positionAnimationer.AnimateExternal(transform);
    }
    public void Disable()
    {
        positionAnimationer.startPoint = transform.position;
        positionAnimationer.endPoint = transform.position + Vector3.up * 20f;
        positionAnimationer.AnimateExternal(transform);

        grid?.Clear();
        StartCoroutine(DisableRoutine());
    }
    private IEnumerator DisableRoutine()
    {
        yield return new WaitForSeconds(positionAnimationer.duration);
        Destroy(gameObject);
    }

}