using System.Collections;
using System.Linq;
using UnityEngine;

public class NotificationSystem : MonoBehaviour
{
    public static NotificationSystem Instance {  get; private set; }
    [SerializeField] NotificationVisual shopNotifier;
    [SerializeField] NotificationVisual inventoryNotifier;
    [SerializeField] NotificationVisual upgradeNotifier;
    [SerializeField] NotificationVisual achievementNotifier;
    [SerializeField] NotificationVisual championNotifier;
    
    public static readonly string Shop_Trigger = "Shop";
    public static readonly string Inventory_Trigger = "Inventory";
    public static readonly string Upgrade_Trigger = "Upgrade";
    public static readonly string Achievement_Trigger = "Achievement";
    public static readonly string Champion_Trigger = "Champion";

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(CheckNotificationRoutine());
    }

    public void TriggerNotification(string key)
    {
        PlayerData playerData = SaveLoadHandler.Instance.GetPlayerData();
        if (key == Shop_Trigger)
        {
            playerData.notification.ShopNotification = true;
            shopNotifier.StartNotifying();
        }
        else if (key == Inventory_Trigger)
        {
            playerData.notification.ShopNotification = true;
            inventoryNotifier.StartNotifying();
        }
        else if (key == Upgrade_Trigger)
        {
            playerData.notification.ShopNotification = true;
            upgradeNotifier.StartNotifying();
        }
        else if (key == Achievement_Trigger) 
        {
            playerData.notification.ShopNotification = true;
            achievementNotifier.StartNotifying();
        }
        else if(key == Champion_Trigger)
        {
            playerData.notification.ShopNotification = true;
            championNotifier.StartNotifying();
        }
    }

    public void ResetNotificationTrigger(string key)
    {
        PlayerData playerData = SaveLoadHandler.Instance.GetPlayerData();
        if (key == Shop_Trigger)
        {
            playerData.notification.ShopNotification = false;
            shopNotifier.StopNotifying();
        }
        else if (key == Inventory_Trigger)
        {
            playerData.notification.InventoryNotification = false;
            inventoryNotifier.StopNotifying();
        }
        else if (key == Upgrade_Trigger)
        {
            playerData.notification.UpgradeNotification = false;
            upgradeNotifier.StopNotifying();
        }
        else if (key == Achievement_Trigger)
        {
            playerData.notification.AchievementNotification = false;
            achievementNotifier.StopNotifying();
        }
        else if (key == Champion_Trigger)
        {
            playerData.notification.ChampionNotification = false;
            championNotifier.StopNotifying();
        }
        SaveLoadHandler.Instance.SaveData();
    }

    private void CheckSavedNotifications()
    {
        PlayerData playerData = SaveLoadHandler.Instance.GetPlayerData();

        if (playerData.notification.ShopNotification || ShouldNotifyShopPanel()) { TriggerNotification(Shop_Trigger); }
        if (playerData.notification.InventoryNotification) { TriggerNotification(Inventory_Trigger); }
        if (playerData.notification.UpgradeNotification || ShouldNotifyUpgradePanel()) { TriggerNotification(Upgrade_Trigger); }
        if (playerData.notification.AchievementNotification || ShouldNotifyAchievementPanel()) { TriggerNotification(Achievement_Trigger); }
        if (playerData.notification.ChampionNotification || ShouldNotifyChampionPanel()) { TriggerNotification(Champion_Trigger); }
    }

    private IEnumerator CheckNotificationRoutine()
    {
        while (true)
        {
            CheckSavedNotifications();
            yield return new WaitForSeconds(60f);
        }
    }

    private bool ShouldNotifyUpgradePanel()
    {
        int upgradeCost = (int)(Mathf.Pow(SaveLoadHandler.Instance.GetPlayerData().totalUpgrades, 1.5f) * 500);
        return EconomyManager.Instance.HasEnoughCoin(upgradeCost);
    }

    private bool ShouldNotifyChampionPanel()
    {
        if (ChampionManager.Instance != null)
        {
            return ChampionManager.Instance.HasAnyBuyable() || ChampionManager.Instance.HasAnyUpgradable();
        }
        return false;
    }

    private bool ShouldNotifyAchievementPanel()
    {
        return SaveLoadHandler.Instance.GetPlayerData().Achievements.Any(achievement => achievement.isCompleted && !achievement.isPrizeTaken);
    }

    private bool ShouldNotifyShopPanel() => EconomyManager.Instance.CurrentSilverKey > 0 || EconomyManager.Instance.CurrentGoldenKey > 0;
}
