using MoreMountains.Feedbacks;
using System.Collections;
using UnityEngine;

public class MultipleShooter : MonoBehaviour
{
    public bool TakeRandomShootPosition = true;
    public Transform[] ShootPositions;
    public EnemyBallistic Ballistic;
    public float ShootDelay = 1f;
    public int shootCount = 10;
    public float ShootInterval = 0.3f;
    public int Damage;
    public bool LookPlayer;
    private bool isShooting;
    public bool DestroyOnShootFinish = true;
    
    [SerializeField] MMF_Player shootFeedbacks;
    private void Update()
    {
        if(LookPlayer)
        {
            Vector3 diff = PlayerHealth.Instance.transform.position - transform.position;
            Quaternion look = Quaternion.LookRotation(diff);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, 2.5f);
        }
    }
    public void StartShootingDelayed(float delay)
    {
        Invoke(nameof(StartShooting), delay);
    }
    public void StartShooting()
    {
        if(isShooting) { return; }
        StartCoroutine(ShootRoutine());
    }

    private IEnumerator ShootRoutine()
    {
        isShooting = true;
        yield return new WaitForSeconds(ShootDelay);
        for (int i = 0; i < shootCount; i++)
        {
            int positionIndex = GrabIndex(i);
            CreateBallistic(ShootPositions[positionIndex]);

            if (shootFeedbacks) shootFeedbacks.PlayFeedbacks();
            yield return new WaitForSeconds(ShootInterval);

        }
        isShooting = false;
        if(DestroyOnShootFinish)
        {
            Destroy(gameObject , 1.5f);
        }
    }

    private int GrabIndex(int i)
    {
        int positionIndex = i;

        if (i > ShootPositions.Length - 1)
        {
            positionIndex = 0;
        }

        if (TakeRandomShootPosition)
        {
            positionIndex = Random.Range(0, ShootPositions.Length);
        }

        return positionIndex;
    }

    private void CreateBallistic(Transform shootPosition)
    {
        EnemyBallistic ballistic = Instantiate(Ballistic, shootPosition.position, Quaternion.identity);
        Vector3 diff = PlayerHealth.Instance.transform.position - shootPosition.position;
        ballistic.SetTarget(PlayerHealth.Instance.transform);
        ballistic.SetDamage(Damage);        
        ballistic.Shoot(diff.normalized);
    }

    public void SetDamage(int Damage) => this.Damage = Damage;
}
