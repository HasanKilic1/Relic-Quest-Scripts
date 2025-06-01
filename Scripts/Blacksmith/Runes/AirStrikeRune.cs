using MoreMountains.Feedbacks;
using System.Collections;
using UnityEngine;
[RequireComponent(typeof(HapticUser))]
public class AirStrikeRune : Rune
{
    private HapticUser hapticUser;
    private PlayerStateMachine playerStateMachine;
    Transform player;
    [SerializeField] Transform marker;
    [SerializeField] float markerMovementSpeed;
    [SerializeField] GameObject airStrikeVfx;
    [SerializeField] float timeBetweenAttacks;
    [SerializeField] int perAttackDamage;
    [SerializeField] int attackCount;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] float checkRadius;

    private void Awake()
    {
        hapticUser = GetComponent<HapticUser>();
    }
    private void Update()
    {
        if (isRunning)
        {
            //Vector2 movementVector = InputReader.Instance.GetMovementVector();
            //Vector3 movePos = marker.transform.position + markerMovementSpeed * Time.unscaledDeltaTime * new Vector3(movementVector.x, 0f, movementVector.y);
            Vector3 movePos = marker.transform.position + markerMovementSpeed * Time.unscaledDeltaTime * playerStateMachine.GetCameraRelativeMovementVector();
            movePos.y = 1f;
            if (MapBound.Instance.Check(movePos))
            {
                marker.transform.position = movePos;
            }
        }
    }
    public override void Settle(RuneUser runeUser)
    {
        player = runeUser.transform;
        playerStateMachine = player.GetComponent<PlayerStateMachine>();
        marker.gameObject.SetActive(false);
    }

    public override void Run()
    {
        base.Run();
        marker.transform.position = PlayerController.Instance.transform.position;
        marker.gameObject.SetActive(true);
        MMTimeManager.Instance.SetTimeScaleTo(0.2f);
    }

    public override void Stop()
    {
        base.Stop();

        marker.gameObject.SetActive(false);
        MMTimeManager.Instance.SetTimeScaleTo(1f);
        hapticUser.Play();
        StartCoroutine(StartAirStrike());
    }

    private IEnumerator StartAirStrike()
    {
        GameObject vfx = Instantiate(airStrikeVfx, marker.position, Quaternion.identity);
        yield return new WaitForSeconds(0.15f);

        for (int i = 0; i < attackCount; i++) 
        {               
            yield return new WaitForSeconds(timeBetweenAttacks);
            CheckDamage();
        }
    }

    private void CheckDamage()
    {
        Collider[] colls = Physics.OverlapSphere(marker.transform.position, checkRadius);

        foreach (var collider in colls)
        {
            if(collider.TryGetComponent(out EnemyHealth enemyHealth))
            {
                enemyHealth.TakeDamage(perAttackDamage , Vector3.zero , isUnstoppableAttack:true);
            }
        }
    }
}
