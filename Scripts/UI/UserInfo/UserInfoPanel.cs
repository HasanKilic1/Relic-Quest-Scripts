using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserInfoPanel : MonoBehaviour
{
    User user;
    public int MaxLevel;
    [SerializeField] List<UserLevel> levelInfos;
    [SerializeField] Slider progress;
    [SerializeField] TextMeshProUGUI userName;
    [SerializeField] TextMeshProUGUI level;
    [SerializeField] TextMeshProUGUI experience;
    [SerializeField] LevelUpPrizePanel levelUpPanel;
    [SerializeField] Transform panelHolder;

    private void OnEnable()
    {
        User.OnExperienceAdded += OnExperienceAdded;
        User.OnExperienceAdded += HandleUI;
    }

    private void OnDisable()
    {
        User.OnExperienceAdded -= OnExperienceAdded;
        User.OnExperienceAdded -= HandleUI;
    }
    void Start()
    {
        user = SaveLoadHandler.Instance.GetPlayerData().User;
        if(string.IsNullOrEmpty(user.Name))
        {
            RandomUsernameGenerator randomUsernameGenerator = new RandomUsernameGenerator();
            user.Name = randomUsernameGenerator.GenerateUsername();
            SaveLoadHandler.Instance.SaveData();
        }
        OnExperienceAdded();
        HandleUI();
    }

    private void HandleUI()
    {
        userName.text = user.Name;
        level.text = user.Level.ToString();

        string requiredExperince = 

        experience.text = user.Experience.ToString() + " / " + GetRequiredExperience().ToString();

        progress.value = user.Experience;
        progress.maxValue = GetRequiredExperience();
    }

    private int GetRequiredExperience()
    {
        int requiredExperince;
        if (levelInfos.Any(info => info.level == user.Level + 1))
        {
            requiredExperince = levelInfos.Find(info => info.level == user.Level + 1).requiredExperience;
        }
        else requiredExperince = user.Experience;
        return requiredExperince;
    }

    private void OnExperienceAdded()
    {
        if(user.Level == MaxLevel) return;
        
        UserLevel nextLevel = levelInfos.Find(i => i.level == user.Level + 1);
        
        if (user.Experience >= nextLevel.requiredExperience)
        {
            HandleLevelUp(nextLevel);
        }
    }

    private void HandleLevelUp(UserLevel newLevel)
    {        
        user.Level = newLevel.level;
        user.Experience = 0;
        var lvlPanel = Instantiate(levelUpPanel, panelHolder);
        lvlPanel.ShowRewards(newLevel.rewards);
        
        SaveLoadHandler.Instance.SaveData();
    }
}

[Serializable]
public class UserLevel
{
    public int level;
    public int requiredExperience;
    public List<RewardSO> rewards;
}
public class RandomUsernameGenerator
{
    private static readonly string[] adjectives =
    {
        "Swift", "Brave", "Mighty", "Silent", "Clever", "Fierce", "Noble", "Quick", "Bold", "Wise"
    };

    private static readonly string[] nouns =
    {
        "Lion", "Eagle", "Wolf", "Falcon", "Tiger", "Panther", "Dragon", "Phoenix", "Griffin", "Bear"
    };

    public string GenerateUsername()
    {
        System.Random random = new System.Random();
        string adjective = adjectives[random.Next(adjectives.Length)];
        string noun = nouns[random.Next(nouns.Length)];
        int number = random.Next(1000, 9999); // Generate a random number between 1000 and 9999

        return $"{adjective}{noun}{number}";
    }
}