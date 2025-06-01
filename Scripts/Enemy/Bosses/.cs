using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkElfBoss : Boss
{
    [SerializeField] int teleportCount = 3;
    [SerializeField] float timeBetweenTeleports = 1f;
    [SerializeField] float teleportDistance = 10f;
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Attack()
    {
        base.Attack();
        if(currentPhase.Tier == Tier.T1)
        {
            StartCoroutine(TeleportRoutine());
        }
    }

    private IEnumerator TeleportRoutine()
    {
        for (int i = 0; i < teleportCount; i++)
        {
            Animator.SetTrigger("Attack1");
            Vector3 targetPosition = NavMeshManager.Instance.GetRandomPositionWithinRadius(player.transform.position, teleportDistance);
            yield return new WaitForSeconds(timeBetweenTeleports);
            transform.position = targetPosition;
        }
    }
}
