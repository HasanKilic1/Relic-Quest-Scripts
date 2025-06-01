using TMPro;
using UnityEngine;

public class TimeBasedReward : MonoBehaviour
{
    public TextMeshProUGUI remainingTimeText;
    public RewardGiver.ResourceGiver reward;

    private const string LastRewardTimeKey = "LastRewardTime";
    [SerializeField] private int RewardIntervalInMinutes = 45;

    void Start()
    {
        CheckReward();
    }

    public void GiveReward()
    {
        PlayerPrefs.SetString(LastRewardTimeKey, System.DateTime.UtcNow.ToString());
        reward.GiveReward();
    }

    void Update()
    {
        CheckReward();
        UpdateRemainingTimeText();
    }

    private void CheckReward()
    {
        // Get the last reward time from PlayerPrefs.
        if (PlayerPrefs.HasKey(LastRewardTimeKey))
        {
            // Parse the last reward time.
            string lastRewardTimeString = PlayerPrefs.GetString(LastRewardTimeKey);
            System.DateTime lastRewardTime = System.DateTime.Parse(lastRewardTimeString);

            // Calculate the time elapsed since the last reward.
            System.TimeSpan timeSinceLastReward = System.DateTime.UtcNow - lastRewardTime;

            // Check if enough time has passed for a new reward.
            if (timeSinceLastReward.TotalMinutes >= RewardIntervalInMinutes)
            {
                //canGiveReward = false;
            }
          //  else canGiveReward = true;
        }
        else
        {
            // If no last reward time is found, initialize it and give the reward.
            PlayerPrefs.SetString(LastRewardTimeKey, System.DateTime.UtcNow.ToString());
          //  canGiveReward = true;
        }
    }

    private void UpdateRemainingTimeText()
    {
        // Get the last reward time from PlayerPrefs.
        if (PlayerPrefs.HasKey(LastRewardTimeKey))
        {
            // Parse the last reward time.
            string lastRewardTimeString = PlayerPrefs.GetString(LastRewardTimeKey);
            System.DateTime lastRewardTime = System.DateTime.Parse(lastRewardTimeString);

            // Calculate the time remaining until the next reward.
            System.TimeSpan timeSinceLastReward = System.DateTime.UtcNow - lastRewardTime;
            float remainingTime = Mathf.Max(0, (float)(RewardIntervalInMinutes * 60 - timeSinceLastReward.TotalSeconds));

            // Update the UI text with the remaining time (formatted as mm:ss).
            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);
            remainingTimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}

