using System;
using UnityEngine;

public class LoginManager : MonoBehaviour
{/*
    [SerializeField] string testLoginDate;
    private readonly string LAST_LOGIN_DATE_KEY = "LAST_LOGIN_DATE";
    private readonly string LAST_EXIT_DATE_KEY = "LAST_EXIT_DATE";
    private readonly string DAILY_CONTINUOUS_LOGIN_KEY = "DAILY_LOGIN";
    public DateTime lastLoginDate {  get; private set; }
    public DateTime lastExitDate { get; private set; }
    public DateTime currentLoginDate { get; private set; }

    void Start()
    {       
        lastLoginDate = DateTime.Parse(PlayerPrefs.GetString(LAST_LOGIN_DATE_KEY));
        currentLoginDate = DateTime.Now;
        PlayerPrefs.SetString(LAST_LOGIN_DATE_KEY, currentLoginDate.ToString());

        if (PlayerPrefs.HasKey(LAST_LOGIN_DATE_KEY))
        {
            if (IsLoggedInTwoDaysContinously())
            {
                PlayerPrefs.SetInt(DAILY_CONTINUOUS_LOGIN_KEY, PlayerPrefs.GetInt(DAILY_CONTINUOUS_LOGIN_KEY) + 1);
            }
        }
    }

    public TimeSpan GetTimeIntervalFromLastExitToNow()
    {
        TimeSpan timeAfterLastExit = currentLoginDate.Subtract(lastExitDate);
        return timeAfterLastExit;
    }
    private bool IsLoggedInTwoDaysContinously()
    {
        return currentLoginDate.Subtract(lastLoginDate).Days > 0;
    }

    public int LoginSeriesCount => PlayerPrefs.GetInt(DAILY_CONTINUOUS_LOGIN_KEY);

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetString(LAST_EXIT_DATE_KEY , DateTime.Now.ToString());
    }*/
}
