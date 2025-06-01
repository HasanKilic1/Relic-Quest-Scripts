using System.Collections;
using System.Linq;
using UnityEngine;

public class NotificationChecker : MonoBehaviour
{/*
    private void Start()
    {
        StartCoroutine(Check());
    }
    private IEnumerator Check()
    {
        while (true) 
        {
            if (ShouldNotifyUpgradePanel()) { NotificationSystem.Instance.TriggerNotification(NotificationSystem.Upgrade_Trigger);}
            if (ShouldNotifyChampionPanel()) { NotificationSystem.Instance.TriggerNotification(NotificationSystem.Champion_Trigger); }
            if (ShouldNotifyAchievementPanel()) { NotificationSystem.Instance.TriggerNotification(NotificationSystem.Achievement_Trigger); }
            yield return new WaitForSeconds(1);
        }
    }
    private bool ShouldNotifyUpgradePanel()
    {
        int upgradeCost = (int)(Mathf.Pow(SaveLoadHandler.Instance.GetPlayerData().totalUpgrades, 1.5f) * 500);
        return EconomyManager.Instance.HasEnoughCoin(upgradeCost);
    }
    
    private bool ShouldNotifyChampionPanel()
    {
        if(ChampionManager.Instance != null)
        {
            return ChampionManager.Instance.HasAnyBuyable() || ChampionManager.Instance.HasAnyUpgradable();
        }
        return false;
    }

    private bool ShouldNotifyAchievementPanel()
    {
        return SaveLoadHandler.Instance.GetPlayerData().Achievements.Any(achievement => achievement.isCompleted && !achievement.isPrizeTaken);
    }*/
}
