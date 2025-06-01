using System;
using UnityEngine;

public class LoginTracker : MonoBehaviour
{
    public const string lastLoginDateKey = "date";
    public DateTime lastLoginDate;
    public int consecutiveLoginDays;
    public bool playerLoggedInDifferentDay;

    void Start()
    {
        CheckLastEnterDate();
        CalculateConsecutiveLoginDays();
        SaveLastLoginDate();
    }

    private void CheckLastEnterDate()
    {
        if (PlayerPrefs.HasKey(lastLoginDateKey))
        {
            lastLoginDate = DateTime.Parse(PlayerPrefs.GetString(lastLoginDateKey));
        }               
    }

    private void SaveLastLoginDate()
    {
        PlayerPrefs.SetString(lastLoginDateKey, DateTime.UtcNow.ToString());
        PlayerPrefs.Save();
    }
    void CalculateConsecutiveLoginDays()
    {
        string loginKey = "consecutiveLogins";
        if (PlayerPrefs.HasKey(loginKey))
        {
            consecutiveLoginDays = PlayerPrefs.GetInt(loginKey);
        }
        if (DateTime.UtcNow.Subtract(lastLoginDate).Seconds >= 1/*Change this to days later*/)
        {            
            consecutiveLoginDays++;
            Debug.Log("Consecutive login days : " + consecutiveLoginDays);
        }
        // If player logged in today but not yesterday, reset consecutive login days count
        else if (DateTime.UtcNow.Subtract(lastLoginDate).Days > 1)
        {
            consecutiveLoginDays = 1;
        }
        
        PlayerPrefs.SetInt(loginKey, consecutiveLoginDays);
    }
   
    public TimeSpan GetOfflineTimeDiff()
    {   
        lastLoginDate = DateTime.Parse(PlayerPrefs.GetString(lastLoginDateKey));
        return DateTime.UtcNow - lastLoginDate;
    }
}
