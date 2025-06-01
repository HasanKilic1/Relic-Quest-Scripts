using MoreMountains.Feedbacks;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ReviveUI : MonoBehaviour
{
    PlayerStateMachine playerStateMachine;
    [SerializeField] GameObject panel;
    [SerializeField] TextMeshProUGUI remainingReviveChance;
    [SerializeField] Button ContinueByGemButton;
    [SerializeField] Button ContinueByAdButton;
    [SerializeField] int gemCost;
    [SerializeField] MMF_Player initializationFeedbacks;
    private int totalRevived;
    private readonly int maxReviveCount = 2;

    private void OnEnable()
    {
        PlayerHealth.OnPlayerDead += TryShowDelayed;
    }

    private void OnDisable()
    {
        PlayerHealth.OnPlayerDead -= TryShowDelayed;
    }

    private void Start()
    {
        panel.SetActive(false);
        playerStateMachine = PlayerHealth.Instance.GetComponent<PlayerStateMachine>();
    }

    private void TryShowDelayed()
    {
        if (HapticManager.instance != null) 
        {
            HapticManager.instance.StopImpulseImmediately();
        }
        Invoke(nameof(TryShow), 1f);
    }

    private void TryShow()
    {
        MMTimeManager.Instance.SetTimeScaleTo(0f);

        if (initializationFeedbacks) initializationFeedbacks.PlayFeedbacks();
        if (totalRevived < maxReviveCount)
        {            
            panel.SetActive(true);
            remainingReviveChance.text = "Remaining resurrection chance : "+(maxReviveCount - totalRevived).ToString();
            ContinueByGemButton.interactable = EconomyManager.Instance.HasEnoughGem(gemCost);            
            StartCoroutine(SetSelectedButton());
        }
        else
        {
            FinishGame();
        }
    }

    private IEnumerator SetSelectedButton()
    {
        float t = 0f;
        while(t < 0.5f)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        EventSystem.current.SetSelectedGameObject(ContinueByAdButton.gameObject);
    }

    public void SpendGemAndContinue()
    {        
        EconomyManager.Instance.SpendGem(gemCost);
        Continue();
    }

    public void WatchAdAndContinue()
    {
        //SHOW AD
        Continue();
    }
    private void Continue()
    {
        totalRevived++;    
        MMTimeManager.Instance.SetTimeScaleTo(1f);
        panel.SetActive(false);
        PlayerHealth.Instance.Revive();
    }

    public void FinishGame()
    {
        panel.SetActive(false);
        InGameUI.Instance.OpenEndGameUI();
    }
}
