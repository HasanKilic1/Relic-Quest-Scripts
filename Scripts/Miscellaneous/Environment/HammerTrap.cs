using MoreMountains.Feedbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(PositionAnimationer))]
public class HammerTrap : MonoBehaviour , IGridObject
{
    WorldGrid grid;
    Animator animator;
    PositionAnimationer positionAnimationer;
    [SerializeField] bool useWorldPosition;
    [SerializeField] Vector3 worldPosition;
    [Header("Animation")]
    [SerializeField] Transform hammer;
    [SerializeField] float heightAtStart = 15f;
    [SerializeField] AnimationCurve swingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] AnimationCurve swingResetCurve = AnimationCurve.Linear(0,0, 1, 1);
    [SerializeField] float waitBeforeSwing = 0.3f;
    [SerializeField] float swingCooldown = 4f;
    [SerializeField] float swingDuration = 0.5f;
    [SerializeField] float swingResetDuration = 0.25f;
    [SerializeField] float swingRotationZ = 75f;

    [Header("Combat")]
    [SerializeField] int damage;
    [SerializeField] Transform checkPoint;
    [SerializeField] float checkRadius;
    [SerializeField] LayerMask playerLayer;
    private bool damageGiven;

    [Header("Feedbacks")]
    [SerializeField] MMF_Player beforeSwingFb;
    [SerializeField] MMF_Player swingFinishFb;
    private float timer;
    private bool isSwinging = false;

    private void Awake()
    {
        positionAnimationer = GetComponent<PositionAnimationer>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > swingCooldown) 
        {
            StartSwing();
        }
        if (!isSwinging) 
        {
            LookToPlayer();
        }
    }

    private void LookToPlayer()
    {
        Vector3 diff = PlayerController.Instance.transform.position - transform.position;
        diff.y = 0f;
        transform.rotation = Quaternion.Slerp(transform.rotation , Quaternion.LookRotation(diff) , Time.deltaTime * 5f);
    }

    private void StartSwing()
    {
        timer = 0f;
        StartCoroutine(SwingRoutine());
    }

    private IEnumerator SwingRoutine()
    {
        if (beforeSwingFb != null) { beforeSwingFb.PlayFeedbacks(); }
        isSwinging = true;
        yield return new WaitForSeconds(waitBeforeSwing);
        
        float t = 0f;
        Quaternion startRotation = hammer.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(startRotation.eulerAngles.x, startRotation.eulerAngles.y, swingRotationZ)); // Swing only on Z axis

        while (t < swingDuration)
        {
            t += Time.deltaTime;
            float evaluated = swingCurve.Evaluate(t / swingDuration);
            hammer.transform.rotation = Quaternion.LerpUnclamped(startRotation, targetRotation, evaluated);
            if(t > swingDuration * 1 / 2)
            {
                Check();
            }
            yield return null;
        }

        if (swingFinishFb != null) { swingFinishFb.PlayFeedbacks(); }
        
        yield return new WaitForSeconds(0.15f);

        t = 0f; // Reset t for the return animation
                // Reset the hammer back to the start position
        while (t < swingResetDuration)
        {
            t += Time.deltaTime;
            float evaluated = swingResetCurve.Evaluate(t / swingResetDuration);
            hammer.transform.rotation = Quaternion.LerpUnclamped(targetRotation, startRotation, evaluated);
            yield return null;
        }

        isSwinging = false;
        damageGiven = false;
    }
    private void Check()
    {
        if (!damageGiven)
        {
            Collider[] colls = Physics.OverlapSphere(checkPoint.position, checkRadius, playerLayer);
            foreach (var col in colls)
            {
                if(col.TryGetComponent(out PlayerHealth playerHealth))
                {
                    playerHealth.TakeDamage(damage);
                    damageGiven = true;
                }
            }
        }
    }
    
    public void Disable()
    {
        positionAnimationer.startPoint = transform.position;
        positionAnimationer.endPoint = transform.position + Vector3.up * heightAtStart;
        positionAnimationer.Animate();
        Destroy(gameObject , positionAnimationer.duration);
    }

    public void SetGrid(WorldGrid grid)
    {
        this.grid = grid;   
    }

    public void SetPosition(Vector3 position)
    {
        if (useWorldPosition)
        {
            positionAnimationer.startPoint = worldPosition + Vector3.up * heightAtStart;
            positionAnimationer.endPoint = worldPosition;
        }
        else
        {
            positionAnimationer.startPoint = position + Vector3.up * heightAtStart;
            positionAnimationer.endPoint = position;
        }        
        positionAnimationer.Animate();
    }

    
}
