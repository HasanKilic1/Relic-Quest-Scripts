using MoreMountains.Feedbacks;
using System.Collections;
using UnityEngine;

public class SpiritDemonBoss : Boss
{
    [Header("Teleport Attack")]
    [SerializeField] int teleportCount = 3;
    [SerializeField] float timeBetweenTeleports = 1f;
    [SerializeField] float radius;
    [SerializeField] int teleportDamage;
    [SerializeField] GameObject teleportAreaVisualPrefab;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] MMF_Player teleportFeedbacks;

    private GameObject teleportAreaVisual;
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        teleportAreaVisual = Instantiate(teleportAreaVisualPrefab);
        teleportAreaVisual.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Attack()
    {
        base.Attack();
        if (CurrentPhase.Tier == Tier.T1)
        {
            StartCoroutine(TeleportRoutine());
        }
    }

    private IEnumerator TeleportRoutine()
    {
        for (int i = 0; i < teleportCount; i++)
        {
            Vector3 targetPosition = GridManager.Instance.GetRandomValidGridPosition(transform , 2 , 2);
            teleportAreaVisual.SetActive(true);
            teleportAreaVisual.transform.position = targetPosition + Vector3.up * 0.3f;

            yield return new WaitForSeconds(timeBetweenTeleports);

            Animator.SetTrigger("Attack1");
            agent.Warp(targetPosition);
            GiveDamageOnArea();

            teleportAreaVisual.SetActive(false);
            teleportFeedbacks?.PlayFeedbacks();
        }
    }

    private void GiveDamageOnArea()
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, radius , playerLayer);
        foreach (var coll in colls)
        {
            if(coll.TryGetComponent(out PlayerHealth playerHealth))
            {
                playerHealth.TakeDamage(teleportDamage);
            }
        }
    }
}
