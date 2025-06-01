using System.Collections;
using UnityEngine;

[RequireComponent (typeof(PathLooper))]
public class SawTrap : MonoBehaviour, IGridObject
{
    PathLooper pathLooper;

    [SerializeField] private int damage;

    private void Awake()
    {
        pathLooper = GetComponent<PathLooper>();
        pathLooper.ContinuePath = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerHealth playerHealth))
        {
            playerHealth.TakeDamage(damage);
        }
    }

    public void Activate(Vector3 position)
    {
       
    }
    private IEnumerator Move(Vector3 targetPos , float duration , bool continuePath)
    {
        Vector3 start = transform.position;
        float t = 0f;
        while(t < duration)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(start, targetPos, t / duration);
            yield return null;
        }
        pathLooper.ContinuePath = continuePath;
    }

    public void Disable()
    {
        pathLooper.ContinuePath = false;
        StartCoroutine(Move(transform.position - Vector3.up * 10f, duration:2f, false));
        Destroy(gameObject , 2f);
    }

    public void SetGrid(WorldGrid grid)
    {}

    public void SetPosition(Vector3 position)
    {
        transform.position = pathLooper.waypoints[0] - Vector3.up * 5f;
        StartCoroutine(Move(pathLooper.waypoints[0], 1, true));
    }

}
