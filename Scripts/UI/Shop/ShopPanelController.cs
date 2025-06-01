using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopPanelController : MonoBehaviour
{    
    void Start()
    {
        ResetShopNotification();
    }

    private void ResetShopNotification()
    {
        SaveLoadHandler.Instance.GetPlayerData().notification.ShopNotification = false;
        NotificationSystem.Instance.ResetNotificationTrigger(NotificationSystem.Shop_Trigger);
    }

}
