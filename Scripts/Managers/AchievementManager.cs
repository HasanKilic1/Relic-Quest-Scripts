using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;
    [field: SerializeField] public List<AchievementSO> questSOs { get; private set; }    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else Instance = this;
    }

    private void Start()
    {
        Debug.Log("Achievement count on data : " +SaveLoadHandler.Instance.GetPlayerData().Achievements.Count);
    }
    public void EffectQuestByType(QuestType type, int amount)
    {
        List<Achievement> effectedQuests = SaveLoadHandler.Instance.GetPlayerData().Achievements.FindAll(q => q.questType == type);
        foreach (var quest in effectedQuests)
        {
            quest.AddProgress(amount);
        }
        SaveLoadHandler.Instance.SaveData();
    }

    public Achievement GetQuestByID(int id)
    {
        return SaveLoadHandler.Instance.GetPlayerData().Achievements.Find(q => q.Id == id);
    }

    public bool ShouldNotifyAchievementPanel()
    {
        return SaveLoadHandler.Instance.GetPlayerData().Achievements.Any(achievement => achievement.isCompleted && !achievement.isPrizeTaken);
    }
}
