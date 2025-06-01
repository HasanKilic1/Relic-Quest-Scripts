using MoreMountains.Feedbacks;
using System.Collections;
using UnityEngine;
[RequireComponent(typeof(HapticUser))]
public class PiercingDashRune : Rune
{
    private Transform player;

    [Header("Before Dash")]
    [SerializeField] GameObject marker;
    [SerializeField] private SequencedObjectEnabler directionShower;
    [SerializeField] MMF_Player feedbackBeforeDash;
    private HapticUser hapticUser;
    [Header("Dash")]
    [SerializeField] AnimationCurve dashCurve;
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashDistance;
    private Vector3 dashPosition;

    [Header("After Dash")]
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private int damage;
    [SerializeField] private float damageCheckInterval;
    [SerializeField] private float damageCheckCount;
    [SerializeField] private Transform vfxParent;
    [SerializeField] private ParticleSystem vfx;
    [SerializeField] MMF_Player feedbackAfterDash;


    [Header("Gizmo")]
    [SerializeField] Transform playerGizmo;

    private void Awake()
    {
        hapticUser = GetComponent<HapticUser>();
    }

    void Update()
    {
        if (isRunning)
        {
            dashPosition = MapBound.Instance.GetClosestPointInBounds(player.transform.position + player.transform.forward * dashDistance);
            marker.transform.position = dashPosition + Vector3.up * 0.5f;//player.position + player.transform.forward * dashDistance + Vector3.up * 0.5f;
            directionShower.transform.position = player.transform.position + player.transform.forward * 3f + Vector3.up * 0.5f;
            directionShower.transform.forward = player.transform.forward;
        }
    }

    public override void Settle(RuneUser runeUser)
    {
        player = runeUser.transform;
    }

    public override void Run()
    {
        base.Run();
        hapticUser.Play();
        marker.SetActive(true);
        directionShower.gameObject.SetActive(true);
        dashPosition = player.transform.position;
        directionShower.StartSequence();
        MMTimeManager.Instance.SetTimeScaleTo(0.2f);
    }

    public override void Stop()
    {
        base.Stop();

        marker.SetActive(false);
        directionShower.gameObject.SetActive(false);

        MMTimeManager.Instance.SetTimeScaleTo(1f);
        if(HapticManager.instance != null)
        {
            HapticManager.instance.StopImpulseImmediately();
        }
        StartCoroutine(Dash());
    }

    private IEnumerator Dash()
    {
        if (feedbackBeforeDash) feedbackBeforeDash.PlayFeedbacks();

        Vector3 dashStartPos = player.position;
        Vector3 dashEndPos = new(dashPosition.x, player.position.y, dashPosition.z);       

        float t = 0f;
        while (t < dashDuration)
        {
            t += Time.deltaTime;
            float evaluated = dashCurve.Evaluate(t / dashDuration);
            Vector3 playerPos = Vector3.Lerp(dashStartPos, dashEndPos, evaluated);
            player.position = playerPos;
            yield return null;
        }
        yield return null;

        if (feedbackAfterDash) feedbackAfterDash.PlayFeedbacks();

        Vector3 rayStartPos = new Vector3(dashStartPos.x, 1.2f, dashStartPos.z);
        Vector3 rayEndPos = new Vector3(player.position.x, 1.2f, player.position.z);

        StartCoroutine(DamageRoutine(rayStartPos, rayEndPos));
    }

    private IEnumerator DamageRoutine(Vector3 start , Vector3 end)
    {
        Ray ray = new Ray(start, (end - start).normalized);
        vfxParent.gameObject.SetActive(true);
        vfxParent.position = (end + start) / 2 + Vector3.up *0.1f;
        vfxParent.forward = (end - vfxParent.position).normalized;
        vfx.Play();

        for (int i = 0; i < damageCheckCount; i++)
        {
            yield return new WaitForSeconds(damageCheckInterval);

            CheckRays(ray);
        }
        vfxParent.gameObject.SetActive(false);
        vfx.Stop();
    }

    private void CheckRays(Ray ray)
    {

        Vector3 halfExtents = new Vector3(0.5f, 0.5f, dashDistance / 2); // Adjust these values as needed

        RaycastHit[] hits = Physics.BoxCastAll(ray.origin, halfExtents, ray.direction, Quaternion.identity, dashDistance, enemyLayer);
        foreach (var hit in hits)
        {
            if (hit.collider.TryGetComponent<EnemyHealth>(out var enemyHealth))
            {
                enemyHealth.TakeDamage(damage, Vector3.zero, isUnstoppableAttack: true);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (playerGizmo)
        {
            Vector3 endPosition = playerGizmo.position + playerGizmo.transform.forward * dashDistance;
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(endPosition, 1f);
        }
    }

}
