using MoreMountains.Feedbacks;
using System.Collections;
using UnityEngine;

[RequireComponent (typeof(PositionAnimationer))]
public class ShooterTrap : MonoBehaviour , IGridObject
{
    WorldGrid grid;
    PositionAnimationer positionAnimationer;
    [SerializeField] EnemyBallistic ballistic;
    [SerializeField] int damage;
    [SerializeField] float coolDown;
    [SerializeField] Transform shootPosition;
    [SerializeField] int shootCount;
    [SerializeField] float timeBetweenShots;

    [Header("Player Look")]
    [SerializeField] bool lookPlayer = true;
    [SerializeField] bool ignoreY = true;
    [SerializeField] float playerFollowSens = 1f;

    [Header("Feedbacks")]
    [SerializeField] MMF_Player shootFeedbacks;

    private float timeToShoot;
    private bool canShoot = false;

    private void OnEnable()
    {
        GameStateManager.OnLevelFinished += StopAttacks;
    }
    private void OnDestroy()
    {
        GameStateManager.OnLevelFinished -= StopAttacks;
    }

    private void Awake()
    {
        positionAnimationer = GetComponent<PositionAnimationer>();       
    }

    private void Start()
    {
        positionAnimationer.OnFinish.AddListener(MakeReadyToShoot);
    }

    private void StopAttacks(int obj)
    {
        canShoot = false;
    }

    private void MakeReadyToShoot() => canShoot = true;

    void Update()
    {
        if(lookPlayer)
        {
            LookPlayer();
        }
        if(Time.time > timeToShoot && canShoot)
        {
            StartShooting();
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
        positionAnimationer.OnFinish.RemoveListener(MakeReadyToShoot);
        positionAnimationer.startPoint = transform.position;
        positionAnimationer.endPoint = transform.position + Vector3.up * 20f;
        positionAnimationer.AnimateExternal(transform);

        canShoot = false;
        grid.Clear();
        StartCoroutine(DisableRoutine());
    }
    private IEnumerator DisableRoutine()
    {
        yield return new WaitForSeconds(positionAnimationer.duration);
        Destroy(gameObject);
    }
    private void LookPlayer()
    {
        Vector3 diff = PlayerController.Instance.transform.position - transform.position;
        if(ignoreY) diff.y = 0;
        Quaternion toLook = Quaternion.LookRotation(diff);
        transform.rotation = Quaternion.Lerp(transform.rotation , toLook , playerFollowSens * Time.deltaTime);
    }
    
    private void StartShooting()
    {
        timeToShoot = Time.time + coolDown;
        if(shootCount == 1)
        {
            shootFeedbacks?.PlayFeedbacks();
            Invoke(nameof(ShootSingle), timeBetweenShots);
        }
        else
        {
            ShootMultiple();
        }
    }

    private void ShootMultiple()
    {
        StartCoroutine(MultipleShoot());
    }

    private IEnumerator MultipleShoot()
    {
        for (int i = 0; i < shootCount; i++)
        {
            shootFeedbacks?.PlayFeedbacks();
            yield return new WaitForSeconds(timeBetweenShots);
            ShootSingle();
        }
    }

    private void ShootSingle()
    {
        EnemyBallistic ballistic_ = Instantiate(ballistic, shootPosition.position, ballistic.transform.rotation);
        ballistic_.SetDamage(damage);
        ballistic_.SetTarget(PlayerController.Instance.transform);
        ballistic.Shoot(PlayerController.Instance.transform.position);
    }

   
}
