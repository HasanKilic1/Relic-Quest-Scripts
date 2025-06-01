using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyLoginController : MonoBehaviour
{
    public List<RewardSO> dailyRewards;
    [SerializeField] Transform contentHolder;
    [SerializeField] TextMeshProUGUI InfoText;
    [SerializeField] Button exitButton;
    private void Start()
    {
       // claimButton.onClick.AddListener(ClaimAllAvailableRewards);
       // exitButton.onClick.AddListener(Exit);
    }

    private void CheckDate()
    {
        DateTime lastLoginDate = DateTime.Parse(SaveLoadHandler.Instance.GetPlayerData().DailyLoginDate);
    }

    private void Refresh()
    {
    }

    public void Exit()
    {
        Destroy(gameObject);
    }
}
