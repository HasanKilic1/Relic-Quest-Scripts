using MoreMountains.Feedbacks;
using System.Collections;
using UnityEngine;

public class ShadowRune : Rune
{
    PlayerStateMachine playerStateMachine;
    [SerializeField] GameObject positionMarker;
    [SerializeField] float markerMovementSpeed;
    [SerializeField] AnimationCurve movementCurve;
    [SerializeField] float movementDuration;
    [Header("Feedbacks")]
    [SerializeField] GameObject reachVfx;
    [SerializeField] MMF_Player MovementStartFeedbacks;
    [SerializeField] MMF_Player MovementFinishFeedbacks;

    private void Update()
    {
        if (isRunning)
        {
            //Vector2 movementVector = InputReader.Instance.GetMovementVector();
            //Vector3 movePos = positionMarker.transform.position + markerMovementSpeed * Time.unscaledDeltaTime * new Vector3(movementVector.x, 0f, movementVector.y);
            Vector3 movePos = positionMarker.transform.position + markerMovementSpeed * Time.unscaledDeltaTime * playerStateMachine.GetCameraRelativeMovementVector();
            if (MapBound.Instance.Check(movePos)) 
            { 
                positionMarker.transform.position = movePos;
            }       
        }
    }

    public override void Settle(RuneUser runeUser)
    {
        positionMarker.SetActive(false);
        playerStateMachine = runeUser.GetComponent<PlayerStateMachine>();   
    }
    public override void Run()
    {
        base.Run();       
        PlayerController.Instance.GetComponent<PlayerStateMachine>().inputClosed = true;
        positionMarker.SetActive(true);
        positionMarker.transform.position = PlayerHealth.Instance.transform.position + Vector3.up * 0.2f;
        MMTimeManager.Instance.SetTimeScaleTo(0.2f);
        StartCoroutine(StopRoutine());
    }

    public override void Stop()
    {
        base.Stop();
        positionMarker.SetActive(false);
        MMTimeManager.Instance.SetTimeScaleTo(1f);
        StartCoroutine(MoveToPosition());
    }

    private IEnumerator StopRoutine()
    {
        yield return new WaitForSeconds(maxDurationWhenEnabled);
        
        if (isRunning)
        {
            Stop();
        }
    }
    private IEnumerator MoveToPosition()
    {
        if (HapticManager.instance)
        {
            HapticManager.instance.EnableHaptics();
            HapticManager.instance.Impulse(0.2f , 0.2f , 0.1f);
        }
        Vector3 start = PlayerHealth.Instance.transform.position;
        Vector3 target = new(positionMarker.transform.position.x , start.y , positionMarker.transform.position.z);
        if (MovementStartFeedbacks) MovementStartFeedbacks.PlayFeedbacks();

        float t = 0f;
        while(t < movementDuration)
        {
            t += Time.deltaTime;
            float evaluated = movementCurve.Evaluate(t / movementDuration);
            PlayerController.Instance.transform.position = Vector3.Lerp(start, target, evaluated);
            yield return null;
        }

        PlayerController.Instance.GetComponent<PlayerStateMachine>().inputClosed = false;
        if(reachVfx != null)
        {
            Instantiate(reachVfx, target, reachVfx.transform.rotation);
        }
        if (MovementFinishFeedbacks) MovementFinishFeedbacks.PlayFeedbacks();
    }
}
