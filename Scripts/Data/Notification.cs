using System;

[Serializable]
public class Notification
{
    public Notification() { }
    public bool ShopNotification; // Reset on shop enter
    public bool InventoryNotification;    // Reset on inventory enter
    public bool UpgradeNotification;  // Reset on attribute panel enter
    public bool AchievementNotification;// Reset on achievement panel enter
    public bool ChampionNotification; // Reset on champion panel enter
}