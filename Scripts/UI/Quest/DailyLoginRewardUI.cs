using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyLoginRewardUI : MonoBehaviour
{
    DailyLoginController controller;
    public RewardSO dailyRewardSO;
    public int Day = 1;

    [Header("UI")]
    [SerializeField] Button CollectButton;
    [SerializeField] Image icon;
    [SerializeField] GameObject isClaimedPanel;
    [SerializeField] GameObject readyToClaimPanel;
    [SerializeField] TextMeshProUGUI dayOfMissionText;
    [SerializeField] TextMeshProUGUI rewardAmountText;
    private void Start()
    {
        if (CollectButton != null) { CollectButton.onClick.AddListener(Claim); }
        CheckRewardStatus();
    }
   
    public void SetDailyReward(DailyLoginController dailyMissionController)
    {
        this.controller = dailyMissionController;
        CheckRewardStatus();        
    }

    public void Claim()
    {
        if (!ReadyToClaim) return;
        SaveLoadHandler.Instance.GetPlayerData().rewardTakenDays.Add(Day);
        GiveReward();
        SaveLoadHandler.Instance.SaveData();
        CheckRewardStatus();
    }

    private void GiveReward()
    {
        switch (dailyRewardSO.resourceType)
        {
            case ResourceType.Coin:
                EconomyManager.Instance.AddCoin(dailyRewardSO.GetRewardAmount());
                break;
            case ResourceType.Gem:
                EconomyManager.Instance.AddGem(dailyRewardSO.GetRewardAmount());
                break;
            case ResourceType.SilverKey:
                EconomyManager.Instance.AddSilverKey(dailyRewardSO.GetRewardAmount());
                break;
            case ResourceType.GoldenKey:
                EconomyManager.Instance.AddGoldenKey(dailyRewardSO.GetRewardAmount());
                break;
            case ResourceType.EtherealStone:
                EconomyManager.Instance.AddEtherealStone(dailyRewardSO.GetRewardAmount());
                break;
        }
    }

    public void CheckRewardStatus()
    {
        rewardAmountText.text = dailyRewardSO.GetRewardAmount().ToString();
        readyToClaimPanel.SetActive(ReadyToClaim);
        isClaimedPanel.SetActive(IsClaimed());        
    }

    public bool ReadyToClaim => !IsClaimed() && SaveLoadHandler.Instance.GetPlayerData().continuousLoginDays >= Day;
    public bool IsClaimed()
    {
        return SaveLoadHandler.Instance.GetPlayerData().rewardTakenDays.Contains(Day); 
    }
}
