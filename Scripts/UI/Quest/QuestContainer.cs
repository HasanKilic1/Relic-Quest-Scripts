using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestContainer : MonoBehaviour
{
    [Header("Achievement Content")]
    [SerializeField] AchievementUI achievementUIPrefab;
    [SerializeField] Transform contentHolder;
    [SerializeField] float timeBetweenAchievementSpawn = 0.5f;
    [HideInInspector] public List<AchievementUI> Achievements { get; private set; }

    [Header("Close")]
    [SerializeField] Button closeButton;

    [Header("Date")]
    [SerializeField] TextMeshProUGUI refreshDateText;
    private const string AchievementRefreshDateKey = "AchievementRefreshDate";

    DateTime lastRefreshDate;
    DateTime targetRefreshDate;

    private void Awake()
    {
        Achievements = new List<AchievementUI>();
        closeButton.onClick.AddListener(() => { Destroy(gameObject); });
    }
    private void Start()
    {
        LoadDateData();
        CheckRefreshCondition();
        StartCoroutine(ShowAchievements());
        ResetAchievementNotification();
    }
    private void ResetAchievementNotification()
    {
        SaveLoadHandler.Instance.GetPlayerData().notification.AchievementNotification = false;
        NotificationSystem.Instance.ResetNotificationTrigger(NotificationSystem.Achievement_Trigger);
    }

    private void Update()
    {
        ShowRemainingTime();
        CheckRefreshCondition();
    }
    private IEnumerator ShowAchievements()
    {
        for (int i = 0; i < AchievementManager.Instance.questSOs.Count; i++)
        {
            AchievementUI achievementUI = Instantiate(achievementUIPrefab, contentHolder.transform);
            achievementUI.SetAchievement(AchievementManager.Instance.questSOs[i]);
            Achievements.Add(achievementUI);
            yield return new WaitForSeconds(timeBetweenAchievementSpawn);
        }
    }

    private void ShowRemainingTime()
    {
        TimeSpan timeBetween = targetRefreshDate - DateTime.UtcNow;
        refreshDateText.text = "Will be refreshed in: " + TimeSpanFormatter.FormatTimeSpanDays(timeBetween);
        //Debug.Log("Remaining time : " + timeBetween);
    }

    private void LoadDateData()
    {
        if (PlayerPrefs.HasKey(AchievementRefreshDateKey)) 
        {
            string savedDate = PlayerPrefs.GetString(AchievementRefreshDateKey);
            lastRefreshDate = DateTime.Parse(savedDate);
        }
        else
        {
            PlayerPrefs.SetString(AchievementRefreshDateKey , DateTime.UtcNow.ToString());
            lastRefreshDate = DateTime.UtcNow;
        }
        targetRefreshDate = lastRefreshDate.AddDays(30);
    }

    private void CheckRefreshCondition()
    {
        if(DateTime.UtcNow > targetRefreshDate)
        {
            foreach (var quest in SaveLoadHandler.Instance.GetPlayerData().Achievements)
            {
                quest.Refresh();
            }
            SaveLoadHandler.Instance.SaveData();
            PlayerPrefs.SetString(AchievementRefreshDateKey, DateTime.UtcNow.ToString());
            targetRefreshDate= DateTime.UtcNow.AddDays(30);
        }
    }
}
[Serializable]
public class ResourceData
{
    public ResourceType ResourceType;
    public int Quantity;

    public void IncreaseQuantity(int quantity)
    {
        this.Quantity += quantity;
    }
}
[Serializable]
public class MultipleResource
{
    public ResourceData[] ResourceArr;
}

public static class TimeSpanFormatter
{
    public static string FormatTimeSpanDays(TimeSpan timeSpan)
    {
        // Get days, hours, minutes, and seconds from the TimeSpan
        int days = timeSpan.Days;
        int hours = timeSpan.Hours;
        int minutes = timeSpan.Minutes;
        int seconds = timeSpan.Seconds;

        // Format the TimeSpan as Days:Hours:Minutes:Seconds
        string formattedTimeSpan = $"{days:D2}:{hours:D2}:{minutes:D2}:{seconds:D2}";

        return formattedTimeSpan;
    }

    public static string FormatTimeSpanHours(TimeSpan timeSpan)
    {
        int hours = timeSpan.Hours;
        int minutes = timeSpan.Minutes;
        int seconds = timeSpan.Seconds;

        // Format the TimeSpan as Days:Hours:Minutes:Seconds
        string formattedTimeSpan = $"{hours:D2}:{minutes:D2}:{seconds:D2}";

        return formattedTimeSpan;
    }
}
