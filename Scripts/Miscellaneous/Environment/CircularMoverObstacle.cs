using UnityEngine;

[RequireComponent(typeof(CircularMover))]
public class CircularMoverObstacle : MonoBehaviour, IGridObject
{
    CircularMover circularMover;
    PositionAnimationer positionAnimationer;
    [SerializeField] int trapDamage = 20;
    [SerializeField] private float damageInterval = 0.25f;
    private float damageTimer;
    private void Awake()
    {
        circularMover = GetComponent<CircularMover>();
        positionAnimationer = GetComponent<PositionAnimationer>();
    }
    private void Start()
    {
        positionAnimationer.OnFinish.AddListener(delegate { circularMover.isStopped = false; });
    }
    public void Disable()
    {
        Destroy(gameObject);
    }

    public void SetGrid(WorldGrid grid)
    {}

    public void SetPosition(Vector3 position)
    {
        positionAnimationer.startPoint = circularMover.path[0] + Vector3.up * 30f;
        positionAnimationer.endPoint = circularMover.path[0];
        positionAnimationer.AnimateExternal(transform);
        circularMover.isStopped = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if(!DamageValid) return;

        if(other.TryGetComponent(out PlayerHealth player))
        {
            player.TakeDamage(trapDamage);
            damageTimer = Time.time + damageInterval;
        }
    }

    private bool DamageValid => Time.time > damageTimer;
}
