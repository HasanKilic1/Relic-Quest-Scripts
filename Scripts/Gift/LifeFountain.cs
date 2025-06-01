using MoreMountains.Feedbacks;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class LifeFountain : GiftResource
{
    PathMovement pathMovement;
    [SerializeField] ParticleSystem portalPrefab;
    ParticleSystem portal;
    [SerializeField] MMF_Player initFeedbacks;
    [SerializeField] MMF_Player stopFeedbacks;
    [SerializeField] MMF_Player giveFeedbacks;
    [SerializeField] MMF_Player flyFeedBacks;
    BoxCollider boxCollider;
    bool readyToGive = false;
    bool canGo = false;
    float flySpeed = 30f;
    int healthRestore;
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        pathMovement = GetComponent<PathMovement>();
        boxCollider.enabled = false;
        portal = Instantiate(portalPrefab);
    }

    private void Start()
    {
        Initialize();        
    }

    private void Update()
    {
        if(canGo)
        {
            GoGround();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(readyToGive && other.GetComponent<PlayerHealth>())
        {
            Give();
            StartCoroutine(EndSequence());
        }
    }
    public override void Initialize()
    {
        initFeedbacks?.PlayFeedbacks();
        healthRestore = (int)(PlayerHealth.Instance.GetMaxHealth / 5);
        Vector3 height = Vector3.up * 30f;
        Vector3 center = GridManager.Instance.GetCenterPosition();
        pathMovement.SetPathPosition( center - height, 0);
        pathMovement.SetPathPosition(center , pathMovement.path.Length - 1);
        portal.transform.position = center + Vector3.up;
        portal.Play();
        pathMovement.StartFollow(transform);
    }
    public void OnStop()
    {
        readyToGive = true;
        boxCollider.enabled = true;
        boxCollider.isTrigger = true;
        stopFeedbacks?.PlayFeedbacks();
        portal.Stop();
    }
    public override void Give()
    {
        PlayerHealth.Instance.IncreaseHealth(healthRestore);
        giveFeedbacks?.PlayFeedbacks();
        readyToGive = false;
    }
       
    private IEnumerator EndSequence()
    {
        portal.Play();
        flyFeedBacks?.PlayFeedbacks();
        yield return new WaitForSeconds(2f);
        
        Destroy(portal.gameObject);
        canGo = true;

        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
        GameStateManager.Instance.StartNewLevel();
    }

    private void GoGround()
    {
        flySpeed += Time.deltaTime * 10f;
        transform.position -= flySpeed * Time.deltaTime * Vector3.up;
    }

}
