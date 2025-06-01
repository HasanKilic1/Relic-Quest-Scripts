using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Messenger : GiftResource
{
    [SerializeField] List<TransactionData> givables;
    [SerializeField] List<TransactionData> takeables;

    [Header("Visual")]
    [SerializeField] GameObject modelObject;
    [SerializeField] MMF_Player initalizationFeedbacks;
    [SerializeField] MMF_Player movementFinishFeedbacks;
    [SerializeField] MMF_Player flyFeedbacks;

    PathMovement pathMovement;
    private bool readyToGive;
    BoxCollider boxCollider;
    private float speed = 30f;

    private void OnEnable()
    {
        GiftPanel.OnDecisionEnd += Fly;
    }

    private void OnDisable()
    {
        GiftPanel.OnDecisionEnd -= Fly;
    }

    private void Awake()
    {
        pathMovement = GetComponent<PathMovement>();
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.enabled = false;
    }
    private void Start() // TEST
    {
        Initialize();
    }

    private void Update()
    {
        if (readyToGive)
        {
            Vector3 look =PlayerHealth.Instance.transform.position - transform.position;
            look.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(look);
            modelObject.transform.rotation = Quaternion.Slerp(modelObject.transform.rotation, lookRotation, Time.deltaTime * 20f);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (readyToGive)
        {
            if(other.GetComponent<PlayerHealth>() != null)
            {
                Give();
                readyToGive = false;    
            }
        }
    }

    public override void Initialize()
    {       
        initalizationFeedbacks?.PlayFeedbacks();
        pathMovement.StartFollow(this.transform);
        pathMovement.SetPathPosition(GridManager.Instance.GetCenterPosition() + Vector3.up * 15f, 0);
        pathMovement.SetPathPosition(GridManager.Instance.GetCenterPosition(), pathMovement.path.Length - 1);
        CameraController.Instance.ChangeLookTarget(transform);
    }
   
    public void MovementFinish()
    {
        CameraController.Instance?.ChangeLookTarget(PlayerHealth.Instance.transform);
        movementFinishFeedbacks?.PlayFeedbacks();
        readyToGive = true;
        boxCollider.enabled = true;
        pathMovement.enabled = false;        
    }

    public override void Give()
    {
        Random.InitState(RandomSeeder.GetSeed());
        TransactionData random_give = givables[UnityEngine.Random.Range(0, givables.Count)];
        TransactionData random_take = takeables[UnityEngine.Random.Range(0, takeables.Count)];

        List<TransactionData> dealList = new List<TransactionData> {random_give , random_take};
        InGameUI.Instance.OpenGiftPanel(dealList);
    }

    private void Fly(object sender, System.EventArgs e)
    {
        StartCoroutine(FlyRoutine());
    }

    private IEnumerator FlyRoutine()
    {
        yield return new WaitForSeconds(1f);
        flyFeedbacks?.PlayFeedbacks();

        float flyDuration = 3f;
        float t = 0f;
        while(t < flyDuration)
        {
            t += Time.deltaTime;
            speed += 15f * Time.deltaTime;
            transform.position += Vector3.up * speed * Time.deltaTime;
            yield return null;
        }
        GameStateManager.Instance.StartNewLevel();
        Destroy(gameObject);
       
    }
}
