using System;
using UnityEngine;

public class RuneUser : MonoBehaviour
{
    PlayerStateMachine playerStateMachine;
    private RuneSO currentRuneSO;
    private Rune currentRune;
    private float runeCooldownTimer;
    private int currentRuneLifeTime;
    private void OnEnable()
    {
        InputReader.RunePressedAction += PlayRune;
        InputReader.RuneReleasedAction += StopRune;
        GameStateManager.OnLevelFinished += DecreaseLifetime;
    }   
    private void OnDisable()
    {
        InputReader.RunePressedAction -= PlayRune;
        InputReader.RuneReleasedAction -= StopRune;
        GameStateManager.OnLevelFinished -= DecreaseLifetime;
    }
    private void Awake()
    {
        playerStateMachine = GetComponent<PlayerStateMachine>();
    }
    public void SetRune(RuneSO runeSO)
    {
        currentRuneSO = runeSO;
        if (currentRune)
        {
            Destroy(currentRune.gameObject);
        }
        currentRune = Instantiate(currentRuneSO.RunePrefab);
        currentRune.Settle(this);
        currentRuneLifeTime = 3;

        InGameUI.Instance.ActivateRuneUI();
        InGameUI.Instance.SetRuneLifeTimeText(currentRuneLifeTime);
        Debug.Log("Rune settled : " + currentRuneSO.Name);
    }

    private void PlayRune()
    {
        if (currentRune != null && CanUse)
        {            
            currentRune.Run();
            runeCooldownTimer = Time.time + currentRuneSO.CoolDown;
            
            InGameUI.Instance.EnterInfoText(currentRuneSO.UsageText, 5f, isRuneInfo: true);
            InGameUI.Instance.EnterRuneCooldown(currentRuneSO.CoolDown);
        }
    }

    private void StopRune()
    {
        if (currentRune != null && currentRune.isRunning)
        {
            currentRune.Stop();
            InGameUI.Instance.CloseInfoPanel();
        }
    }

    private bool CanUse => Time.time > runeCooldownTimer && !currentRune.isRunning && !playerStateMachine.inputClosed;

    private void DecreaseLifetime(int obj)
    {
        currentRuneLifeTime--;
        if (currentRuneLifeTime <= 0) 
        {
            InGameUI.Instance.DeactivateRuneUI();
            currentRune = null;
        }
        InGameUI.Instance.SetRuneLifeTimeText(currentRuneLifeTime);
    }
}