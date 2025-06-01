using MoreMountains.Feedbacks;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HapticUser))]
public class CrossFireRune : Rune
{
    private HapticUser hapticUser;
    public struct ShootOrientation
    {
        public Vector3 Position { get; set; }
        public Vector3 Forward { get; set; }

        public ShootOrientation(Vector3 position, Vector3 forward)
        {
            Position = position;
            Forward = forward;
        }
    }

    Transform player;
    [SerializeField] Transform arrowParent;
    [SerializeField] Projectile projectile;
    [SerializeField] int projectileDamage;
    [SerializeField] SequencedObjectEnabler arrow;
    [SerializeField] int arrowCount;
    [SerializeField] float arrowSpawnInterval;
    [SerializeField] MMF_Player shootFeedbacks;
    
    private List<SequencedObjectEnabler> arrows;
    private List<ShootOrientation> ShootOrientations;
    int createdArrow = 0;
    private float runeTimer;
    private float arrowTimer;

    private void Awake()
    {
        ShootOrientations = new List<ShootOrientation>();
        arrows = new List<SequencedObjectEnabler>();
        hapticUser = GetComponent<HapticUser>();
    }

    private void Update()
    {
        if (isRunning)
        {
            arrowTimer += Time.unscaledDeltaTime;
            if (arrowTimer > arrowSpawnInterval && createdArrow < arrowCount) 
            { 
                arrowTimer = 0;
                CreateArrowAngleRelatively();
            }
            arrowParent.transform.position = player.position + Vector3.up * 1.5f;
            runeTimer += Time.unscaledDeltaTime;
            if(runeTimer > maxDurationWhenEnabled)
            {
                Stop();
            }
        }       
    }

    public override void Settle(RuneUser runeUser)
    {
        player = runeUser.transform;
    }

    public override void Run()
    {
        base.Run();
     //   player.GetComponent<PlayerStateMachine>().inputClosed = true;
        MMTimeManager.Instance.SetTimeScaleTo(0.75f);
        hapticUser.Play();
    }

    public override void Stop()
    {       
        base.Stop();
     //   player.GetComponent<PlayerStateMachine>().inputClosed = false;
        foreach (var item in arrows)
        {
            Destroy(item.gameObject);
        }
        arrows.Clear();

        SendProjectiles();
        createdArrow = 0;
        arrowTimer = 0f;
        runeTimer = 0f;
        MMTimeManager.Instance.SetTimeScaleTo(1f);
    }

    private void CreateArrowAngleRelatively()
    {        
        Vector3 firstSpawnPosition = player.position + arrowParent.forward * 3f;
        Vector3 diff = firstSpawnPosition - player.position;

        float angle = 360f / arrowCount;
        Vector3 rotatedDiff = Quaternion.AngleAxis(angle * createdArrow, Vector3.up) * diff;
        SequencedObjectEnabler objectEnabler = Instantiate(arrow, arrowParent.position, Quaternion.identity);
               
        objectEnabler.transform.forward = rotatedDiff.normalized;
        objectEnabler.StartSequence();
        objectEnabler.transform.SetParent(arrowParent);
        ShootOrientation orientation = new ShootOrientation(objectEnabler.transform.position + Vector3.up * 1f + objectEnabler.transform.forward * 3f, 
                                                            objectEnabler.transform.forward);
        ShootOrientations.Add(orientation);
        arrows.Add(objectEnabler);

        createdArrow++;
    }

    private void SendProjectiles()
    {
        if (ShootOrientations.Count > 0)
        {
            foreach (var shootOrientation in ShootOrientations)
            {
                Projectile spawned = Instantiate(projectile, arrowParent.position, Quaternion.identity);
                spawned.transform.forward = shootOrientation.Forward;
                spawned.SetDirection(shootOrientation.Forward);
            }
        }
        if(shootFeedbacks != null)
        {
            shootFeedbacks.PlayFeedbacks();
        }
        ShootOrientations.Clear();
    }
}
